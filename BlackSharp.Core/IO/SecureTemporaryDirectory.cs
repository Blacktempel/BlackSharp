// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Linux.Native;
using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;
using BlackSharp.Core.Security;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.IO
{
    /// <summary>
    /// Creates private, atomically reserved working directories for executable or sensitive temporary data.
    /// </summary>
    public static class SecureTemporaryDirectory
    {
        #region Fields

        /// <summary>
        /// Identifies a private access-control list that grants full control to the system,
        /// administrators and denies access to all other users.
        /// </summary>
        const string PRIVATE_DIRECTORY_SECURITY = "D:P(A;OICI;FA;;;SY)(A;OICI;FA;;;BA)(A;OICI;FA;;;OW)";

        /// <summary>
        /// Identifies the supported security descriptor revision.
        /// </summary>
        const uint SECURITY_DESCRIPTOR_REVISION = 1;

        /// <summary>
        /// Indicates that another entry already occupies a generated directory name.
        /// </summary>
        const int ERROR_ALREADY_EXISTS = 183;

        #endregion

        #region Public

        /// <summary>
        /// Creates a unique private directory whose lifetime is managed by the caller.
        /// </summary>
        /// <param name="prefix">A filename prefix of at most 80 characters.</param>
        /// <returns>The absolute path of the newly reserved directory.</returns>
        /// <exception cref="ArgumentException">The prefix is empty or contains unsupported filename characters.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system is neither Windows nor Linux.</exception>
        /// <exception cref="UnauthorizedAccessException">The parent directory does not satisfy the ownership and permission requirements.</exception>
        /// <exception cref="Win32Exception">The operating system cannot create the private directory or security descriptor.</exception>
        /// <exception cref="IOException">The parent directory is unavailable or a unique name cannot be reserved.</exception>
        /// <remarks>
        /// Linux uses an exclusively created directory with mode 0700 under a validated temporary parent.
        /// Windows uses protected ProgramData ancestry and an explicit private access-control list.
        /// The caller must remove the directory when it is no longer needed.
        /// </remarks>
        public static string Create(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix)
             || prefix.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
             || prefix.IndexOf('/') >= 0
             || prefix.IndexOf('\\') >= 0
             || prefix.Length > 80)
            {
                throw new ArgumentException("A simple temporary-directory prefix is required.", nameof(prefix));
            }

            if (OS.IsLinux())
            {
                return CreateLinuxDirectory(prefix);
            }

            if (OS.IsWindows())
            {
                return CreateWindowsDirectory(prefix);
            }

            throw new PlatformNotSupportedException("Private temporary directories are supported on Windows and Linux.");
        }

        #endregion

        #region Private

        /// <summary>
        /// Atomically creates a mode-0700 directory beneath a validated Linux temporary parent.
        /// </summary>
        /// <param name="prefix">The validated filename prefix for the directory.</param>
        /// <returns>The absolute path returned in the native directory template.</returns>
        private static string CreateLinuxDirectory(string prefix)
        {
            var parent = Path.GetFullPath(Path.GetTempPath());

            UnixFileSecurity.EnsureTemporaryParent(parent);

            var template = Encoding.UTF8.GetBytes(Path.Combine(parent, prefix + "XXXXXX") + "\0");

            if (LibC.mkdtemp(template) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot create a private temporary directory.");
            }

            return Encoding.UTF8.GetString(template, 0, Array.IndexOf(template, (byte)0));
        }

        /// <summary>
        /// Atomically creates a Windows directory with a private, noninherited access-control list.
        /// </summary>
        /// <param name="prefix">The validated filename prefix for the directory.</param>
        /// <returns>The absolute path of the exclusively created directory.</returns>
        private static string CreateWindowsDirectory(string prefix)
        {
            // Executable temporary data must not trust caller-controlled TEMP or mutable profile ancestry.
            // ProgramData permits child creation while protecting existing children from other users.
            var parent = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            if (string.IsNullOrWhiteSpace(parent))
            {
                throw new IOException("The temporary directory is unavailable.");
            }

            TrustedExecutablePath.EnsureTrustedDirectory(parent, allowChildCreation: true);

            if (!AdvApi32.ConvertStringSecurityDescriptorToSecurityDescriptor(
                PRIVATE_DIRECTORY_SECURITY,
                SECURITY_DESCRIPTOR_REVISION,
                out var descriptor,
                out _))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            try
            {
                var security = new SecurityAttributes
                {
                    Length             = (uint)Marshal.SizeOf<SecurityAttributes>(),
                    SecurityDescriptor = descriptor
                };

                // Create a unique directory name and attempt to create it with the private security descriptor.
                for (var attempt = 0; attempt < 10; ++attempt)
                {
                    var path = Path.Combine(Path.GetFullPath(parent), prefix + Guid.NewGuid().ToString("N"));

                    if (Kernel32.CreateDirectory(path, ref security))
                    {
                        return path;
                    }

                    var error = Marshal.GetLastWin32Error();

                    if (error != ERROR_ALREADY_EXISTS)
                    {
                        throw new Win32Exception(error, "Cannot create a private temporary directory.");
                    }
                }

                throw new IOException("Cannot reserve a unique temporary directory.");
            }
            finally
            {
                Kernel32.LocalFree(descriptor);
            }
        }

        #endregion
    }
}
