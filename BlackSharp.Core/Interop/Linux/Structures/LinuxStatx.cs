// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Structures
{
    /// <summary>
    /// Exposes the ownership and mode fields of the fixed-size Linux statx ABI.
    /// </summary>
    /// <remarks>
    /// The complete native buffer occupies 256 bytes. Only fields required for security validation are represented.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 256)]
    public struct LinuxStatx
    {
        #region Fields

        /// <summary>
        /// Identifies which metadata fields were supplied by the filesystem.
        /// </summary>
        [FieldOffset(0)]
        public uint Mask;

        /// <summary>
        /// The numeric identifier of the file owner.
        /// </summary>
        [FieldOffset(20)]
        public uint UserID;

        /// <summary>
        /// The file-type and Unix permission bits.
        /// </summary>
        [FieldOffset(28)]
        public ushort Mode;

        #endregion
    }
}
