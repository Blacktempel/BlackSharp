// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes a device interface returned by the Windows Setup API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInterfaceData
    {
        /// <summary>
        /// The size of this structure in bytes.
        /// </summary>
        public int Size;

        /// <summary>
        /// The device interface class identifier.
        /// </summary>
        public Guid InterfaceClassGuid;

        /// <summary>
        /// Flags describing the state of the device interface.
        /// </summary>
        public int Flags;

        /// <summary>
        /// A value reserved for the operating system.
        /// </summary>
        public IntPtr Reserved;
    }
}
