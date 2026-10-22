/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Imports Windows cryptographic data-protection functions.
    /// </summary>
    public static class Crypt32
    {
        #region Fields

        private const string DLL_NAME = "crypt32.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Protects a data blob using Windows DPAPI.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptProtectData(ref DataBlob input, string description, IntPtr entropy, IntPtr reserved, IntPtr prompt, int flags, ref DataBlob output);

        /// <summary>
        /// Unprotects a data blob using Windows DPAPI.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptUnprotectData(ref DataBlob input, out IntPtr description, IntPtr entropy, IntPtr reserved, IntPtr prompt, int flags, ref DataBlob output);

        #endregion
    }
}
