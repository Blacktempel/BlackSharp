// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Contains source or target mode information from the Windows display configuration API.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct DisplayConfigModeInfo
    {
        #region Fields

        /// <summary>
        /// Identifies the contained mode information type.
        /// </summary>
        [FieldOffset(0)]
        public uint InfoType;

        /// <summary>
        /// Identifies the source or target on the adapter.
        /// </summary>
        [FieldOffset(4)]
        public uint ID;

        /// <summary>
        /// Identifies the display adapter.
        /// </summary>
        [FieldOffset(8)]
        public DisplayConfigLuid AdapterID;

        /// <summary>
        /// Contains target mode information when this entry describes a target.
        /// </summary>
        [FieldOffset(16)]
        public DisplayConfigTargetMode TargetMode;

        /// <summary>
        /// Contains source mode information when this entry describes a source.
        /// </summary>
        [FieldOffset(16)]
        public DisplayConfigSourceMode SourceMode;

        #endregion
    }
}
