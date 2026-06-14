/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Utilities
{
    /// <summary>
    /// Utility class for string operations.
    /// </summary>
    public class StringUtilities
    {
        #region Public

        /// <summary>
        /// Converts byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Hex string.</returns>
        public static string ToHexString(byte[] bytes)
        {
            var chars = new char[bytes.Length * 2];

            for (var i = 0; i < bytes.Length; i++)
            {
                var value = bytes[i];
                chars[i * 2] = GetHexChar(value >> 4);
                chars[i * 2 + 1] = GetHexChar(value & 0x0F);
            }

            return new string(chars);
        }

        /// <summary>
        /// Converts a value to its corresponding hex character.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Hex character.</returns>
        public static char GetHexChar(int value)
        {
            return (char)(value < 10 ? '0' + value : 'A' + value - 10);
        }

        #endregion
    }
}
