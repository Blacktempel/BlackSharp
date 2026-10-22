// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes an unmanaged byte buffer passed to Windows data-protection functions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DataBlob
    {
        #region Fields

        /// <summary>
        /// Length of the buffer in bytes.
        /// </summary>
        public int DataLength;

        /// <summary>
        /// Address of the buffer.
        /// </summary>
        public IntPtr Data;

        #endregion
    }
}
