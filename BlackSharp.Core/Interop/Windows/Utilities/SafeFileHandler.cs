/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Enums;
using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;

namespace BlackSharp.Core.Interop.Windows.Utilities
{
    /// <summary>
    /// Abstracted low level file operations via <see cref="Kernel32"/>.
    /// </summary>
    public static class SafeFileHandler
    {
        #region Public

        /// <summary>
        /// Opens a handle with given physical path.
        /// </summary>
        /// <param name="physicalPath">Physical path to open handle for.</param>
        /// <returns>Returns opened handle.</returns>
        public static IntPtr OpenHandle(string physicalPath)
        {
            return Kernel32.CreateFile(physicalPath,
                DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
                FileShareMode.Read | FileShareMode.Write,
                IntPtr.Zero,
                FileCreationDisposition.OpenExisting,
                0,
                IntPtr.Zero);
        }

        /// <summary>
        /// Opens a handle with given physical path.
        /// </summary>
        /// <param name="physicalPath">Physical path to open handle for.</param>
        /// <param name="fileFlagsAndAttributes">File flags and attributes to use when opening the handle.</param>
        /// <returns>Returns opened handle.</returns>
        public static IntPtr OpenHandle(string physicalPath, FileFlagsAndAttributes fileFlagsAndAttributes)
        {
            return Kernel32.CreateFile(physicalPath,
                DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
                FileShareMode.Read | FileShareMode.Write,
                IntPtr.Zero,
                FileCreationDisposition.OpenExisting,
                fileFlagsAndAttributes,
                IntPtr.Zero);
        }

        /// <summary>
        /// Opens a safe file handle with given parameters.
        /// </summary>
        /// <param name="lpFileName">The name of the file or device to be created or opened.</param>
        /// <param name="dwDesiredAccess">The requested access to the file or device.</param>
        /// <param name="dwShareMode">The requested sharing mode of the file or device.</param>
        /// <param name="lpSecurityAttributes">A pointer to a <see cref="SecurityAttributes"/> structure.</param>
        /// <param name="dwCreationDisposition">An action to take on a file or device that exists or does not exist.</param>
        /// <param name="dwFlagsAndAttributes">The file or device attributes and flags.</param>
        /// <param name="hTemplateFile">A handle to a template file with the GENERIC_READ access right.</param>
        /// <returns>A safe file handle to the opened file or device.</returns>
        public static SafeFileHandle OpenSafeFileHandle(
            string lpFileName,
            DesiredAccess dwDesiredAccess,
            FileShareMode dwShareMode,
            IntPtr lpSecurityAttributes,
            FileCreationDisposition dwCreationDisposition,
            FileFlagsAndAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            return Kernel32.CreateFileS(
                lpFileName,
                dwDesiredAccess,
                dwShareMode,
                lpSecurityAttributes,
                dwCreationDisposition,
                dwFlagsAndAttributes,
                hTemplateFile);
        }

        /// <summary>
        /// Opens a safe file handle with given parameters.
        /// </summary>
        /// <param name="lpFileName">The name of the file or device to be created or opened.</param>
        /// <param name="dwDesiredAccess">The requested access to the file or device.</param>
        /// <param name="dwShareMode">The requested sharing mode of the file or device.</param>
        /// <param name="lpSecurityAttributes">A pointer to a <see cref="SecurityAttributes"/> structure.</param>
        /// <param name="dwCreationDisposition">An action to take on a file or device that exists or does not exist.</param>
        /// <param name="dwFlagsAndAttributes">The file or device attributes and flags.</param>
        /// <param name="hTemplateFile">A handle to a template file with the GENERIC_READ access right.</param>
        /// <returns>A safe file handle to the opened file or device.</returns>
        public static SafeFileHandle OpenSafeFileHandle(
            string lpFileName,
            DesiredAccess dwDesiredAccess,
            FileShareMode dwShareMode,
            ref SecurityAttributes lpSecurityAttributes,
            FileCreationDisposition dwCreationDisposition,
            FileFlagsAndAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            return Kernel32.CreateFileS(
                lpFileName,
                dwDesiredAccess,
                dwShareMode,
                ref lpSecurityAttributes,
                dwCreationDisposition,
                dwFlagsAndAttributes,
                hTemplateFile);
        }

        /// <summary>
        /// Closes given handle.
        /// </summary>
        /// <param name="handle">Handle to close.</param>
        public static void CloseHandle(IntPtr handle)
        {
            Kernel32.CloseHandle(handle);
        }

        /// <summary>
        /// Checks if given handle is valid.
        /// </summary>
        /// <param name="handle">Handle to verify.</param>
        /// <returns>Returns boolean value to determine if handle is valid.</returns>
        public static bool IsHandleValid(IntPtr handle)
        {
            return handle != IntPtr.Zero
                && handle != Kernel32.InvalidHandle;
        }

        #endregion
    }
}
