// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a two-dimensional display region.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfig2DRegion
    {
        #region Fields

        /// <summary>
        /// Contains the region width.
        /// </summary>
        public uint Width;

        /// <summary>
        /// Contains the region height.
        /// </summary>
        public uint Height;

        #endregion
    }
}
