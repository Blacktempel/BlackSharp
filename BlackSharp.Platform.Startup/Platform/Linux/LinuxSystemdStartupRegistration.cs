// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Platform.Startup.Models;
using BlackSharp.Platform.Startup.Utilities;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Platform.Startup.Platform.Linux
{
    /// <summary>
    /// Registers a system-wide service to start during Linux boot using systemd.
    /// </summary>
    public static class LinuxSystemdStartupRegistration
    {
        #region Public

        /// <summary>
        /// Creates or removes a system unit in <c>/etc/systemd/system</c>.
        /// </summary>
        /// <param name="unitName">The systemd unit file name.</param>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The command to run when enabling the unit.</param>
        /// <param name="enabled">Whether the unit should be registered.</param>
        public static void SetEnabled(string unitName, string description, StartupCommand command, bool enabled)
        {
            SetEnabledCore(unitName, description, command, enabled, null);
        }

        /// <summary>
        /// Creates a system unit with the specified nice value or removes the unit.
        /// </summary>
        /// <param name="unitName">The systemd unit file name.</param>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The command to run when enabling the unit.</param>
        /// <param name="enabled">Whether the unit should be registered.</param>
        /// <param name="nice">The Linux nice value from -20 to 19.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="nice"/> is out of range.</exception>
        /// <exception cref="PlatformNotSupportedException">Linux systemd registration is unavailable.</exception>
        /// <exception cref="ArgumentException">The unit name or description has an invalid format.</exception>
        /// <exception cref="ArgumentNullException">The unit is enabled and <paramref name="command"/> is null.</exception>
        public static void SetEnabled(string unitName, string description, StartupCommand command, bool enabled, int nice)
        {
            ValidateNice(nice);
            SetEnabledCore(unitName, description, command, enabled, nice);
        }

        /// <summary>
        /// Creates the systemd unit file content.
        /// </summary>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The process command.</param>
        /// <returns>The unit file content.</returns>
        public static string CreateUnitContent(string description, StartupCommand command)
        {
            return CreateUnitContentCore(description, command, null);
        }

        /// <summary>
        /// Creates the systemd unit file content with a <c>Nice=</c> directive.
        /// </summary>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The process command.</param>
        /// <param name="nice">The Linux nice value from -20 to 19.</param>
        /// <returns>The unit file content.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="nice"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="description"/> is null or spans multiple lines.</exception>
        public static string CreateUnitContent(string description, StartupCommand command, int nice)
        {
            ValidateNice(nice);
            return CreateUnitContentCore(description, command, nice);
        }

        #endregion

        #region Private

        /// <summary>
        /// Updates the unit file and invokes systemctl to apply its registration.
        /// </summary>
        /// <param name="unitName">The systemd unit file name.</param>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The command to run when enabling the unit.</param>
        /// <param name="enabled">Whether the unit should be registered.</param>
        /// <param name="nice">The nice value. Null omits the directive.</param>
        private static void SetEnabledCore(string unitName, string description, StartupCommand command, bool enabled, int? nice)
        {
            if (!PlatformOperatingSystem.IsLinux())
            {
                throw new PlatformNotSupportedException("Systemd startup registration requires Linux.");
            }

            if (string.IsNullOrWhiteSpace(unitName) || Path.GetFileName(unitName) != unitName || !unitName.EndsWith(".service", StringComparison.Ordinal))
            {
                throw new ArgumentException("A simple .service unit name is required.", nameof(unitName));
            }

            var unitPath = Path.Combine("/etc/systemd/system", unitName);

            if (!enabled && !File.Exists(unitPath))
            {
                return;
            }

            var systemctl = GetSystemControlPath();

            if (!enabled)
            {
                RunRequiredProcess(systemctl, "disable", unitName);

                File.Delete(unitPath);

                RunRequiredProcess(systemctl, "daemon-reload");
                return;
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var previousContent = File.Exists(unitPath) ? File.ReadAllText(unitPath) : null;

            try
            {
                File.WriteAllText(unitPath, CreateUnitContentCore(description, command, nice), new UTF8Encoding(false));

                RunRequiredProcess(systemctl, "daemon-reload");
                RunRequiredProcess(systemctl, "enable", unitName);
            }
            catch
            {
                if (previousContent == null)
                {
                    File.Delete(unitPath);
                }
                else
                {
                    File.WriteAllText(unitPath, previousContent, new UTF8Encoding(false));
                }

                try
                {
                    RunProcess(systemctl, "daemon-reload");
                }
                catch
                {
                    // Preserve the original registration failure.
                }

                throw;
            }
        }

        /// <summary>
        /// Builds the unit directives for the supplied command.
        /// </summary>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The process command.</param>
        /// <param name="nice">The nice value. Null omits the directive.</param>
        /// <returns>The unit file content.</returns>
        private static string CreateUnitContentCore(string description, StartupCommand command, int? nice)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (description == null || description.Contains('\n') || description.Contains('\r'))
            {
                throw new ArgumentException("A single-line description is required.", nameof(description));
            }

            var commandLine = string.Join(" ", new[] { command.ExecutablePath }
                .Concat(command.Arguments)
                .Select(argument => StartupArgumentQuoting.QuoteSystemdArgument(argument, true)));

            var niceDirective = nice.HasValue ? "Nice=" + nice.Value.ToString(CultureInfo.InvariantCulture) + "\n" : string.Empty;

            return "[Unit]\n"
                + "Description=" + description + "\n"
                + "After=local-fs.target network.target\n\n"
                + "[Service]\nType=simple\n"
                + "ExecStart=" + commandLine + "\n"
                + "WorkingDirectory=" + StartupArgumentQuoting.QuoteSystemdArgument(command.WorkingDirectory) + "\n"
                + niceDirective
                + "Restart=on-failure\nRestartSec=5s\nUMask=0077\n\n"
                + "[Install]\nWantedBy=multi-user.target\n";
        }

        /// <summary>
        /// Rejects a nice value outside the Linux scheduling range.
        /// </summary>
        /// <param name="nice">The Linux nice value.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="nice"/> is out of range.</exception>
        private static void ValidateNice(int nice)
        {
            if (nice is < -20 or > 19)
            {
                throw new ArgumentOutOfRangeException(nameof(nice));
            }
        }

        /// <summary>
        /// Resolves the systemctl executable in a standard system directory.
        /// </summary>
        /// <returns>The executable path.</returns>
        private static string GetSystemControlPath()
        {
            foreach (var path in new[] { "/usr/bin/systemctl", "/bin/systemctl" })
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new PlatformNotSupportedException("systemd is required for boot-time service registration.");
        }

        /// <summary>
        /// Runs systemctl and captures its result.
        /// </summary>
        /// <param name="executablePath">The systemctl path.</param>
        /// <param name="arguments">The separate command arguments.</param>
        /// <returns>The exit code and captured output streams.</returns>
        private static (int ExitCode, string Output, string Error) RunProcess(string executablePath, params string[] arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName               = executablePath,
                CreateNoWindow         = true,
                RedirectStandardError  = true,
                RedirectStandardOutput = true,
                UseShellExecute        = false,
            };

#if NET8_0_OR_GREATER
            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }
#else
            startInfo.Arguments = string.Join(" ", arguments.Select(StartupArgumentQuoting.QuoteWindowsArgument));
#endif

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException($"The startup registration utility '{executablePath}' could not be started.");

            var output = process.StandardOutput.ReadToEnd();
            var error  = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return (process.ExitCode, output, error);
        }

        /// <summary>
        /// Runs systemctl and rejects an unsuccessful result.
        /// </summary>
        /// <param name="executablePath">The systemctl path.</param>
        /// <param name="arguments">The separate command arguments.</param>
        private static void RunRequiredProcess(string executablePath, params string[] arguments)
        {
            var result = RunProcess(executablePath, arguments);

            if (result.ExitCode == 0)
            {
                return;
            }

            var message = string.IsNullOrWhiteSpace(result.Error) ? result.Output : result.Error;

            throw new InvalidOperationException(
                $"The startup registration utility '{executablePath}' failed with exit code {result.ExitCode}: {message.Trim()}");
        }

#endregion
    }
}
