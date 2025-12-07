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
    /// Specifies the set of button combinations that can be displayed in a dialog box.
    /// </summary>
    public enum DialogButtons
    {
        /// <summary>
        /// The dialog displays an OK button.
        /// </summary>
        OK,

        /// <summary>
        /// The dialog displays OK and Cancel buttons.
        /// </summary>
        OKCancel,

        /// <summary>
        /// The dialog displays Yes and No buttons.
        /// </summary>
        YesNo,

        /// <summary>
        /// The dialog displays Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel,

        /// <summary>
        /// The dialog displays Retry and Cancel buttons.
        /// </summary>
        RetryCancel,

        /// <summary>
        /// The dialog displays Cancel, Try Again and Continue buttons.
        /// </summary>
        CancelTryAgainContinue,

        /// <summary>
        /// The dialog displays entirely custom buttons.
        /// </summary>
        Custom = 0x100
    }
}
