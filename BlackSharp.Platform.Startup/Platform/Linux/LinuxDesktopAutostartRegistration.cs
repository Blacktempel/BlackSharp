// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Platform.Startup.Models;
using BlackSharp.Platform.Startup.Utilities;
using System.Text;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

#if !NET8_0_OR_GREATER
using BlackSharp.Core.Interop.Linux.Native;
#endif

#pragma warning disable CA1416 // Platform compatibility warning

namespace BlackSharp.Platform.Startup.Platform.Linux
{
    /// <summary>
    /// Writes XDG desktop entries that start applications when a graphical Linux session opens.
    /// </summary>
    public static class LinuxDesktopAutostartRegistration
    {
        #region Fields

#if !NET8_0_OR_GREATER
        /// <summary>Grants the owner read/write and everyone else read access (0644).</summary>
        private const uint WORLD_READABLE_FILE_MODE = 0x1A4;
#endif

#endregion

        #region Public

        /// <summary>
        /// Gets the current user's XDG autostart directory.
        /// </summary>
        /// <returns>The absolute autostart directory path.</returns>
        public static string GetCurrentUserAutostartDirectory()
        {
            var configurationDirectory = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");

            if (string.IsNullOrWhiteSpace(configurationDirectory))
            {
                configurationDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
            }

            return Path.Combine(configurationDirectory, "autostart");
        }

        /// <summary>
        /// Creates or removes an XDG desktop autostart entry.
        /// </summary>
        /// <param name="desktopEntryPath">The absolute desktop-entry path.</param>
        /// <param name="displayName">The name shown to the desktop environment.</param>
        /// <param name="command">The process command when enabling the entry.</param>
        /// <param name="enabled">Whether the entry should exist.</param>
        /// <param name="worldReadable">Whether other users may read a system-wide entry.</param>
        public static void SetEnabled(string desktopEntryPath, string displayName, StartupCommand command, bool enabled, bool worldReadable = false)
        {
            if (!PlatformOperatingSystem.IsLinux())
            {
                throw new PlatformNotSupportedException("XDG autostart registration requires Linux.");
            }

            if (!enabled)
            {
                if (File.Exists(desktopEntryPath))
                {
                    File.Delete(desktopEntryPath);
                }

                return;
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var directory = Path.GetDirectoryName(desktopEntryPath)
                ?? throw new ArgumentException("A desktop-entry directory is required.", nameof(desktopEntryPath));

            Directory.CreateDirectory(directory);

            File.WriteAllText(desktopEntryPath, CreateDesktopEntry(displayName, command), new UTF8Encoding(false));

            if (worldReadable)
            {
#if NET8_0_OR_GREATER
                File.SetUnixFileMode(desktopEntryPath,
                    UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead);
#else
                if (LibC.chmod(desktopEntryPath, WORLD_READABLE_FILE_MODE) != 0)
                {
                    throw new IOException($"Could not set permissions for '{desktopEntryPath}'.", LibC.LastErrnoException());
                }
#endif
            }
        }

        /// <summary>
        /// Creates the contents of a desktop entry.
        /// </summary>
        /// <param name="displayName">The entry name.</param>
        /// <param name="command">The process command.</param>
        /// <returns>The desktop-entry content.</returns>
        public static string CreateDesktopEntry(string displayName, StartupCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (displayName == null || displayName.Contains('\n') || displayName.Contains('\r'))
            {
                throw new ArgumentException("A single-line display name is required.", nameof(displayName));
            }

            var executable = StartupArgumentQuoting.QuoteDesktopArgument(command.ExecutablePath);

            var arguments = string.Join(" ", command.Arguments.Select(StartupArgumentQuoting.QuoteDesktopArgument));

            return "[Desktop Entry]\nType=Application\nName=" + displayName + "\n"
                + "Exec=" + executable + (arguments.Length == 0 ? string.Empty : " " + arguments) + "\n"
                + "Terminal=false\nNoDisplay=true\n";
        }

#endregion
    }
}

#pragma warning restore CA1416 // Platform compatibility warning
