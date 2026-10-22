// SPDX-License-Identifier: MPL-2.0

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
