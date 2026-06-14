/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Linux.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Termios
    {
        public uint c_iflag;
        public uint c_oflag;
        public uint c_cflag;
        public uint c_lflag;
        public byte c_line;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = LinuxNativeMethods.NCCS)]
        public byte[] c_cc;

        public uint c_ispeed;
        public uint c_ospeed;
    }
}
