// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a point used by the Windows display configuration API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigPoint
    {
        #region Fields

        /// <summary>
        /// Contains the horizontal coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// Contains the vertical coordinate.
        /// </summary>
        public int Y;

        #endregion
    }
}
