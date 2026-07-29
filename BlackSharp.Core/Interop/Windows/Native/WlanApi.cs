/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected Windows Native Wi-Fi functions.
    /// </summary>
    public static class WlanApi
    {
        #region Fields

        const string DLL_NAME = "wlanapi.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Closes a Native Wi-Fi client session.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint WlanCloseHandle(IntPtr clientHandle, IntPtr reserved);

        /// <summary>
        /// Enumerates the wireless LAN interfaces visible to a client session.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint WlanEnumInterfaces(IntPtr clientHandle, IntPtr reserved, out IntPtr interfaceList);

        /// <summary>
        /// Releases memory allocated by the Native Wi-Fi service.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern void WlanFreeMemory(IntPtr memory);

        /// <summary>
        /// Opens a Native Wi-Fi client session.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint WlanOpenHandle(uint clientVersion, IntPtr reserved, out uint negotiatedVersion, out IntPtr clientHandle);

        /// <summary>
        /// Retrieves information about a wireless LAN interface.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint WlanQueryInterface(IntPtr clientHandle, ref Guid interfaceGuid, int opcode, IntPtr reserved, out uint dataSize, out IntPtr data, out int opcodeValueType);

        #endregion
    }
}
