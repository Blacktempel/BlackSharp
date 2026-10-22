// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a rational number used by the display configuration API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigRational
    {
        #region Fields

        /// <summary>
        /// Contains the numerator.
        /// </summary>
        public uint Numerator;

        /// <summary>
        /// Contains the denominator.
        /// </summary>
        public uint Denominator;

        #endregion
    }
}
