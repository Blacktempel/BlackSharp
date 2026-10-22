// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Logging;

namespace BlackSharp.Core.Tests.Logging
{
    /// <summary>
    /// Verifies independent in-memory logger instances and severity filtering.
    /// </summary>
    [TestClass]
    public class LoggerTests
    {
        /// <summary>
        /// Verifies that entries are retained only by the instance that receives them.
        /// </summary>
        [TestMethod]
        public void Add()
        {
            // Arrange
            var message0 = "Add Message 0";
            var message1 = "Add Message 1";
            var message2 = "Add Message 2";

            var logger   = new Logger { IsEnabled = true, LogLevel = LogLevel.Debug };
            var other    = new Logger { IsEnabled = true, LogLevel = LogLevel.Debug };

            // Act
            logger.Add(LogLevel.Debug, message0, DateTime.Now);
            logger.Add(LogLevel.Debug, message1);
            logger.Add(new LogItem(LogLevel.Debug, message2, DateTime.Now));

            var result = logger.ToString();

            // Assert
            Assert.AreEqual(LogLevel.Debug, logger.LogLevel);
            Assert.Contains(message0, result);
            Assert.Contains(message1, result);
            Assert.Contains(message2, result);
            Assert.IsEmpty(other);
        }

        /// <summary>
        /// Verifies that removing one severity affects only the current instance.
        /// </summary>
        [TestMethod]
        public void Remove()
        {
            // Arrange
            var message = "Remove Message";
            var logger  = new Logger { IsEnabled = true, LogLevel = LogLevel.Debug };

            // Act
            logger.Add(LogLevel.Debug, message, DateTime.Now);
            logger.Remove(LogLevel.Debug);

            // Assert
            Assert.DoesNotContain(message, logger.ToString());
        }

        /// <summary>
        /// Verifies the textual label for a diagnostic severity.
        /// </summary>
        [TestMethod]
        public void GetStringForLogLevel()
        {
            // Act
            var result = Logger.GetStringForLogLevel(LogLevel.Debug);

            // Assert
            Assert.AreEqual("[DEBUG]", result);
        }

        /// <summary>
        /// Includes the exception type and Core exception details in retained entries.
        /// </summary>
        [TestMethod]
        public void LogFormatsExceptionWithCoreDetails()
        {
            // Arrange
            var logger = new Logger { IsEnabled = true };

            var exception = new InvalidOperationException("outer failure", new ArgumentException("inner failure"));

            // Act
            logger.Log(LogLevel.Error, "Test", "operation failed", exception);

            var result = logger.ToString();

            // Assert
            Assert.Contains(typeof(InvalidOperationException).FullName, result);
            Assert.Contains($"(0x{exception.HResult:X})", result);
            Assert.Contains("inner failure", result);
            Assert.Contains("Info:", result);
        }

        /// <summary>
        /// Appends only entries that were added after the previous save.
        /// </summary>
        [TestMethod]
        public void MemoryOnlyLoggerDoesNotDuplicateSavedEntries()
        {
            // Arrange
            var path   = Path.Combine(Path.GetTempPath(), "BlackSharp-Logger-" + Guid.NewGuid().ToString("N") + ".log");
            var logger = new Logger { IsEnabled = true, LogLevel = LogLevel.Info };

            try
            {
                // Act
                logger.Add(LogLevel.Info, "first");
                var firstSaved  = logger.SaveToFile(path);
                var secondSaved = logger.SaveToFile(path);

                logger.Add(LogLevel.Info, "second");
                var thirdSaved  = logger.SaveToFile(path);

                var content = File.ReadAllText(path);

                // Assert
                Assert.IsTrue(firstSaved);
                Assert.IsTrue(secondSaved);
                Assert.IsTrue(thirdSaved);

                Assert.AreEqual(1, content.Split(new[] { "first"  }, StringSplitOptions.None).Length - 1);
                Assert.AreEqual(1, content.Split(new[] { "second" }, StringSplitOptions.None).Length - 1);

                Assert.IsFalse(logger.TryWriteRaw("raw"));
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
