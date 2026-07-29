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
    /// Defines how Windows transfers buffers for a device-control request.
    /// </summary>
    public enum IoControlMethod : uint
    {
        /// <summary>Uses a system buffer for input and output data.</summary>
        Buffered  = 0,
        /// <summary>Uses a direct input buffer described by an MDL.</summary>
        InDirect  = 1,
        /// <summary>Uses a direct output buffer described by an MDL.</summary>
        OutDirect = 2,
        /// <summary>Passes user-mode buffer addresses directly to the driver.</summary>
        Neither   = 3,
    }
}
