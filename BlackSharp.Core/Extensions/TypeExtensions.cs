/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions.Helper;
using System.Reflection;

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        #region Public

        /// <summary>
        /// Checks if <see cref="Type"/> is subclass or same <see cref="Type"/> of given <see cref="Type"/>.
        /// </summary>
        /// <param name="t">This object.</param>
        /// <param name="c">Expected type.</param>
        /// <returns>If <see cref="Type"/> is subclass or same <see cref="Type"/>.</returns>
        public static bool IsTypeOrSubclassOf(this Type t, Type c)
        {
            return t == c || t.IsSubclassOf(c);
        }

        /// <summary>
        /// Determines whether the specified type represents a floating-point numeric type.
        /// </summary>
        /// <param name="t">The type to evaluate for floating-point numeric representation. Cannot be null.</param>
        /// <returns>true if the type is float, double, or decimal; otherwise, false.</returns>
        public static bool IsFloatingType(this Type t)
        {
            return t.Any(typeof(float),
                         typeof(double),
                         typeof(decimal));
        }

        /// <summary>
        /// Gets default value for current type.
        /// </summary>
        /// <param name="t">This type.</param>
        /// <returns>Default value for type.</returns>
        public static dynamic GetDefault(this Type t)
        {
            var type = typeof(Default<>).MakeGenericType(t);
            var prop = type.GetProperty(nameof(Default<int>.Value), BindingFlags.Static | BindingFlags.Public);
            var get = prop.GetGetMethod();
            return get.Invoke(null, null);
        }

        #endregion
    }
}
