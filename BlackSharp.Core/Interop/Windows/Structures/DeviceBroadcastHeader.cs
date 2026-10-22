// SPDX-License-Identifier: MPL-2.0

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
