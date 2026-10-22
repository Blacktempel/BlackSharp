// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a native Windows point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
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
