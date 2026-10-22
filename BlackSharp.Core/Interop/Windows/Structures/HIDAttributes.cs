// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures;

/// <summary>
/// Contains the identifiers reported by a Windows HID interface.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct HIDAttributes
{
    /// <summary>The size of this structure in bytes.</summary>
    public int Size;

    /// <summary>The USB vendor identifier.</summary>
    public ushort VendorID;

    /// <summary>The USB product identifier.</summary>
    public ushort ProductID;

    /// <summary>The binary-coded device release number.</summary>
    public ushort VersionNumber;
}
