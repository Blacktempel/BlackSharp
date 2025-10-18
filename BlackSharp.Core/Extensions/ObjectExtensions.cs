/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using System.Collections;

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Checks if the current object matches (Equals) any of the specified objects.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="value1">An object to check for equality.</param>
        /// <param name="value2">An object to check for equality.</param>
        /// <returns>If any specified object is equal to the current object.</returns>
        public static bool Any<T>(this T obj, T value1, T value2)
        {
            return obj.Equals(value1) || obj.Equals(value2);
        }

        /// <summary>
        /// Checks if the current object matches (Equals) any of the specified objects.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="value1">An object to check for equality.</param>
        /// <param name="value2">An object to check for equality.</param>
        /// <param name="values">A variable list of objects to check for equality.</param>
        /// <returns>If any specified object is equal to the current object.</returns>
        public static bool Any<T>(this T obj, T value1, T value2, params T[] values)
        {
            if (Any(obj, value1, value2))
                return true;
            foreach (var o in values)
                if (obj.Equals(o))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if the current object matches (Equals) any of the objects in specified <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="collection">A collection of objects to check for equality.</param>
        /// <returns>If any object in <see cref="IEnumerable"/> is equal to the current object.</returns>
        public static bool Any<T>(this T obj, IEnumerable<T> collection)
        {
            foreach (var o in collection)
                if (obj.Equals(o))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if the current object matches (Equals) any of the specified objects.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="value1">An object to check for equality.</param>
        /// <param name="value2">An object to check for equality.</param>
        /// <returns>If any specified object is equal to the current object.</returns>
        public static bool AnyOf(this object obj, object value1, object value2)
        {
            return obj.Equals(value1) || obj.Equals(value2);
        }

        /// <summary>
        /// Checks if the current object matches (Equals) any of the specified objects.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="value1">An object to check for equality.</param>
        /// <param name="value2">An object to check for equality.</param>
        /// <param name="values">A variable list of objects to check for equality.</param>
        /// <returns>If any specified object is equal to the current object.</returns>
        public static bool AnyOf(this object obj, object value1, object value2, params object[] values)
        {
            if (AnyOf(obj, value1, value2))
                return true;
            foreach (var o in values)
                if (obj.Equals(o))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if the current object matches (Equals) any of the objects in specified <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <param name="collection">A collection of objects to check for equality.</param>
        /// <returns>If any object in <see cref="IEnumerable"/> is equal to the current object.</returns>
        public static bool AnyOf(this object obj, IEnumerable collection)
        {
            foreach (var o in collection)
                if (obj.Equals(o))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if object is of a number-type.
        /// </summary>
        /// <param name="obj">The current object.</param>
        /// <returns>If the object is of a number type.</returns>
        public static bool IsNumber(this object obj)
        {
            return obj is byte ||
                    obj is short ||
                    obj is int ||
                    obj is long ||
                    obj is float ||
                    obj is double ||
                    obj is decimal ||
                    obj is sbyte ||
                    obj is ushort ||
                    obj is uint ||
                    obj is ulong;
        }
    }
}
