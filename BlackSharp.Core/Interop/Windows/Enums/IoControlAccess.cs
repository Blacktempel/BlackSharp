// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Interop.Windows.Enums
{
    /// <summary>
    /// Defines the access rights encoded in a Windows device-control code.
    /// </summary>
    [Flags]
    public enum IoControlAccess : uint
    {
        /// <summary>No access validation is required.</summary>
        None  = 0x0000,
        /// <summary>Read access to the device is required.</summary>
        Read  = 0x0001,
        /// <summary>Write access to the device is required.</summary>
        Write = 0x0002,
    }
}
