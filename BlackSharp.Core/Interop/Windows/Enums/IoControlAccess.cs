/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

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
