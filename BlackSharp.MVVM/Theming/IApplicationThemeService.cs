// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.MVVM.Theming
{
    /// <summary>
    /// Defines a service for changing the application's theme and appearance.
    /// </summary>
    public interface IApplicationThemeService
    {
        #region Properties

        /// <summary>
        /// Gets the requested application theme mode.
        /// </summary>
        ApplicationThemeMode ThemeMode { get; }

        #endregion

        #region Public

        /// <summary>
        /// Updates the requested application theme mode.
        /// </summary>
        /// <param name="themeMode">The mode to apply.</param>
        void SetThemeMode(ApplicationThemeMode themeMode);

        #endregion
    }
}
