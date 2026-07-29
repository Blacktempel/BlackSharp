/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Structures
{
    /// <summary>
    /// Stores POSIX terminal input, output, control, and line-discipline settings.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Termios
    {
        /// <summary>
        /// The terminal input-mode flags.
        /// </summary>
        public uint InputFlags;

        /// <summary>
        /// The terminal output-mode flags.
        /// </summary>
        public uint OutputFlags;

        /// <summary>
        /// The terminal control-mode flags.
        /// </summary>
        public uint ControlFlags;

        /// <summary>
        /// The terminal local-mode flags.
        /// </summary>
        public uint LocalFlags;

        /// <summary>
        /// The active line discipline.
        /// </summary>
        public byte LineDiscipline;

        /// <summary>
        /// The terminal control-character array.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ControlCharacters;

        /// <summary>
        /// The encoded input baud rate.
        /// </summary>
        public uint InputSpeed;

        /// <summary>
        /// The encoded output baud rate.
        /// </summary>
        public uint OutputSpeed;
    }
}
