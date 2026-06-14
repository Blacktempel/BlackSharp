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
    internal struct PollFd
    {
        public int fd;
        public short events;
        public short revents;
    }
}
