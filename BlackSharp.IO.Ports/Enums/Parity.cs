// SPDX-License-Identifier: MPL-2.0

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
