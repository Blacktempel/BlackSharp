// SPDX-License-Identifier: MPL-2.0

using BlackSharp.MVVM.Dialogs;
using BlackSharp.MVVM.Dialogs.Enums;
using BlackSharp.UI.Avalonia.Windows.Dialogs;
using BlackSharp.UI.Avalonia.Windows.Dialogs.Enums;

namespace BlackSharp.UI.Avalonia.Services
{
    /// <summary>
    /// Displays confirmation dialogs through the shared Avalonia message box.
    /// </summary>
    public sealed class AvaloniaConfirmationDialogService : IConfirmationDialogService
    {
        #region Public

        /// <inheritdoc/>
        public bool Confirm(string title, string message)
        {
            var messageBox = new MessageBox(DialogType.Confirmation) { AllowOwnerlessDisplay = true };

            var buttons = DialogButton.CreateDefaultButtons(DialogButtons.YesNo);

            var noButton = buttons.Single(button => button.ButtonType == DialogButtonType.No);
            noButton.IsDefault = true;

            messageBox.ShowDialog(title, message, DialogButtons.Custom, buttons);

            return messageBox.Result == DialogButtonType.Yes;
        }

        #endregion
    }
}
