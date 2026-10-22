/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using Avalonia.Media;
using BlackSharp.MVVM.ComponentModel;

namespace BlackSharp.UI.Avalonia.Global
{
    /// <summary>
    /// Supplies observable font settings for applications that enable the global font themes.
    /// </summary>
    public sealed class GlobalFontSettings : ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Initializes the shared font settings.
        /// </summary>
        private GlobalFontSettings()
        {
        }

        #endregion

        #region Fields

        /// <summary>
        /// Stores the selected font family.
        /// </summary>
        private FontFamily _fontFamily = FontManager.Current.DefaultFontFamily;

        /// <summary>
        /// Stores the selected font size in device-independent pixels.
        /// </summary>
        private double _fontSize = 12.0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the shared font settings instance.
        /// </summary>
        public static GlobalFontSettings Current { get; } = new();

        /// <summary>
        /// Gets or sets the font family applied by the optional global font themes.
        /// </summary>
        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => SetField(ref _fontFamily, value ?? FontManager.Current.DefaultFontFamily);
        }

        /// <summary>
        /// Gets or sets the font size applied by the optional global font themes.
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            set => SetField(ref _fontSize, value > 0.0 ? value : 12.0);
        }

        #endregion

        #region Public

        /// <summary>
        /// Resolves an installed font family by name or returns the system default.
        /// </summary>
        /// <param name="name">The installed family name.</param>
        /// <returns>The matching family or the system default.</returns>
        public static FontFamily ResolveFontFamily(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return FontManager.Current.DefaultFontFamily;
            }

            return FontManager.Current.SystemFonts.FirstOrDefault(
                font => string.Equals(font.Name, name, StringComparison.OrdinalIgnoreCase),
                FontManager.Current.DefaultFontFamily);
        }

        /// <summary>
        /// Returns the installed font-family names in display order.
        /// </summary>
        /// <returns>The installed family names.</returns>
        public static IReadOnlyList<string> GetSystemFontFamilies()
        {
            return FontManager.Current.SystemFonts
                .Select(font => font.Name)
                .OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }

        #endregion
    }
}
