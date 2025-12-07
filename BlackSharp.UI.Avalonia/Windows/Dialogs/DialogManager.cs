/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

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
