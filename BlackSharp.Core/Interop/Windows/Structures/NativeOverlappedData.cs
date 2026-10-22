// SPDX-License-Identifier: MPL-2.0

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
