/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures;

/// <summary>
/// Contains state used by an asynchronous Windows I/O operation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NativeOverlappedData
{
    /// <summary>
    /// The operation status maintained by the operating system.
    /// </summary>
    public IntPtr Internal;

    /// <summary>
    /// The number of bytes transferred, maintained by the operating system.
    /// </summary>
    public IntPtr InternalHigh;

    /// <summary>
    /// The low-order part of the file position.
    /// </summary>
    public uint Offset;

    /// <summary>
    /// The high-order part of the file position.
    /// </summary>
    public uint OffsetHigh;

    /// <summary>
    /// The event signaled when the operation completes.
    /// </summary>
    public IntPtr EventHandle;
}
