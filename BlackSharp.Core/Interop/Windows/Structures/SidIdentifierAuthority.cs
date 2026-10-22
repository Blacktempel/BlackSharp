// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Stores the six-byte authority portion of a Windows security identifier.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SidIdentifierAuthority
    {
        /// <summary>
        /// The authority value in network byte order.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
        public byte[] Value;
    }
}
