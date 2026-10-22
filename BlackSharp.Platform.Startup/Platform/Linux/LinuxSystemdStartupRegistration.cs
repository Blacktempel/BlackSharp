/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Platform.Startup.Models;
using BlackSharp.Platform.Startup.Utilities;
using System.Diagnostics;
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
                File.WriteAllText(unitPath, CreateUnitContent(description, command), new UTF8Encoding(false));

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
        /// Creates the systemd unit file content without changing the file system.
        /// </summary>
        /// <param name="description">The unit description.</param>
        /// <param name="command">The process command.</param>
        /// <returns>The UTF-8 unit file content.</returns>
        public static string CreateUnitContent(string description, StartupCommand command)
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
                .Select(argument => StartupArgumentQuoting.QuoteSystemdArgument(argument, escapeEnvironmentVariables: true)));

            return "[Unit]\n"
                + "Description=" + description + "\n"
                + "After=local-fs.target network.target\n\n"
                + "[Service]\nType=simple\n"
                + "ExecStart=" + commandLine + "\n"
                + "WorkingDirectory=" + StartupArgumentQuoting.QuoteSystemdArgument(command.WorkingDirectory) + "\n"
                + "Restart=on-failure\nRestartSec=5s\nUMask=0077\n\n"
                + "[Install]\nWantedBy=multi-user.target\n";
        }

        #endregion

        #region Private

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

            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

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
