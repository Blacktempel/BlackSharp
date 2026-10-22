/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Platform.Startup.Models;
using BlackSharp.Platform.Startup.Platform.Linux;
using BlackSharp.Platform.Startup.Utilities;

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
