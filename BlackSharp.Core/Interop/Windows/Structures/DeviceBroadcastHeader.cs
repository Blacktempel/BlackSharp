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
    /// Represents the common header of a Windows device broadcast notification.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceBroadcastHeader
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

        #endregion
    }
}
