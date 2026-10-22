// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes the mode of a Windows display source.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigSourceMode
    {
        #region Fields

        /// <summary>
        /// Contains the source width.
        /// </summary>
        public uint Width;

        /// <summary>
        /// Contains the source height.
        /// </summary>
        public uint Height;

        /// <summary>
        /// Identifies the pixel format.
        /// </summary>
        public uint PixelFormat;

        /// <summary>
        /// Contains the source position.
        /// </summary>
        public DisplayConfigPoint Position;

        #endregion
    }
}
