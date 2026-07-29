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
    /// Defines the configuration of a Windows serial communications device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Dcb
    {
        /// <summary>
        /// The size of this structure in bytes.
        /// </summary>
        public uint DcbLength;

        /// <summary>
        /// The serial transmission rate in bits per second.
        /// </summary>
        public uint BaudRate;

        /// <summary>
        /// Packed flags controlling the serial connection.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// A value reserved for the operating system.
        /// </summary>
        public ushort Reserved;

        /// <summary>
        /// The minimum receive-buffer occupancy before XON is sent.
        /// </summary>
        public ushort XonLim;

        /// <summary>
        /// The maximum free receive-buffer space before XOFF is sent.
        /// </summary>
        public ushort XoffLim;

        /// <summary>
        /// The number of data bits in each transmitted and received byte.
        /// </summary>
        public byte ByteSize;

        /// <summary>
        /// The parity scheme.
        /// </summary>
        public byte Parity;

        /// <summary>
        /// The number of stop bits.
        /// </summary>
        public byte StopBits;

        /// <summary>
        /// The XON flow-control character.
        /// </summary>
        public sbyte XonChar;

        /// <summary>
        /// The XOFF flow-control character.
        /// </summary>
        public sbyte XoffChar;

        /// <summary>
        /// The character used to replace bytes received with parity errors.
        /// </summary>
        public sbyte ErrorChar;

        /// <summary>
        /// The end-of-input character.
        /// </summary>
        public sbyte EofChar;

        /// <summary>
        /// The character that signals an event.
        /// </summary>
        public sbyte EvtChar;

        /// <summary>
        /// A value reserved for the operating system.
        /// </summary>
        public ushort Reserved1;
    }
}
