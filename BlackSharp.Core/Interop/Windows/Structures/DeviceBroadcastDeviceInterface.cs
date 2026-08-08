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
    /// Describes a Windows device-interface broadcast notification filter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceBroadcastDeviceInterface
    {
        #region Fields

        /// <summary>
        /// Contains the structure size.
        /// </summary>
        public int Size;

        /// <summary>
        /// Identifies the device type.
        /// </summary>
        public int DeviceType;

        /// <summary>
        /// Contains the reserved value.
        /// </summary>
        public int Reserved;

        /// <summary>
        /// Identifies the device interface class.
        /// </summary>
        public Guid ClassGuid;

        /// <summary>
        /// Marks the beginning of the variable-length interface name.
        /// </summary>
        public short Name;

        #endregion
    }
}
