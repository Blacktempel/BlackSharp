/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using BlackSharp.MVVM.ComponentModel;
using BlackSharp.MVVM.Dialogs.Enums;
using CommunityToolkit.Mvvm.Input;

namespace BlackSharp.MVVM.Dialogs
{
    /// <summary>
    /// Represents a button within a dialog.
    /// </summary>
    public class DialogButton : ViewModelBase
    {
        #region Fields

        static Dictionary<DialogButtonType, string> _ContentMapping = new Dictionary<DialogButtonType, string>
        {
            { DialogButtonType.OK,       "OK"        },
            { DialogButtonType.Cancel,   "Cancel"    },
            { DialogButtonType.Yes,      "Yes"       },
            { DialogButtonType.No,       "No"        },
            { DialogButtonType.Retry,    "Retry"     },
            { DialogButtonType.TryAgain, "Try Again" },
            { DialogButtonType.Continue, "Continue"  },
        };

        #endregion

        #region Properties

        string _Content;
        public string Content
        {
            get { return _Content; }
            set { SetField(ref _Content, value); }
        }

        bool _IsDefault;
        public bool IsDefault
        {
            get { return _IsDefault; }
            set { SetField(ref _IsDefault, value); }
        }

        bool _IsCancel;
        public bool IsCancel
        {
            get { return _IsCancel; }
            set { SetField(ref _IsCancel, value); }
        }

        DialogButtonType _ButtonType;
        public DialogButtonType ButtonType
        {
            get { return _ButtonType; }
            set { SetField(ref _ButtonType, value); }
        }

        RelayCommand _Command;
        public RelayCommand Command
        {
            get { return _Command; }
            set { SetField(ref _Command, value); }
        }

        object _CommandParameter;
        public object CommandParameter
        {
            get { return _CommandParameter; }
            set { SetField(ref _CommandParameter, value); }
        }

        object _UserData;
        public object UserData
        {
            get { return _UserData; }
            set { SetField(ref _UserData, value); }
        }

        #endregion

        #region Public

        /// <summary>
        /// Creates a list of default dialog buttons based on the specified button configuration.
        /// </summary>
        /// <param name="type">A value that specifies the set of dialog buttons to include.</param>
        /// <returns>A list of <see cref="DialogButton"/> objects representing the default buttons for the given configuration.<br/>
        /// The list will be empty if the specified type does not match any predefined button set.</returns>
        public static List<DialogButton> CreateDefaultButtons(DialogButtons type)
        {
            var list = new List<DialogButton>();

            var createButton = new Func<DialogButtonType, DialogButton>((dbi) =>
            {
                //All default buttons are closing the dialog
                var btn = CreateButton(dbi, TryGet(_ContentMapping, dbi), true);
                return btn;
            });

            switch (type)
            {
                case DialogButtons.OK:
                    list.Add(createButton(DialogButtonType.OK));
                    break;
                case DialogButtons.OKCancel:
                    list.Add(createButton(DialogButtonType.OK));
                    list.Add(createButton(DialogButtonType.Cancel));
                    break;
                case DialogButtons.YesNo:
                    list.Add(createButton(DialogButtonType.Yes));
                    list.Add(createButton(DialogButtonType.No));
                    break;
                case DialogButtons.YesNoCancel:
                    list.Add(createButton(DialogButtonType.Yes));
                    list.Add(createButton(DialogButtonType.No));
                    list.Add(createButton(DialogButtonType.Cancel));
                    break;
                case DialogButtons.RetryCancel:
                    list.Add(createButton(DialogButtonType.Retry));
                    list.Add(createButton(DialogButtonType.Cancel));
                    break;
                case DialogButtons.CancelTryAgainContinue:
                    list.Add(createButton(DialogButtonType.Cancel));
                    list.Add(createButton(DialogButtonType.TryAgain));
                    list.Add(createButton(DialogButtonType.Continue));
                    break;
                default:
                    break;
            }

            return list;
        }

        /// <summary>
        /// Creates a new instance of a dialog button with the specified type, title and cancel behavior.
        /// </summary>
        /// <param name="type">The type of the button to create.</param>
        /// <param name="title">The text displayed on the button. Cannot be null.</param>
        /// <param name="close">true to designate the button as a cancel or close action; otherwise, false.</param>
        /// <returns>A DialogButton instance configured with the specified type, title and cancel behavior.</returns>
        public static DialogButton CreateButton(DialogButtonType type, string title, bool close = false)
        {
            return new DialogButton
            {
                Content    = title,
                ButtonType = type,
                IsCancel   = close
            };
        }

        #endregion

        #region Private

        static TValue TryGet<TKey, TValue>(Dictionary<TKey, TValue> lookup, TKey findAndGetValue)
        {
            if (lookup.TryGetValue(findAndGetValue, out var value))
                return value;
            else
                return default;
        }

        #endregion
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
