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
        /// Formats an unsigned byte as a prefixed hexadecimal value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The prefixed hexadecimal value.</returns>
        public static string FormatHex(byte value)
        {
            return FormatHex(value, 1);
        }

        /// <summary>
        /// Formats an unsigned 16-bit integer as a prefixed hexadecimal value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The prefixed hexadecimal value.</returns>
        public static string FormatHex(ushort value)
        {
            return FormatHex(value, 2);
        }

        /// <summary>
        /// Formats an unsigned 32-bit integer as a prefixed hexadecimal value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The prefixed hexadecimal value.</returns>
        public static string FormatHex(uint value)
        {
            return FormatHex(value, 4);
        }

        /// <summary>
        /// Formats an unsigned integer as a prefixed hexadecimal value with a minimum width.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="minimumByteCount">The minimum number of bytes represented by the result.</param>
        /// <returns>The prefixed hexadecimal value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minimumByteCount"/> is outside the range from one to eight.
        /// </exception>
        public static string FormatHex(ulong value, int minimumByteCount)
        {
            if (minimumByteCount < 1 || minimumByteCount > sizeof(ulong))
            {
                throw new ArgumentOutOfRangeException(nameof(minimumByteCount));
            }

            var format = "X" + (minimumByteCount * 2).ToString(CultureInfo.InvariantCulture);

            return "0x" + value.ToString(format, CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Replaces null and empty strings with their configured display values.
        /// </summary>
        /// <param name="value">The text to format.</param>
        /// <param name="nullValue">The text returned when <paramref name="value"/> is null.</param>
        /// <param name="emptyValue">The text returned when <paramref name="value"/> is empty.</param>
        /// <returns>The original text or the matching configured display value.</returns>
        public static string FormatNullOrEmpty(string value, string nullValue, string emptyValue)
        {
            if (value == null)
            {
                return nullValue ?? string.Empty;
            }

            return value.Length == 0
                 ? emptyValue ?? string.Empty
                 : value;
        }

        /// <summary>
        /// Splits text into lines independent of the source line-ending convention.
        /// </summary>
        /// <param name="value">The text to split.</param>
        /// <returns>The lines, including empty lines.</returns>
        public static string[] SplitLines(string value)
        {
            return (value ?? string.Empty)
                   .Replace("\r\n", "\n")
                   .Replace('\r', '\n')
                   .Split(new[] { '\n' }, StringSplitOptions.None);
        }

        /// <summary>
        /// Removes surrounding whitespace and null padding from text.
        /// </summary>
        /// <param name="value">The text to normalize.</param>
        /// <returns>The normalized text, or an empty string when <paramref name="value"/> is null.</returns>
        public static string TrimNullPadding(string value)
        {
            return (value ?? string.Empty)
                   .Trim()
                   .Trim('\0')
                   .Trim();
        }

        #endregion
    }
}
