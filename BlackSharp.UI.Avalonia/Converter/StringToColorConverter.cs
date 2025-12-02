/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace BlackSharp.UI.Avalonia.Converter
{
    /// <summary>
    /// Provides a value converter that converts between color strings and <see cref="Color"/> objects for data binding scenarios.
    /// </summary>
    public class StringToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string representation of a color to a <see cref="Color"/> object.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="parameter">This parameter is not used in this implementation.</param>
        /// <param name="culture">This parameter is not used in this implementation.</param>
        /// <returns>A <see cref="Color"/> object parsed from the input string, or <see cref="Colors.Transparent"/> if the input
        /// is not a string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return Color.Parse(str);
            }

            return Colors.Transparent;
        }

        /// <summary>
        /// Converts a value from a color representation back to its string format.
        /// </summary>
        /// <param name="value">The value to convert back.</param>
        /// <param name="targetType">This parameter is not used in this implementation.</param>
        /// <param name="parameter">This parameter is not used in this implementation.</param>
        /// <param name="culture">This parameter is not used in this implementation.</param>
        /// <returns>A string representation of the color if <paramref name="value"/> is
        /// a <see cref="Color"/>; otherwise, "#000000".</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToString();
            }

            return "#000000";
        }
    }
}
