/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Data.Converters;
using System.Globalization;

namespace BlackSharp.UI.Avalonia.Converter
{
    /// <summary>
    /// Provides a value converter that determines whether a string value is equal to a specified parameter string.
    /// </summary>
    public class StringEqualsConverter : IValueConverter
    {
        /// <summary>
        /// Compares the specified value to the parameter and returns a boolean indicating whether they are equal as strings.
        /// </summary>
        /// <param name="value">The value to compare. If not a string, the method returns <see langword="false"/>.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">The value to compare against. If not a string, the method returns <see langword="false"/>.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>A Boolean value indicating whether <paramref name="value"/> and <paramref name="parameter"/> are equal as
        /// strings; otherwise, <see langword="false"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string v && parameter is string p)
            {
                return v == p;
            }

            return false;
        }

        /// <summary>
        /// Converts a value from the target type back to the source type in a data binding scenario.
        /// </summary>
        /// <param name="value">The value produced by the binding target to be converted.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="parameter">An optional parameter to be used in the conversion logic.</param>
        /// <param name="culture">The culture to use in the conversion.</param>
        /// <returns>The converted value to be passed to the source object.</returns>
        /// <exception cref="NotImplementedException">Always thrown, as this method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
