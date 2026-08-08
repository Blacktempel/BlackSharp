/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

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
