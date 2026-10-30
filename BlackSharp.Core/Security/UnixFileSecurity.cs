// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Linux.Native;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Security
{
    /// <summary>
    /// Represents the access control and audit security for a file or directory on Unix-based systems.
    /// </summary>
    internal static class UnixFileSecurity
    {
        #region Fields

        /// <summary>
        /// Identifies a directory in the file-type portion of a Unix mode.
        /// </summary>
        internal const uint DirectoryType = 0x4000;

        /// <summary>
        /// Identifies a regular file in the file-type portion of a Unix mode.
        /// </summary>
        internal const uint RegularFileType = 0x8000;

        /// <summary>
        /// Selects the file-type portion of a Unix mode.
        /// </summary>
        internal const uint FileTypeMask = 0xf000;

        /// <summary>
        /// Selects group-write and other-write permissions, corresponding to octal 0022.
        /// </summary>
        internal const uint WritableByOthers = 0x12;

        /// <summary>
        /// Selects the sticky permission bit, corresponding to octal 01000.
        /// </summary>
        internal const uint StickyBit = 0x200;

        /// <summary>
        /// Resolves a relative statx path from the current working directory.
        /// </summary>
        private const int AT_FDCWD = -100;

        /// <summary>
        /// Requests metadata for a symbolic link itself rather than its target.
        /// </summary>
        private const int AT_SYMLINK_NOFOLLOW = 0x100;

        /// <summary>
        /// Requests and verifies the file type, permissions, and owner identifier.
        /// </summary>
        private const uint REQUIRED_METADATA = 0xb;

        #endregion

        #region Internal

        /// <summary>
        /// Gets the access control and audit security for a file or directory on Unix-based systems.
        /// </summary>
        /// <param name="path">The file or directory whose metadata is required.</param>
        /// <returns>The owner identifier and the complete Unix mode, including file-type bits.</returns>
        /// <exception cref="IOException">The filesystem does not supply all required metadata.</exception>
        /// <exception cref="Win32Exception">The native statx request fails.</exception>
        /// <exception cref="PlatformNotSupportedException">The C library does not export the required statx function.</exception>
        internal static (uint UserID, uint Mode) Read(string path)
        {
            try
            {
                if (LibC.statx(AT_FDCWD, path, AT_SYMLINK_NOFOLLOW, REQUIRED_METADATA, out var metadata) != 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot inspect '" + path + "' using statx.");
                }

                if ((metadata.Mask & REQUIRED_METADATA) != REQUIRED_METADATA)
                {
                    throw new IOException("The filesystem did not return file ownership and mode information.");
                }

                return (metadata.UserID, metadata.Mode);
            }
            catch (EntryPointNotFoundException exception)
            {
                throw new PlatformNotSupportedException("Secure Linux path checks require statx.", exception);
            }
        }

        /// <summary>
        /// Validates that a temporary directory and its ancestors cannot be replaced by an untrusted account.
        /// </summary>
        /// <param name="path">The absolute or relative temporary parent directory.</param>
        /// <exception cref="UnauthorizedAccessException">An ancestor has an untrusted owner, unsafe permissions or a symbolic link.</exception>
        /// <remarks>
        /// This method is intended to be used for temporary directories that are created by a trusted account, such as the current user or root.
        /// It is not intended to be used for arbitrary directories on the filesystem.
        /// </remarks>
        internal static void EnsureTemporaryParent(string path)
        {
            var userID = LibC.geteuid();

            // Walk up the directory tree, checking each ancestor for ownership and permissions.
            for (var directory = new DirectoryInfo(Path.GetFullPath(path)); directory != null; directory = directory.Parent)
            {
                var metadata   = Read(directory.FullName);
                var sharedRoot = metadata.UserID == 0 && (metadata.Mode & StickyBit) != 0;

                if ((metadata.Mode & FileTypeMask) != DirectoryType
                 || (metadata.UserID != 0 && metadata.UserID != userID)
                 || ((metadata.Mode & WritableByOthers) != 0 && !sharedRoot))
                {
                    throw new UnauthorizedAccessException(
                        "The temporary directory has an untrusted owner, permissions or symbolic link: '" + directory.FullName + "'.");
                }
            }
        }

        #endregion
    }
}
