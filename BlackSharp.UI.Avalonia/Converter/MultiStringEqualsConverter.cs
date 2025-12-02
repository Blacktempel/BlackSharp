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
    /// Provides a value converter that determines whether two input strings are equal.
    /// </summary>
    public class MultiStringEqualsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Compares two string values in the provided list and returns a boolean indicating whether they are equal.
        /// </summary>
        /// <param name="values">A list containing the values to compare.<br/>
        /// Must contain exactly two elements, both of which should be strings for a valid comparison.</param>
        /// <param name="targetType">This parameter is not used in the comparison.</param>
        /// <param name="parameter">This parameter is not used in the comparison.</param>
        /// <param name="culture">This parameter is not used in the comparison.</param>
        /// <returns>A Boolean value indicating whether the two string values in the list are equal.<br/>
        /// Returns <see langword="false"/> if the list does not contain exactly two strings.</returns>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count == 2)
            {
                if (values[0] is string str1 && values[1] is string str2)
                {
                    return str1 == str2;
                }
            }

            return false;
        }
    }
}
