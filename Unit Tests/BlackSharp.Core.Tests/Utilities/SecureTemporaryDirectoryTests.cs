// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.IO;
using BlackSharp.Core.Security;

using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.Tests.Utilities
{
    /// <summary>
    /// Verifies private temporary directory creation and trusted executable path validation.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class SecureTemporaryDirectoryTests
    {
        #region Public

        /// <summary>
        /// Verifies that a private temporary directory can be created and deleted successfully.
        /// </summary>
        [TestMethod]
        public void CreatesDistinctPrivateDirectoriesWithoutSharedIntermediateParent()
        {
            // Arrange
            const string prefix = "BlackSharp-SecureTest-";

            // Act
            var first  = SecureTemporaryDirectory.Create(prefix);
            var second = SecureTemporaryDirectory.Create(prefix);

            try
            {
                // Assert
                Assert.AreNotEqual(first, second);

                Assert.AreEqual(Path.GetDirectoryName(first), Path.GetDirectoryName(second));

                Assert.IsTrue(Path.GetFileName(first).StartsWith("BlackSharp-SecureTest-", StringComparison.Ordinal));

                if (OS.IsLinux())
                {
                    Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute, File.GetUnixFileMode(first));
                }
            }
            finally
            {
                // Cleanup
                Directory.Delete(first);
                Directory.Delete(second);
            }
        }

        /// <summary>
        /// Verifies that a filename prefix cannot introduce a path traversal or child directory.
        /// </summary>
        [TestMethod]
        public void RejectsPathComponentsInPrefix()
        {
            // Arrange
            const string traversalPrefix = "../escape/";
            const string childPrefix     = "escape\\child";

            // Act and Assert
            Assert.ThrowsExactly<ArgumentException>(() => SecureTemporaryDirectory.Create(traversalPrefix));
            Assert.ThrowsExactly<ArgumentException>(() => SecureTemporaryDirectory.Create(childPrefix));
        }

        /// <summary>
        /// Verifies that a writable Linux temporary parent is rejected before a child is created.
        /// </summary>
        [TestMethod]
        public void LinuxRejectsAnUnprotectedTemporaryParent()
        {
            // Arrange
            if (!OS.IsLinux())
            {
                return;
            }

            var directory = SecureTemporaryDirectory.Create("BlackSharp-UnsafeParent-");
            var previous  = Environment.GetEnvironmentVariable("TMPDIR");

            try
            {
                File.SetUnixFileMode(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute
                    | UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute
                    | UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute);

                Environment.SetEnvironmentVariable("TMPDIR", directory);

                // Act and Assert
                Assert.ThrowsExactly<UnauthorizedAccessException>(() => SecureTemporaryDirectory.Create("child-"));
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable("TMPDIR", previous);

                Directory.Delete(directory);
            }
        }

        /// <summary>
        /// Verifies that an ordinary user's writable deployment cannot supply executable content to privileged startup.
        /// </summary>
        [TestMethod]
        public void TrustedExecutableRejectsAnUnprivilegedWritableDeployment()
        {
            // Arrange
            var directory = Path.Combine(Path.GetTempPath(), "BlackSharp-UntrustedDeployment-" + Guid.NewGuid().ToString("N"));
            var file      = Path.Combine(directory, "application.dll");

            Directory.CreateDirectory(directory);

            try
            {
                File.WriteAllText(file, "test");

                if (OS.IsLinux())
                {
                    File.SetUnixFileMode(file, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.OtherWrite);
                }

                // Act and Assert
                Assert.ThrowsExactly<UnauthorizedAccessException>(() => TrustedExecutablePath.EnsureTrusted(file));
            }
            finally
            {
                // Cleanup
                Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// Verifies that Windows network and device namespace paths are rejected before contacting a remote host.
        /// </summary>
        [TestMethod]
        public void TrustedExecutableRejectsNetworkAndDeviceNamespacePathsWithoutContactingThem()
        {
            // Arrange
            if (!OS.IsWindows())
            {
                return;
            }

            const string networkPath = @"\\untrusted.invalid\share\service.exe";
            const string devicePath  = @"\\?\C:\Windows\System32\cmd.exe";

            // Act and Assert
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => TrustedExecutablePath.EnsureTrusted(networkPath));
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => TrustedExecutablePath.EnsureTrusted(devicePath));
        }

        /// <summary>
        /// Verifies that a protected, regular system executable passes the platform-specific ownership checks.
        /// </summary>
        [TestMethod]
        public void TrustedExecutableAcceptsProtectedSystemBinary()
        {
            if (OS.IsWindows())
            {
                // Arrange
                var executable = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                // Act and Assert
                TrustedExecutablePath.EnsureTrusted(executable);
            }
            else if (OS.IsLinux())
            {
                // Arrange
                // /usr/bin/true is a regular file on supported distributions; /bin may be a symbolic link.
                const string executable = "/usr/bin/true";

                if (File.Exists(executable))
                {
                    // Act and Assert
                    TrustedExecutablePath.EnsureTrusted(executable);
                }
            }
        }

        #endregion
    }
}
