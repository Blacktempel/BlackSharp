// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Platform.Startup.Models;
using BlackSharp.Platform.Startup.Platform.Linux;
using BlackSharp.Platform.Startup.Platform.Windows;
using BlackSharp.Platform.Startup.Utilities;
using System.Diagnostics;

namespace BlackSharp.Platform.Startup.Tests
{
    /// <summary>
    /// Verifies the systemd startup registration behavior.
    /// </summary>
    [TestClass]
    public sealed class StartupRegistrationTests
    {
        #region Public

        /// <summary>
        /// Verifies that Windows quoting preserves paths with spaces and terminal backslashes.
        /// </summary>
        [TestMethod]
        public void WindowsArgumentQuotesTerminalBackslashes()
        {
            // Arrange
            const string argument = @"C:\Program Files\Application\";

            // Act
            var quoted = StartupArgumentQuoting.QuoteWindowsArgument(argument);

            // Assert
            Assert.AreEqual(@"""C:\Program Files\Application\\""", quoted);
        }

        /// <summary>
        /// Verifies that generated systemd directives protect specifiers and environment expansion.
        /// </summary>
        [TestMethod]
        public void SystemdUnitEscapesCommandArguments()
        {
            // Arrange
            var command = new StartupCommand("/opt/example app/bin", new[] { "--name", "a %$b" }, "/opt/example app");

            // Act
            var content = LinuxSystemdStartupRegistration.CreateUnitContent("Example service", command);

            // Assert
            StringAssert.Contains(content, "ExecStart=\"/opt/example app/bin\" \"--name\" \"a %%$$b\"");
            StringAssert.Contains(content, "WorkingDirectory=\"/opt/example app\"");

            Assert.IsFalse(content.Contains("Nice="));
        }

        /// <summary>
        /// Verifies that the nice value appears in the systemd service section.
        /// </summary>
        /// <param name="nice">The scheduling priority for the service.</param>
        /// <param name="directive">The expected line in the service section.</param>
        [TestMethod]
        [DataRow(-20, "Nice=-20")]
        [DataRow( -5, "Nice=-5" )]
        [DataRow(  0, "Nice=0"  )]
        [DataRow(  5, "Nice=5"  )]
        [DataRow( 19, "Nice=19" )]
        public void SystemdUnitIncludesConfiguredNiceValue(int nice, string directive)
        {
            // Arrange
            var command = new StartupCommand("/opt/example/bin", Array.Empty<string>(), "/opt/example");

            // Act
            var content = LinuxSystemdStartupRegistration.CreateUnitContent("Example service", command, nice);

            // Assert
            StringAssert.Contains(content, "WorkingDirectory=\"/opt/example\"\n" + directive + "\nRestart=on-failure\n");
        }

        /// <summary>
        /// Verifies that the generated unit rejects an invalid nice value.
        /// </summary>
        /// <param name="nice">The invalid scheduling priority.</param>
        [TestMethod]
        [DataRow(-21)]
        [DataRow( 20)]
        public void SystemdUnitRejectsInvalidNiceValue(int nice)
        {
            // Arrange
            var command = new StartupCommand("/opt/example/bin", Array.Empty<string>(), "/opt/example");

            // Act and assert
            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => LinuxSystemdStartupRegistration.CreateUnitContent("Example service", command, nice));

            Assert.AreEqual("nice", exception.ParamName);
        }

        /// <summary>
        /// Verifies that Windows boot task registration rejects undefined priority classes.
        /// </summary>
        /// <param name="taskPriority">A numeric value outside the defined process priority classes.</param>
        [TestMethod]
        [DataRow(-1)]
        [DataRow( 7)]
        public void WindowsBootTaskRejectsInvalidPriority(int taskPriority)
        {
            // Arrange
            var command = new StartupCommand(@"C:\Example\Service.exe", Array.Empty<string>(), @"C:\Example");

            // Act
            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => WindowsTaskStartupRegistration.SetBootTaskEnabled(
                    "Example", "Example service", command, true, (ProcessPriorityClass)taskPriority));

            // Assert
            Assert.AreEqual("taskPriority", exception.ParamName);
        }

        /// <summary>
        /// Verifies escaping in XDG autostart entries.
        /// </summary>
        [TestMethod]
        public void DesktopEntryEscapesArguments()
        {
            // Arrange
            var command = new StartupCommand("/opt/example app/bin", new[] { "x %$y" }, "/opt/example app");

            // Act
            var content = LinuxDesktopAutostartRegistration.CreateDesktopEntry("Example", command);

            // Assert
            StringAssert.Contains(content, "Exec=\"/opt/example app/bin\" \"x %%\\$y\"");
        }

        /// <summary>
        /// Verifies that startup commands retain a snapshot of the supplied arguments.
        /// </summary>
        [TestMethod]
        public void StartupCommandCopiesArguments()
        {
            // Arrange
            var arguments = new[] { "first" };

            // Act
            var command = new StartupCommand("/bin/example", arguments, "/bin");
            arguments[0] = "changed";

            // Assert
            Assert.AreEqual("first", command.Arguments[0]);
        }

        #endregion
    }
}
