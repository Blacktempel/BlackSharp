// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.IO;
using System.Security.AccessControl;
using System.Security.Principal;

using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.Tests.Utilities
{
    /// <summary>
    /// Verifies that Windows executable temporary data uses protected ProgramData ancestry and a private access-control list.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public sealed class WindowsTemporaryParentTests
    {
        #region Public

        /// <summary>
        /// Verifies that the caller can use a newly reserved directory and only trusted principals receive access.
        /// </summary>
        [TestMethod]
        public void CreatesAFunctionalPrivateDirectoryUnderProtectedProgramData()
        {
            // Arrange
            if (!OS.IsWindows())
            {
                return;
            }

            const string payload = "private payload";
            var expectedParent   = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // Act
            var directory = SecureTemporaryDirectory.Create("BlackSharp-UserTemp-");
            var file      = Path.Combine(directory, "payload.tmp");

            try
            {
                File.WriteAllText(file, payload);

                var actualPayload = File.ReadAllText(file);
                var rules         = new DirectoryInfo(directory).GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));

                // Assert
                Assert.AreEqual(expectedParent, Path.GetDirectoryName(directory), ignoreCase: true);
                Assert.AreEqual(payload, actualPayload);

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (rule.AccessControlType != AccessControlType.Allow)
                    {
                        continue;
                    }

                    var sid = rule.IdentityReference.Value;

                    Assert.IsTrue(sid == "S-1-5-18" || sid == "S-1-5-32-544" || sid == "S-1-3-4",
                        "The private directory must only grant access to SYSTEM, administrators and its owner.");
                }
            }
            finally
            {
                // Cleanup
                File.Delete(file);
                Directory.Delete(directory);
            }
        }

        /// <summary>
        /// Verifies that caller-controlled TEMP and TMP values cannot redirect executable temporary data into an unsafe parent.
        /// </summary>
        [TestMethod]
        public void IgnoresWorldWritableCustomTemporaryParentBeforeCreatingAChild()
        {
            // Arrange
            if (!OS.IsWindows())
            {
                return;
            }

            var directory    = SecureTemporaryDirectory.Create("BlackSharp-UnsafeTemp-");
            var info         = new DirectoryInfo(directory);

            var original     = info.GetAccessControl(AccessControlSections.Access);
            var insecure     = info.GetAccessControl(AccessControlSections.Access);

            var previousTmp  = Environment.GetEnvironmentVariable("TMP");
            var previousTemp = Environment.GetEnvironmentVariable("TEMP");

            string created = null;

            try
            {
                insecure.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                info.SetAccessControl(insecure);

                Environment.SetEnvironmentVariable("TMP", directory);
                Environment.SetEnvironmentVariable("TEMP", directory);

                // Act
                created = SecureTemporaryDirectory.Create("protected-child-");

                // Assert
                Assert.AreEqual(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    Path.GetDirectoryName(created), ignoreCase: true);
                Assert.AreNotEqual(directory, Path.GetDirectoryName(created));
                Assert.AreEqual(0, Directory.GetFileSystemEntries(directory).Length);
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable("TMP", previousTmp);
                Environment.SetEnvironmentVariable("TEMP", previousTemp);

                info.SetAccessControl(original);

                if (created != null)
                {
                    Directory.Delete(created);
                }

                Directory.Delete(directory);
            }
        }

        #endregion
    }
}
