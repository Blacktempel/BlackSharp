/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Provides operations for general-purpose collections.
    /// </summary>
    public static class CollectionExtensions
    {
        #region Public

        /// <summary>
        /// Finds the first element that is the same object instance as the specified value.
        /// </summary>
        /// <typeparam name="T">The reference type stored by the collection.</typeparam>
        /// <param name="values">The collection to search.</param>
        /// <param name="value">The object instance to find.</param>
        /// <returns>The zero-based index of the matching instance, or <c>-1</c> when it is absent.</returns>
        public static int IndexOfReference<T>(this IReadOnlyList<T> values, T value)
            where T : class
        {
            if (values == null || value == null)
            {
                return -1;
            }

            for (var index = 0; index < values.Count; ++index)
            {
                if (ReferenceEquals(values[index], value))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the first element that satisfies a predicate.
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="values">The collection to search.</param>
        /// <param name="predicate">The predicate that identifies a matching item.</param>
        /// <param name="value">The first matching value, or the default value when no item matches.</param>
        /// <returns><see langword="true"/> when a matching item was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetFirst<T>(
            this IEnumerable<T> values,
            Func<T, bool> predicate,
            out T value)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (T candidate in values)
            {
                if (predicate(candidate))
                {
                    value = candidate;
                    return true;
                }
            }

            value = default;

            return false;
        }

        #endregion
    }
}
