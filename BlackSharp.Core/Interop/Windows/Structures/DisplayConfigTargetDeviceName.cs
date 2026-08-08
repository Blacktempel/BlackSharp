/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Contains the friendly name and device path of a Windows display target.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DisplayConfigTargetDeviceName
    {
        #region Fields

        /// <summary>
        /// Contains the request header.
        /// </summary>
        public DisplayConfigDeviceInfoHeader Header;

        /// <summary>
        /// Contains target name flags.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Identifies the video output technology.
        /// </summary>
        public uint OutputTechnology;

        /// <summary>
        /// Contains the EDID manufacturer identifier.
        /// </summary>
        public ushort EdidManufactureID;

        /// <summary>
        /// Contains the EDID product code.
        /// </summary>
        public ushort EdidProductCodeID;

        /// <summary>
        /// Identifies the connector instance.
        /// </summary>
        public uint ConnectorInstance;

        /// <summary>
        /// Contains the monitor-friendly device name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string MonitorFriendlyDeviceName;

        /// <summary>
        /// Contains the monitor device path.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string MonitorDevicePath;

        #endregion
    }
}
