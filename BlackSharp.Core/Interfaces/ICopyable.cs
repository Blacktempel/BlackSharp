/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a method to copy the state from another instance of the same type.
    /// </summary>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Copies the values from the specified object to the current instance.
        /// </summary>
        /// <remarks>This shall copy value types only, not copying reference types at all.</remarks>
        void Copy(T from);
    }
}
