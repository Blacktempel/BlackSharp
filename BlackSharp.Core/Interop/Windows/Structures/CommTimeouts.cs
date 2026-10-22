// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Defines read and write timeout parameters for a Windows communications device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CommTimeouts
    {
        /// <summary>
        /// The maximum interval between incoming bytes.
        /// </summary>
        public uint ReadIntervalTimeout;

        /// <summary>
        /// The per-byte multiplier used to calculate a read timeout.
        /// </summary>
        public uint ReadTotalTimeoutMultiplier;

        /// <summary>
        /// The constant added when calculating a read timeout.
        /// </summary>
        public uint ReadTotalTimeoutConstant;

        /// <summary>
        /// The per-byte multiplier used to calculate a write timeout.
        /// </summary>
        public uint WriteTotalTimeoutMultiplier;

        /// <summary>
        /// The constant added when calculating a write timeout.
        /// </summary>
        public uint WriteTotalTimeoutConstant;
    }
}
