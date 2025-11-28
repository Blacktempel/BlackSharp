/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia;

namespace BlackSharp.UI.Avalonia.Attached
{
    /// <summary>
    /// Provides an attached property for associating arbitrary user-defined data with Avalonia objects.
    /// </summary>
    /// <remarks>The attached property enables developers to store custom data on Avalonia objects without
    /// modifying their types.<br/>
    /// This is useful for scenarios where additional information needs to be associated with UI
    /// elements, such as tagging, metadata, or context-specific values.</remarks>
    public static class UserData
    {
        #region XAML Properties

        /// <summary>
        /// Identifies the Data attached property, which enables associating arbitrary user-defined data with an
        /// Avalonia object in XAML.
        /// </summary>
        public static readonly AttachedProperty<object> DataProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, object>("Data", typeof(UserData));

        #endregion

        #region Public

        /// <summary>
        /// Sets the value of the attached data property on the specified Avalonia object.
        /// </summary>
        /// <param name="element">The Avalonia object on which to set the data property. Cannot be null.</param>
        /// <param name="value">The value to assign to the data property. May be null.</param>
        public static void SetData(AvaloniaObject element, object value)
        {
            element.SetValue(DataProperty, value);
        }

        /// <summary>
        /// Retrieves the value of the attached data property from the specified Avalonia object.
        /// </summary>
        /// <param name="element">The Avalonia object from which to retrieve the data property value. Cannot be null.</param>
        /// <returns>An object representing the value of the data property associated with the specified element.</returns>
        public static object GetData(AvaloniaObject element)
        {
            return element.GetValue(DataProperty);
        }

        #endregion
    }
}
