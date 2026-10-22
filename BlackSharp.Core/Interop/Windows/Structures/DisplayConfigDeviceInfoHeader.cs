// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Provides the header shared by Windows display device information requests.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigDeviceInfoHeader
    {
        #region Fields

        /// <summary>
        /// Identifies the requested information type.
        /// </summary>
        public DisplayConfigDeviceInfoType Type;

        /// <summary>
        /// Contains the size of the complete request structure.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Identifies the display adapter.
        /// </summary>
        public DisplayConfigLuid AdapterID;

        /// <summary>
        /// Identifies the source or target on the adapter.
        /// </summary>
        public uint ID;

        #endregion
    }
}
