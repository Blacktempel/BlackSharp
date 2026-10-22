// SPDX-License-Identifier: MPL-2.0

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
