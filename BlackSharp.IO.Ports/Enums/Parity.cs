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
    /// Parity mode for the serial line.
    /// </summary>
    /// <remarks>Values intentionally mirror System.IO.Ports.Parity where practical.</remarks>
    public enum Parity
    {
        /// <summary>
        /// No parity bit is used.
        /// </summary>
        None = 0,

        /// <summary>
        /// The parity bit makes the count of set bits odd.
        /// </summary>
        Odd = 1,

        /// <summary>
        /// The parity bit makes the count of set bits even.
        /// </summary>
        Even = 2,

        /// <summary>
        /// The parity bit is always set.
        /// </summary>
        Mark = 3,

        /// <summary>
        /// The parity bit is always cleared.
        /// </summary>
        Space = 4
    }
}
