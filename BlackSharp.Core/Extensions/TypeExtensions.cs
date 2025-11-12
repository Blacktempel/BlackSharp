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

        #endregion
    }
}
