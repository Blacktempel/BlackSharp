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
    /// Hardware/software flow-control configuration.
    /// </summary>
    /// <remarks>Values intentionally mirror System.IO.Ports.Handshake where practical.</remarks>
    public enum Handshake
    {
        None = 0,
        XOnXOff = 1,
        RequestToSend = 2,
        RequestToSendXOnXOff = 3
    }
}
