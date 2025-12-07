/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.UI.Avalonia.Windows.Dialogs.Enums
{
    /// <summary>
    /// Specifies the type of dialog to display, such as informational, confirmation, warning or error dialogs.
    /// </summary>
    public enum DialogType
    {
        /// <summary>
        /// Nothing
        /// </summary>
        None,

        /// <summary>
        /// Information
        /// </summary>
        Information,

        /// <summary>
        /// Confirmation
        /// </summary>
        Confirmation,

        /// <summary>
        /// Warning
        /// </summary>
        Warning,

        /// <summary>
        /// Error
        /// </summary>
        Error,
    }
}
