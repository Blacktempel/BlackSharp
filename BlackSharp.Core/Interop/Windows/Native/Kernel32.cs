/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

#pragma warning disable CS1591

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;
using System.Security;

namespace BlackSharp.Core.Interop.Windows.Native
{
    public static class Kernel32
    {
        #region Fields

        const string DLL_NAME = "kernel32.dll";

        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 3;

        public static readonly IntPtr InvalidHandle = new IntPtr(-1);

        #endregion

        #region Public

        [DllImport(DLL_NAME, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport(DLL_NAME)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(DLL_NAME, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(DLL_NAME)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr module);

        [DllImport(DLL_NAME, ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr GetProcAddress(IntPtr module, string methodName);

        [DllImport(DLL_NAME)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport(DLL_NAME)]
        public static extern bool ReleaseMutex(IntPtr hMutex);

        [DllImport(DLL_NAME)]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        [DllImport(DLL_NAME)]
        public static extern IntPtr CreateMutex(ref SECURITY_ATTRIBUTES lpMutexAttributes, bool bInitialOwner, string lpName);

        [DllImport(DLL_NAME)]
        public static extern IntPtr OpenMutex(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        #endregion
    }
}

#pragma warning restore CS1591
