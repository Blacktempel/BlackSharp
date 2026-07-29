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
        /// <summary>
        /// No stop bits are used.
        /// </summary>
        None = 0,

        /// <summary>
        /// One stop bit is used.
        /// </summary>
        One = 1,

        /// <summary>
        /// Two stop bits are used.
        /// </summary>
        Two = 2,

        /// <summary>
        /// One and a half stop bits are used.
        /// </summary>
        OnePointFive = 3
    }
}
