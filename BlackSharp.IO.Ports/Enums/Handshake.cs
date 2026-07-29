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
    /// Hardware/software flow-control configuration.
    /// </summary>
    /// <remarks>Values intentionally mirror System.IO.Ports.Handshake where practical.</remarks>
    public enum Handshake
    {
        /// <summary>
        /// No hardware or software flow control.
        /// </summary>
        None = 0,

        /// <summary>
        /// Software flow control using XON and XOFF characters.
        /// </summary>
        XOnXOff = 1,

        /// <summary>
        /// Hardware flow control using the Request To Send line.
        /// </summary>
        RequestToSend = 2,

        /// <summary>
        /// Hardware and software flow control.
        /// </summary>
        RequestToSendXOnXOff = 3
    }
}
