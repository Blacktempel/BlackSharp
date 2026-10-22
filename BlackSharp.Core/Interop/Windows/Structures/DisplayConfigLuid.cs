// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a Windows locally unique identifier used by the display configuration API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigLuid
    {
        #region Fields

        /// <summary>
        /// Contains the low-order part of the identifier.
        /// </summary>
        public uint LowPart;

        /// <summary>
        /// Contains the high-order part of the identifier.
        /// </summary>
        public int HighPart;

        #endregion
    }
}
