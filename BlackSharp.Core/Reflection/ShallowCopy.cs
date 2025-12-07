/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions;
using System.Reflection;

namespace BlackSharp.Core.Reflection
{
    /// <summary>
    /// Provides static methods for performing shallow copies of value type properties between objects of the same type.
    /// </summary>
    public static class ShallowCopy
    {
        #region Fields

        static readonly Type StringType = typeof(string);
        static readonly Type ObjectType = typeof(object);

        #endregion

        #region Public

        /// <summary>
        /// Copies all writable value type and string properties from the source object to the target object of
        /// the same type.
        /// </summary>
        /// <remarks>Only properties that are value types, strings or objects (where the assigned value
        /// is a value type) and are writable, are copied.<br/>
        /// Reference type properties other than string and object are not copied.<br/>
        /// Both source and target must be non-null and of the same type.</remarks>
        /// <typeparam name="T">The reference type whose properties will be copied.</typeparam>
        /// <param name="source">The object from which property values are copied. Cannot be null.</param>
        /// <param name="target">The object to which property values are assigned. Cannot be null.</param>
        /// <param name="bindingFlags">A bitwise combination of <see cref="BindingFlags"/> values that specifies which properties to include.</param>
        public static void CopyValueTypeProperties<T>(T source, T target, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
            where T : class
        {
            //Get all public value types
            var props = typeof(T).GetProperties(bindingFlags)
                                 .Where(pi => (pi.PropertyType.IsValueType || pi.PropertyType.AnyOf(StringType, ObjectType)) && pi.CanWrite);

            //Copy value types from source to target
            foreach (var prop in props)
            {
                var value = prop.GetValue(source);

                //If object type, check actual type of assigned value
                if (prop.PropertyType == ObjectType
                 && value?.GetType().IsValueType == false)
                {
                    continue;
                }

                prop.SetValue(target, value);
            }
        }

        #endregion
    }
}
