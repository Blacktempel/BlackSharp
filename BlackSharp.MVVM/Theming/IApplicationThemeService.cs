/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

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
