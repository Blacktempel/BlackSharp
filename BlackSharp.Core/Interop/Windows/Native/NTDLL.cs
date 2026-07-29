/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;
using System.Security;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected native Windows system functions.
    /// </summary>
    public static class NTDLL
    {
        #region Fields

        const string DLL_NAME = "ntdll.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Retrieves the requested class of system information.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int NtQuerySystemInformation(int systemInformationClass, IntPtr systemInformation, uint systemInformationLength, out uint returnLength);

        /// <summary>
        /// Retrieves the operating-system version from the native runtime library.
        /// </summary>
        [SecurityCritical]
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern uint RtlGetVersion(ref OsVersionInfoEx versionInfo);

        #endregion
    }
}
