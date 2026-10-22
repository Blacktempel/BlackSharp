// SPDX-License-Identifier: MPL-2.0

using Avalonia;
using BlackSharp.UI.Avalonia.Windows.Dialogs.Enums;

namespace BlackSharp.UI.Avalonia.Windows.Dialogs
{
    internal sealed class DialogManager
    {
        #region Fields

        static Dictionary<DialogSize, Size> _DialogSizes = new()
        {
            { DialogSize.Small , new Size(300, 225) },
            { DialogSize.Medium, new Size(500, 375) },
            { DialogSize.Large , new Size(700, 550) },
        };

        static Dictionary<DialogType, string> _DialogTitles = new()
        {
            { DialogType.Information , "Information" },
            { DialogType.Confirmation, "Confirm"     },
            { DialogType.Warning     , "Warning"     },
            { DialogType.Error       , "Error"       },
        };

        #endregion

        #region Public

        public static Size GetDialogSize(DialogSize dialogSize)
        {
            return _DialogSizes[dialogSize];
        }

        public static string GetDialogTitle(DialogType dialogType)
        {
            return _DialogTitles[dialogType];
        }

        #endregion
    }
}
