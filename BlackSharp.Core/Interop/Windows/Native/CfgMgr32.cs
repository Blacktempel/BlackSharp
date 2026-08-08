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
    /// Provides selected Windows Configuration Manager functions.
    /// </summary>
    public static class CfgMgr32
    {
        #region Fields

        const string DLL_NAME = "cfgmgr32.dll";

        /// <summary>
        /// Selects the hardware-identifiers registry property.
        /// </summary>
        public const uint CrpHardwareId = 0x00000002;

        /// <summary>
        /// Selects the service-name registry property.
        /// </summary>
        public const uint CrpService = 0x00000005;

        /// <summary>
        /// Selects the setup-class registry property.
        /// </summary>
        public const uint CrpClass = 0x00000008;

        #endregion

        #region Imports

        /// <summary>
        /// Retrieves the device instance identifier for a device node.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern int CM_Get_Device_ID(uint dnDevInst, [Out] char[] buffer, int bufferLen, int flags);

        /// <summary>
        /// Retrieves a registry property associated with a device node.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern int CM_Get_DevNode_Registry_Property(uint dnDevInst, uint ulProperty, out int regDataType, [Out] byte[] buffer, ref int bufferLen, int flags);

        /// <summary>
        /// Retrieves the parent of a device node.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, int flags);

        #endregion
    }
}
