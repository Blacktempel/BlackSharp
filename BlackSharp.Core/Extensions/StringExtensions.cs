/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Extensions
{
    public static class StringExtensions
    {
        #region Public

        /// <summary>
        /// Returns a value indicating whether a specified string occurs within this string, using the specified comparison rules.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules to use in this comparison.</param>
        /// <returns><see langword="true"/> if the value parameter occurs within this string, or if value is the empty string; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Contains(this string str, string value, StringComparison comparison)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return str.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Concatenates all given elements, using the specified separator for each element, but removing the last one.
        /// </summary>
        /// <param name="separator">Separator to use to separate elements.</param>
        /// <param name="values">Values to concatenate.</param>
        /// <returns>Concatenated string.</returns>
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
            string str = string.Empty;
            foreach (var value in values)
            {
                str += value + separator;
            }
            return str?.Substring(0, str.Length - separator.Length);
        }

        /// <inheritdoc cref="Join{T}(string, IEnumerable{T})"/>
        public static string Join(string separator, params object[] values)
        {
            return Join<object>(separator, values);
        }

        #endregion
    }
}
