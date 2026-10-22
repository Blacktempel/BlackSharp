// SPDX-License-Identifier: MPL-2.0

using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace BlackSharp.MVVM.ComponentModel
{
    /// <summary>
    /// <inheritdoc cref="ObservableObject"/>
    /// </summary>
    public class ViewModelBase : ObservableObject
    {
        #region Public

        /// <inheritdoc cref="ObservableObject.SetProperty{T}(ref T, T, string)"/>
        public bool SetField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(ref field, newValue, propertyName);
        }

        /// <summary>
        /// Raises property change notifications for all properties on the object.
        /// </summary>
        public void NotifyAllPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        #endregion
    }
}
