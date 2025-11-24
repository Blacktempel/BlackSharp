/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using System.Collections;

namespace BlackSharp.Core.Extensions.Helper
{
    /// <summary>
    /// Provides a default value for the specified type parameter, including special handling for arrays and enumerable
    /// types.
    /// </summary>
    /// <remarks>For array types, the default value is an empty array instance. For enumerable types, the
    /// default value is an empty array if compatible; otherwise, a NotImplementedException is thrown. For strings and
    /// other types, the default value is the result of the default(T) expression. This class is intended for internal
    /// use and may throw exceptions for certain enumerable types that do not have a well-defined default
    /// value.</remarks>
    /// <typeparam name="T">The type for which to provide a default value.</typeparam>
    internal static class Default<T>
    {
        #region Constructor

        static Default()
        {
            if (typeof(T).IsArray)
            {
                if (typeof(T).GetArrayRank() > 1)
                    _Value = (T)(object)Array.CreateInstance(typeof(T).GetElementType(), new int[typeof(T).GetArrayRank()]);
                else
                    _Value = (T)(object)Array.CreateInstance(typeof(T).GetElementType(), 0);
                return;
            }

            if (typeof(T) == typeof(string))
            {
                //String is IEnumerable<char>, but don't want to treat it like a collection
                _Value = default(T);
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                //Check if an empty array is an instance of T
                if (typeof(T).IsAssignableFrom(typeof(object[])))
                {
                    _Value = (T)(object)new object[0];
                    return;
                }

                if (typeof(T).IsGenericType && typeof(T).GetGenericArguments().Length == 1)
                {
                    Type elementType = typeof(T).GetGenericArguments()[0];
                    if (typeof(T).IsAssignableFrom(elementType.MakeArrayType()))
                    {
                        _Value = (T)(object)Array.CreateInstance(elementType, 0);
                        return;
                    }
                }

                throw new NotImplementedException($"No default value is implemented for type '{typeof(T).FullName}'.");
            }

            _Value = default(T);
        }

        #endregion

        #region Properties

        static readonly T _Value;
        public static T Value
        {
            get
            {
                return _Value;
            }
        }

        #endregion
    }
}
