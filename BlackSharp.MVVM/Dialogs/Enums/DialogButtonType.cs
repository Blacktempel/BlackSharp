/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.MVVM.Dialogs.Enums
{
    /// <summary>
    /// Specifies the types of buttons that can be displayed in a dialog box.
    /// </summary>
    public enum DialogButtonType
    {
        /// <summary>
        /// This is used to indicate that no valid button was clicked.
        /// </summary>
        Invalid,

        /// <summary>
        /// A 'OK' button.
        /// </summary>
        OK,

        /// <summary>
        /// A 'Cancel' button.
        /// </summary>
        Cancel,

        /// <summary>
        /// A 'Yes' button.
        /// </summary>
        Yes,

        /// <summary>
        /// A 'No' button.
        /// </summary>
        No,

        /// <summary>
        /// A 'Retry' button.
        /// </summary>
        Retry,

        /// <summary>
        /// A 'Try Again' button.
        /// </summary>
        TryAgain,

        /// <summary>
        /// A 'Continue' button.
        /// </summary>
        Continue,

        /// <summary>
        /// Custom buttons start here.
        /// </summary>
        Custom = 0x100
    }
}
