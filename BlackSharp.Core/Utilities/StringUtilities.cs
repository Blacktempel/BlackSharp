/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Extensions;
using System.Globalization;

namespace BlackSharp.Core.Utilities
{
    /// <summary>
    /// Utility class for string operations.
    /// </summary>
    public static class StringUtilities
    {
        #region Public

        /// <summary>
        /// Converts byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Hex string.</returns>
        public static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return ToHexString(bytes, 0, bytes.Length, string.Empty);
        }

        /// <summary>
        /// Converts a bounded byte-array range to a hexadecimal string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="offset">Start offset of the range.</param>
        /// <param name="length">Number of bytes to convert.</param>
        /// <param name="separator">Text inserted between encoded bytes.</param>
        /// <returns>The hexadecimal string, or an empty string when the range is unavailable.</returns>
        public static string ToHexString(byte[] bytes, int offset, int length, string separator = " ")
        {
            if (!ByteArrayExtensions.HasRange(bytes, offset, length))
            {
                return string.Empty;
            }

            separator ??= string.Empty;

            var chars = new char[length * 2 + Math.Max(0, length - 1) * separator.Length];
            var position = 0;

            for (var index = 0; index < length; ++index)
            {
                if (index > 0)
                {
                    separator.CopyTo(0, chars, position, separator.Length);
                    position += separator.Length;
                }

                var value = bytes[offset + index];

                chars[position++] = GetHexChar(value >> 4);
                chars[position++] = GetHexChar(value & 0x0F);
            }

            return new string(chars);
        }

        /// <summary>
        /// Decodes and normalizes an ASCII byte array.
        /// </summary>
        /// <param name="value">The encoded byte array.</param>
        /// <returns>The decoded and normalized string.</returns>
        public static string DecodeASCII(byte[] value)
        {
            return value == null ? string.Empty : DecodeASCII(value, 0, value.Length);
        }

        /// <summary>
        /// Decodes and normalizes a bounded ASCII byte-array range.
        /// </summary>
        /// <param name="value">The encoded byte array.</param>
        /// <param name="offset">The first byte to decode.</param>
        /// <param name="length">The number of bytes to decode.</param>
        /// <returns>The decoded and normalized string, or an empty string when the range is unavailable.</returns>
        public static string DecodeASCII(byte[] value, int offset, int length)
        {
            return ByteArrayExtensions.HasRange(value, offset, length)
                 ? TrimNullPadding(System.Text.Encoding.ASCII.GetString(value, offset, length))
                 : string.Empty;
        }

        /// <summary>
        /// Returns the first normalized string that is not empty or whitespace.
        /// </summary>
        /// <param name="values">The candidate strings in priority order.</param>
        /// <returns>The first normalized value or an empty string.</returns>
        public static string FirstNonEmpty(params string[] values)
        {
            if (values == null)
            {
                return string.Empty;
            }

            foreach (var value in values)
            {
                var candidate = TrimNullPadding(value);

                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
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
