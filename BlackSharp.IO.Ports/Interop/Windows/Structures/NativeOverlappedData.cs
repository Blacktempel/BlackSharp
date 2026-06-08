/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Windows.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct NativeOverlappedData
{
    public IntPtr Internal;
    public IntPtr InternalHigh;
    public uint Offset;
    public uint OffsetHigh;
    public IntPtr hEvent;
}
