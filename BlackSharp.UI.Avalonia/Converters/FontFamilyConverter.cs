// SPDX-License-Identifier: MPL-2.0

using Avalonia.Data.Converters;
using Avalonia.Media;
using BlackSharp.UI.Avalonia.Global;
using System.Globalization;

namespace BlackSharp.UI.Avalonia.Converters
{
    /// <summary>
    /// Converts between installed Avalonia font families and their names.
    /// </summary>
    public sealed class FontFamilyConverter : IValueConverter
    {
        #region Public

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                FontFamily family => family,
                string name       => GlobalFontSettings.ResolveFontFamily(name),
                _                 => FontManager.Current.DefaultFontFamily,
            };
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                FontFamily family => family.Name,
                string name       => name,
                _                 => string.Empty,
            };
        }

        #endregion
    }
}
