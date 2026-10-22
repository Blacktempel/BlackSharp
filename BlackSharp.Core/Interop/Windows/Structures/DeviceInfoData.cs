// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Identifies a device information element managed by the Windows Setup API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInfoData
    {
        /// <summary>
        /// The size of this structure in bytes.
        /// </summary>
        public int Size;

        /// <summary>
        /// The setup class identifier of the device.
        /// </summary>
        public Guid ClassGuid;

        /// <summary>
        /// The configuration-manager device instance handle.
        /// </summary>
        public uint DeviceInstance;

        /// <summary>
        /// A value reserved for the operating system.
        /// </summary>
        public IntPtr Reserved;
    }
}
