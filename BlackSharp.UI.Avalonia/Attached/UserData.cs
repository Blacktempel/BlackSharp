// SPDX-License-Identifier: MPL-2.0

using Avalonia;

namespace BlackSharp.UI.Avalonia.Attached
{
    /// <summary>
    /// Provides an attached property for associating arbitrary user-defined data with Avalonia objects.
    /// </summary>
    /// <remarks>The attached property enables developers to associate custom data with Avalonia objects.<br/>
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
