// SPDX-License-Identifier: MPL-2.0

using Avalonia.Data.Converters;
using System.Globalization;

namespace BlackSharp.UI.Avalonia.Converter
{
    /// <summary>
    /// Provides a value converter that multiplies a double value by a specified parameter for use in data binding scenarios.
    /// </summary>
    public class DoubleMultiplyConverter : IValueConverter
    {
        #region Convert

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)Convert.ChangeType(value, typeof(double));
            var param = (double)Convert.ChangeType(parameter, typeof(double));

            ThrowIfDoubleValueIsInvalid(val);

            return val * (param == 0.0 ? 1.0 : param);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)Convert.ChangeType(value, typeof(double));
            var param = (double)Convert.ChangeType(parameter, typeof(double));

            ThrowIfDoubleValueIsInvalid(val);

            return val == 0.0 ? val : val / param;
        }

        #endregion

        #region Private API

        void ThrowIfDoubleValueIsInvalid(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Received value '{value}' is invalid.");
            }
        }

        #endregion
    }
}
