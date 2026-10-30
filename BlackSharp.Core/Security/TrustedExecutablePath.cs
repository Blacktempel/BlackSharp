// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Windows.Native;
using System.ComponentModel;
using System.Runtime.InteropServices;

using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.Security
{
    /// <summary>
    /// Rejects executable paths that an unprivileged account can replace or modify.
    /// </summary>
    public static class TrustedExecutablePath
    {
        #region Fields

        /// <summary>
        /// Selects generic write/all, delete, permission/owner changes and file write/append/attribute rights.
        /// </summary>
        const uint FILE_MUTATION_RIGHTS = 0x40000000 | 0x10000000 | 0x000d0000 | 0x00000116;

        /// <summary>
        /// Selects rights that allow replacement or permission changes beneath an ancestor directory.
        /// </summary>
        const uint ANCESTOR_MUTATION_RIGHTS = 0x10000000 | 0x000d0000 | 0x00000040;

        /// <summary>
        /// Selects subdirectory creation and deletion of children in a deployment directory.
        /// </summary>
        const uint DIRECTORY_MUTATION_RIGHTS = 0x44;

        /// <summary>
        /// Identifies a file or directory object.
        /// </summary>
        const int SE_FILE_OBJECT = 1;

        /// <summary>
        /// Requests the object owner and discretionary access-control list.
        /// </summary>
        const uint OWNER_AND_DACL_SECURITY_INFORMATION = 0x5;

        /// <summary>
        /// Marks an access-control entry that applies only to inherited children.
        /// </summary>
        const byte INHERIT_ONLY_ACE = 0x8;

        #endregion

        #region Public

        /// <summary>
        /// Requires a regular executable or assembly in a protected directory tree.
        /// </summary>
        /// <param name="filePath">An existing file to validate before privileged execution.</param>
        /// <exception cref="ArgumentException">The executable path is empty.</exception>
        /// <exception cref="UnauthorizedAccessException">The file or an ancestor is linked, remotely hosted, or writable by an untrusted account.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support these path checks.</exception>
        /// <exception cref="Win32Exception">The operating system cannot supply the required security metadata.</exception>
        /// <remarks>
        /// Validation requires administrator/root ownership and protected ancestors. Callers must also validate
        /// any companion executable content, such as assemblies and native dependencies, before privileged execution.
        /// </remarks>
        public static void EnsureTrusted(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("An executable path is required.", nameof(filePath));
            }

            var file = new FileInfo(Path.GetFullPath(filePath));

            EnsureTrustedDirectory(file.Directory.FullName);
            EnsureComponent(file.FullName, directory: false, allowChildCreation: false);
        }

        /// <summary>
        /// Requires a protected directory and protected ancestors.
        /// </summary>
        /// <param name="directoryPath">The directory to validate.</param>
        /// <param name="allowChildCreation">Whether ordinary accounts may create unrelated children without replacing existing entries.</param>
        /// <exception cref="UnauthorizedAccessException">A directory is linked, remotely hosted, or writable by an untrusted account.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support these path checks.</exception>
        /// <exception cref="Win32Exception">The operating system cannot supply the required security metadata.</exception>
        /// <remarks>
        /// Ancestors may permit unrelated child creation, but must prevent replacement of the validated path.
        /// The final directory also prevents content injection unless <paramref name="allowChildCreation"/> is enabled.
        /// </remarks>
        public static void EnsureTrustedDirectory(string directoryPath, bool allowChildCreation = false)
        {
            var fullPath = Path.GetFullPath(directoryPath);

            if (OS.IsWindows())
            {
                // Remote administrators can mint the same well-known owner SIDs. A remote ACL is not local trust.
                var root = Path.GetPathRoot(fullPath);

                if (root.StartsWith(@"\\", StringComparison.Ordinal) || new DriveInfo(root).DriveType != DriveType.Fixed)
                {
                    Reject(fullPath);
                }
            }

            var directories = new Stack<string>();

            for (var directory = new DirectoryInfo(fullPath); directory != null; directory = directory.Parent)
            {
                directories.Push(directory.FullName);
            }

            while (directories.Count != 0)
            {
                var path = directories.Pop();

                EnsureComponent(path, directory: true, allowChildCreation: directories.Count != 0 || allowChildCreation);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Validates the type, owner, and mutation permissions of one path component on the current platform.
        /// </summary>
        /// <param name="path">The existing file or directory to inspect.</param>
        /// <param name="directory">Whether the component must be a directory instead of a regular file.</param>
        /// <param name="allowChildCreation">Whether a directory may permit creation of unrelated children.</param>
        private static void EnsureComponent(string path, bool directory, bool allowChildCreation)
        {
            if (OS.IsLinux())
            {
                var metadata = UnixFileSecurity.Read(path);

                if (metadata.UserID != 0
                 || (metadata.Mode & UnixFileSecurity.WritableByOthers) != 0
                 || (metadata.Mode & UnixFileSecurity.FileTypeMask) != (directory ? UnixFileSecurity.DirectoryType : UnixFileSecurity.RegularFileType))
                {
                    Reject(path);
                }

                return;
            }

            if (!OS.IsWindows())
            {
                throw new PlatformNotSupportedException("Privileged executable validation is supported on Windows and Linux.");
            }

            var attributes = File.GetAttributes(path);

            if ((attributes & FileAttributes.ReparsePoint) != 0 || ((attributes & FileAttributes.Directory) != 0) != directory)
            {
                Reject(path);
            }

            var error = AdvApi32.GetNamedSecurityInfo(
                path,
                SE_FILE_OBJECT,
                OWNER_AND_DACL_SECURITY_INFORMATION,
                out var owner,
                out _,
                out var acl,
                out _,
                out var descriptor);

            if (error != 0)
            {
                throw new Win32Exception((int)error, "Cannot inspect executable security: '" + path + "'.");
            }

            try
            {
                if (!IsTrustedSid(owner) || acl == IntPtr.Zero)
                {
                    Reject(path);
                }

                var count    = (ushort)Marshal.ReadInt16(acl, 4);

                var mutation = directory && allowChildCreation
                    ? ANCESTOR_MUTATION_RIGHTS
                    : FILE_MUTATION_RIGHTS | (directory ? DIRECTORY_MUTATION_RIGHTS : 0u);

                for (uint index = 0; index < count; ++index)
                {
                    if (!AdvApi32.GetAce(acl, index, out var ace))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    var type  = Marshal.ReadByte(ace);
                    var flags = Marshal.ReadByte(ace, 1);

                    // Inherit-only entries and the basic, object, and callback deny variants do not grant access.
                    if ((flags & INHERIT_ONLY_ACE) != 0 || type == 1 || type == 6 || type == 10 || type == 12)
                    {
                        continue;
                    }

                    // Only basic, compound, object, and callback allow variants can grant the rights inspected here.
                    if (type != 0 && type != 4 && type != 5 && type != 9 && type != 11)
                    {
                        continue;
                    }

                    var mask = unchecked((uint)Marshal.ReadInt32(ace, 4));

                    if ((mask & mutation) == 0)
                    {
                        continue;
                    }

                    // Nonbasic allow entries carry different SID layouts. Reject them instead of misreading their trustees.
                    if (type != 0 || !IsTrustedSid(IntPtr.Add(ace, 8), allowOwnerRights: true))
                    {
                        Reject(path);
                    }
                }
            }
            finally
            {
                Kernel32.LocalFree(descriptor);
            }
        }

        /// <summary>
        /// Identifies SYSTEM, administrators, TrustedInstaller and optionally the previously validated owner.
        /// </summary>
        /// <param name="sid">The native security identifier to inspect.</param>
        /// <param name="allowOwnerRights">Whether the OWNER RIGHTS identifier is accepted after owner validation.</param>
        /// <returns>Whether the identifier represents a trusted principal.</returns>
        private static bool IsTrustedSid(IntPtr sid, bool allowOwnerRights = false)
        {
            if (sid == IntPtr.Zero || !AdvApi32.ConvertSidToStringSid(sid, out var text))
            {
                return false;
            }

            try
            {
                var value = Marshal.PtrToStringUni(text);

                return (allowOwnerRights && value == "S-1-3-4")
                    || value == "S-1-5-18"
                    || value == "S-1-5-32-544"
                    || value == "S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
            }
            finally
            {
                Kernel32.LocalFree(text);
            }
        }

        /// <summary>
        /// Rejects a path that cannot safely provide executable content to a privileged process.
        /// </summary>
        /// <param name="path">The path that failed a security requirement.</param>
        /// <exception cref="UnauthorizedAccessException">Always thrown to reject privileged use of the path.</exception>
        private static void Reject(string path)
        {
            throw new UnauthorizedAccessException(
                "Privileged execution requires administrator/root-owned files and directories without unprivileged write access or links: '"
                + path + "'. Install the service in a protected directory first.");
        }

        #endregion
    }
}
