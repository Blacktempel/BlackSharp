// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Reports the status and queue sizes of a Windows communications device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ComStat
    {
        /// <summary>
        /// The packed communications-status flags.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// The number of bytes waiting in the receive queue.
        /// </summary>
        public uint BytesInQueue;

        /// <summary>
        /// The number of bytes waiting in the transmit queue.
        /// </summary>
        public uint BytesOutQueue;
    }
}
