/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.MVVM.Dialogs
{
    /// <summary>
    /// Defines a service for displaying confirmation dialogs to the user.
    /// </summary>
    public interface IConfirmationDialogService
    {
        #region Public

        /// <summary>
        /// Asks the user to confirm an action.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The question to display.</param>
        /// <returns>Whether the user confirmed the action.</returns>
        bool Confirm(string title, string message);

        #endregion
    }
}
