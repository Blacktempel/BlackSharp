// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures;

/// <summary>
/// Identifies a file whose signature is evaluated by the Windows trust provider.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct WinTrustFileInfo
{
    /// <summary>
    /// The size of this structure in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// The fully qualified path of the file to verify.
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string FilePath;

    /// <summary>
    /// An optional handle to the open file.
    /// </summary>
    public IntPtr FileHandle;

    /// <summary>
    /// An optional pointer to the subject type identifier.
    /// </summary>
    public IntPtr KnownSubject;
}
