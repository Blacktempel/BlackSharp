// SPDX-License-Identifier: MPL-2.0

using Avalonia;
using Avalonia.Styling;
using BlackSharp.MVVM.Theming;

namespace BlackSharp.UI.Avalonia.Services
{
    /// <summary>
    /// Applies the requested theme variant to the current Avalonia application.
    /// </summary>
    public sealed class AvaloniaThemeService : IApplicationThemeService
    {
        #region Fields

        /// <summary>
        /// Stores the most recently requested mode, even before an application is available.
        /// </summary>
        private ApplicationThemeMode _themeMode = ApplicationThemeMode.System;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public ApplicationThemeMode ThemeMode => _themeMode;

        #endregion

        #region Public

        /// <inheritdoc/>
        public void SetThemeMode(ApplicationThemeMode themeMode)
        {
            _themeMode = themeMode;

            if (Application.Current == null)
            {
                return;
            }

            Application.Current.RequestedThemeVariant = themeMode switch
            {
                ApplicationThemeMode.Light => ThemeVariant.Light,
                ApplicationThemeMode.Dark  => ThemeVariant.Dark,
                _                          => ThemeVariant.Default,
            };
        }

        #endregion
    }
}
