/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Interfaces;

namespace BlackSharp.MVVM.ComponentModel
{
    /// <summary>
    /// Provides a base class for view models that support drag-and-drop operations.
    /// </summary>
    public abstract class DragDropViewModelBase : ViewModelBase, ICloneable<DragDropViewModelBase>
    {
        /// <summary>
        /// <inheritdoc cref="ICloneable{T}"/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public abstract DragDropViewModelBase Clone();
    }
}
