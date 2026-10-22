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
    /// Selects whether the application follows the system theme or requests a specific appearance.
    /// </summary>
    public enum ApplicationThemeMode
    {
        /// <summary>
        /// Follows the operating system's current theme.
        /// </summary>
        System = 0,

        /// <summary>
        /// Requests a light appearance.
        /// </summary>
        Light = 1,

        /// <summary>
        /// Requests a dark appearance.
        /// </summary>
        Dark = 2,
    }
}
