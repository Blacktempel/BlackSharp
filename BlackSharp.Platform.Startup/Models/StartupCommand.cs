// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Platform.Startup.Models
{
    /// <summary>
    /// Describes a process command registered for operating-system startup.
    /// </summary>
    public sealed class StartupCommand
    {
        #region Constructor

        /// <summary>
        /// Initializes an executable, its separate arguments and its working directory.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The command-line arguments.</param>
        /// <param name="workingDirectory">The process working directory.</param>
        public StartupCommand(string executablePath, IEnumerable<string> arguments, string workingDirectory)
        {
            ExecutablePath = string.IsNullOrWhiteSpace(executablePath)
                ? throw new ArgumentException("An executable path is required.", nameof(executablePath))
                : executablePath;

            Arguments = arguments?.ToArray() ?? throw new ArgumentNullException(nameof(arguments));

            WorkingDirectory = string.IsNullOrWhiteSpace(workingDirectory)
                ? throw new ArgumentException("A working directory is required.", nameof(workingDirectory))
                : workingDirectory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        public string ExecutablePath { get; }

        /// <summary>
        /// Gets the separate command-line arguments.
        /// </summary>
        public IReadOnlyList<string> Arguments { get; }

        /// <summary>
        /// Gets the process working directory.
        /// </summary>
        public string WorkingDirectory { get; }

        #endregion
    }
}
