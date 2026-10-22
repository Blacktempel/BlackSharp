// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Identifies the display device information requested from Windows.
    /// </summary>
    public enum DisplayConfigDeviceInfoType
    {
        /// <summary>
        /// Requests the source device name.
        /// </summary>
        GetSourceName = 1,

        /// <summary>
        /// Requests the target device name.
        /// </summary>
        GetTargetName = 2,
    }
}
