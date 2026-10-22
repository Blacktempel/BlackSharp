// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Logging;

namespace BlackSharp.Core.Tests.Logging
{
    /// <summary>
    /// Verifies bounded file output shared by applications and hardware components.
    /// </summary>
    [TestClass]
    public sealed class LoggerFileModeTests
    {
        #region Public

        /// <summary>
        /// Keeps log destinations independent while rotating oversized files.
        /// </summary>
        [TestMethod]
        public void FileInstancesFilterAndRotateIndependently()
        {
            // Arrange
            var directory  = Path.Combine(Path.GetTempPath(), "BlackSharp-FileLogger-" + Guid.NewGuid().ToString("N"));
            var firstPath  = Path.Combine(directory, "service.log");
            var secondPath = Path.Combine(directory, "tray.log");

            var first  = new Logger(firstPath, LogLevel.Warn, maximumFileSizeBytes: 140, retainedFileCount: 2);
            var second = new Logger(secondPath);

            var exception = new InvalidOperationException("failure");

            try
            {
                // Act
                first.Log(LogLevel.Info, "Ignored", "not written");
                first.Log(LogLevel.Warn, "Service", "first entry");
                first.Log(LogLevel.Error, "Service", "second entry", exception);

                second.Log(LogLevel.Info, "Tray", "tray entry");

                var rawWritten = second.TryWriteRaw("raw entry" + Environment.NewLine);

                // Assert
                Assert.IsTrue(rawWritten);
                Assert.IsFalse(File.ReadAllText(firstPath).Contains("not written", StringComparison.Ordinal));
                Assert.IsTrue(File.Exists(firstPath + ".1"));
                Assert.IsTrue(File.ReadAllText(firstPath + ".1").Contains("first entry", StringComparison.Ordinal));
                Assert.IsTrue(File.ReadAllText(firstPath).Contains("failure", StringComparison.Ordinal));
                Assert.IsTrue(File.ReadAllText(firstPath).Contains($"(0x{exception.HResult:X})", StringComparison.Ordinal));
                Assert.IsTrue(File.ReadAllText(secondPath).Contains("tray entry", StringComparison.Ordinal));
                Assert.IsTrue(File.ReadAllText(secondPath).Contains("raw entry", StringComparison.Ordinal));
                Assert.IsFalse(File.ReadAllText(secondPath).Contains("first entry", StringComparison.Ordinal));
            }
            finally
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, recursive: true);
                }
            }
        }

        /// <summary>
        /// Reports invalid options before attempting file access.
        /// </summary>
        [TestMethod]
        public void InvalidOptionsAreRejected()
        {
            // Act and assert
            Assert.ThrowsExactly<ArgumentException>(() => new Logger(" "));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Logger("log", maximumFileSizeBytes: 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Logger("log", retainedFileCount: 0));
        }

        #endregion
    }
}
