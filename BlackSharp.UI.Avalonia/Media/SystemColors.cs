/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia;
using Avalonia.Media;

namespace BlackSharp.UI.Avalonia.Media
{
    /// <summary>
    /// Provides methods for retrieving system-defined colors.
    /// </summary>
    public class SystemColors
    {
        #region Public

        /// <summary>
        /// Retrieves the systems default foreground brush for UI elements, if available.
        /// </summary>
        /// <returns>A <see cref="SolidColorBrush"/> representing the system foreground color, or <see langword="null"/> if the
        /// resource is not found.</returns>
        public static SolidColorBrush GetSystemForeground()
        {
            if (Application.Current.TryGetResource("SystemControlForegroundBaseHighBrush", Application.Current.RequestedThemeVariant, out var res))
            {
                return res as SolidColorBrush;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the systems default background brush for UI elements, if available.
        /// </summary>
        /// <returns>A <see cref="SolidColorBrush"/> representing the system background color, or <see langword="null"/> if the
        /// resource is not found.</returns>
        public static SolidColorBrush GetSystemBackground()
        {
            if (Application.Current.TryGetResource("SystemRegionBrush", Application.Current.RequestedThemeVariant, out var res))
            {
                return res as SolidColorBrush;
            }

            return null;
        }

        #endregion
    }
}
