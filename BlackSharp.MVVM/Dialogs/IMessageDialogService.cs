// SPDX-License-Identifier: MPL-2.0

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
