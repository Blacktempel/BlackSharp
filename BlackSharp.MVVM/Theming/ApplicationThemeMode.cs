// SPDX-License-Identifier: MPL-2.0

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
