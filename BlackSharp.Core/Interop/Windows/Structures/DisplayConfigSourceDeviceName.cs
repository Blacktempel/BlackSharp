// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Contains the GDI device name of a Windows display source.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DisplayConfigSourceDeviceName
    {
        #region Fields

        /// <summary>
        /// Contains the request header.
        /// </summary>
        public DisplayConfigDeviceInfoHeader Header;

        /// <summary>
        /// Contains the GDI device name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string ViewGdiDeviceName;

        #endregion
    }
}
