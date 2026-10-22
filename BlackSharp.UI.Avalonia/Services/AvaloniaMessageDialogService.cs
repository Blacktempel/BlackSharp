// SPDX-License-Identifier: MPL-2.0

using BlackSharp.MVVM.Dialogs;
using BlackSharp.UI.Avalonia.Windows.Dialogs;
using BlackSharp.UI.Avalonia.Windows.Dialogs.Enums;

namespace BlackSharp.UI.Avalonia.Services
{
    /// <summary>
    /// Displays informational and error dialogs through the shared Avalonia message box.
    /// </summary>
    public sealed class AvaloniaMessageDialogService : IMessageDialogService
    {
        #region Public

        /// <inheritdoc/>
        public void ShowError      (string title, string message) => Show(DialogType.Error, title, message);

        /// <inheritdoc/>
        public void ShowInformation(string title, string message) => Show(DialogType.Information, title, message);

        #endregion

        #region Private

        /// <summary>
        /// Opens a message box with the requested severity.
        /// </summary>
        /// <param name="type">The dialog severity.</param>
        /// <param name="title">The window title.</param>
        /// <param name="message">The displayed message.</param>
        private static void Show(DialogType type, string title, string message)
        {
            var messageBox = new MessageBox(type) { AllowOwnerlessDisplay = true };

            messageBox.ShowDialog(title, message);
        }

        #endregion
    }
}
