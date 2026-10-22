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
    /// Defines a service for displaying message dialogs to the user.
    /// </summary>
    public interface IMessageDialogService
    {
        #region Public

        /// <summary>
        /// Shows an error message.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Shows an informational message.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        void ShowInformation(string title, string message);

        #endregion
    }
}
