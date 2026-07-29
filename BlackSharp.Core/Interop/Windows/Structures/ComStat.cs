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
