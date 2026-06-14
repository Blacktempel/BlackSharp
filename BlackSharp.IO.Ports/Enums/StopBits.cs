/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.Ports
{
    /// <summary>
    /// Stop bit configuration for the serial line.
    /// </summary>
    /// <remarks>Values intentionally mirror System.IO.Ports.StopBits where practical.</remarks>
    public enum StopBits
    {
        None = 0,
        One = 1,
        Two = 2,
        OnePointFive = 3
    }
}
