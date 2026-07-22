/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        #region Public

        /// <summary>
        /// Returns a value indicating whether a specified string occurs within this string, using the specified
        /// comparison rules.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="comparison">One of the values that specifies the rules to use in this comparison.</param>
        /// <returns>
        /// <see langword="true"/> if the value parameter occurs within this string, or if value is the empty string;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Contains(this string str, string value, StringComparison comparison)
        {
            if (str == null || value == null)
            {
                return false;
            }

            return str.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Returns whether this string starts with any of the specified values using the specified comparison rules.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="comparison">One of the values that specifies the rules to use in this comparison.</param>
        /// <param name="values">The strings to compare.</param>
        /// <returns><see langword="true"/> if this string starts with any specified value.</returns>
        public static bool StartsWithAny(this string str, StringComparison comparison, params string[] values)
        {
            if (str == null || values == null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (value != null && str.StartsWith(value, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether this string contains any non-whitespace specified value using the specified
        /// comparison rules.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="comparison">One of the values that specifies the rules to use in this comparison.</param>
        /// <param name="values">The strings to seek.</param>
        /// <returns><see langword="true"/> if any specified value occurs within this string.</returns>
        public static bool ContainsAny(this string str, StringComparison comparison, params string[] values)
        {
            if (string.IsNullOrWhiteSpace(str) || values == null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value) && str.Contains(value, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the first specified string that is not <see langword="null"/>, empty, or whitespace.
        /// </summary>
        /// <param name="values">The strings to inspect in order.</param>
        /// <returns>The first matching string, or <see cref="string.Empty"/> if no string matches.</returns>
        public static string FirstNotNullOrWhiteSpace(params string[] values)
        {
            if (values == null)
            {
                return string.Empty;
            }

            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }

        /// <summary>
        /// Concatenates all given elements, using the specified separator between elements.
        /// </summary>
        /// <param name="separator">Separator to use to separate elements.</param>
        /// <param name="values">Values to concatenate.</param>
        /// <returns>Concatenated string.</returns>
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
            return string.Join(separator, values);
        }

        /// <inheritdoc cref="Join{T}(string, IEnumerable{T})"/>
        public static string Join(string separator, params object[] values)
        {
            return Join<object>(separator, values);
        }

        /// <summary>
        /// Replaces a substring (range) in current string with specified string.
        /// </summary>
        /// <param name="str">This object.</param>
        /// <param name="startIndex">
        /// The zero-based starting character position of the substring in this instance.
        /// </param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="instead">The string to replace the specified substring (range).</param>
        /// <returns>The new string.</returns>
        public static string ReplaceAtIndex(this string str, int startIndex, int length, string instead)
        {
            string temp0 = str.Substring(0, startIndex);
            int index = startIndex + length;
            string temp2 = str.Substring(index, str.Length - index);
            return temp0 + instead + temp2;
        }

        #endregion
    }
}
