/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

namespace BlackSharp.IO.Ports
{
    /// <summary>
    /// Parity mode for the serial line.
    /// </summary>
    /// <remarks>Values intentionally mirror System.IO.Ports.Parity where practical.</remarks>
    public enum Parity
    {
        None = 0,
        Odd = 1,
        Even = 2,
        Mark = 3,
        Space = 4
    }
}
