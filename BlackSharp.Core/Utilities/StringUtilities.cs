/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Globalization;

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

        /// <summary>
        /// Formats a nullable value with the invariant culture.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The optional value.</param>
        /// <param name="format">The format string, or <see langword="null"/> for the default format.</param>
        /// <param name="nullValue">The text returned when the value is absent.</param>
        /// <returns>The invariantly formatted value or the configured null text.</returns>
        public static string FormatNullableInvariant<T>(
            T? value,
            string format,
            string nullValue)
            where T : struct, IFormattable
        {
            return value.HasValue
                ? value.Value.ToString(format, CultureInfo.InvariantCulture)
                : nullValue ?? string.Empty;
        }

        #endregion
    }
}
