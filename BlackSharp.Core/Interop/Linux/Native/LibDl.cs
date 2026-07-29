/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Native
{
    /// <summary>
    /// Provides access to the Linux dynamic-linker functions.
    /// </summary>
    public static class LibDl
    {
        #region Fields

        const string LIBC_LIBRARY_NAME  = "libc";
        const string LIBDL_LIBRARY_NAME = "libdl.so.2";

        #endregion

        #region Imports

        /// <summary>
        /// Releases a dynamic-library handle through libdl.
        /// </summary>
        [DllImport(LIBDL_LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int dlclose(IntPtr handle);

        /// <summary>
        /// Releases a dynamic-library handle through libc.
        /// </summary>
        [DllImport(LIBC_LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlclose")]
        public static extern int dlcloseLibC(IntPtr handle);

        /// <summary>
        /// Opens a dynamic library through libdl.
        /// </summary>
        [DllImport(LIBDL_LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string fileName, int flags);

        /// <summary>
        /// Opens a dynamic library through libc.
        /// </summary>
        [DllImport(LIBC_LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlopen")]
        public static extern IntPtr dlopenLibC([MarshalAs(UnmanagedType.LPStr)] string fileName, int flags);

        /// <summary>
        /// Resolves a symbol through libdl.
        /// </summary>
        [DllImport(LIBDL_LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string symbol);

        /// <summary>
        /// Resolves a symbol through libc.
        /// </summary>
        [DllImport(LIBC_LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "dlsym")]
        public static extern IntPtr dlsymLibC(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string symbol);

        #endregion
    }
}
