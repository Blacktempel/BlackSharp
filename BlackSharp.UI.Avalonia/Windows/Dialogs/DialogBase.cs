/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using BlackSharp.MVVM.Dialogs;
using BlackSharp.MVVM.Dialogs.Enums;
using BlackSharp.UI.Avalonia.Media;
using BlackSharp.UI.Avalonia.Windows.Dialogs.Enums;
using BlackSharp.UI.Avalonia.Windows.Media;
using BlackSharp.UI.Avalonia.Windows.Media.Enums;
using CommunityToolkit.Mvvm.Input;

namespace BlackSharp.UI.Avalonia.Windows.Dialogs
{
    /// <summary>
    /// Provides a base class for modal and non-modal dialog windows,
    /// supporting customizable buttons, messages and icons.
    /// </summary>
    /// <remarks><see cref="DialogBase"/> enables the creation and display of dialogs with configurable appearance and
    /// behavior, including support for standard and custom buttons, message text, and dialog icons.</remarks>
    public class DialogBase : Window
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of this class with default settings.
        /// </summary>
        public DialogBase()
            : this(DialogType.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified dialog type
        /// and a default size of <see cref="DialogSize.Medium"/>.
        /// </summary>
        /// <param name="dialogType">The type of dialog to be created.<br/>
        /// Determines the behavior and appearance of the dialog.</param>
        public DialogBase(DialogType dialogType)
            : this(dialogType, DialogSize.Medium)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified dialog size.
        /// </summary>
        /// <param name="dialogSize">The size of the dialog to be created.<br/>
        /// Determines the dimensions and layout of the dialog window.</param>
        public DialogBase(DialogSize dialogSize)
            : this(DialogType.None, dialogSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified dialog type and size.
        /// </summary>
        /// <remarks>This constructor sets up key bindings for closing the dialog with the Escape key and
        /// copying the message with Ctrl + C.<br/>
        /// It also configures commands for copying the dialog message to the clipboard and handling the default button action.</remarks>
        /// <param name="dialogType">The type of dialog to display.<br/>
        /// Determines the dialog's title and icon.</param>
        /// <param name="dialogSize">The size of the dialog window.<br/>
        /// Specifies the initial dimensions of the dialog.</param>
        public DialogBase(DialogType dialogType, DialogSize dialogSize)
        {
            Title = DialogManager.GetDialogTitle(dialogType);

            SetSize(dialogSize);
            SetIcon(dialogType);

            CopyMessageCommand = new(async (t) =>
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    var clipboard = GetTopLevel(this)?.Clipboard;

                    if (clipboard == null)
                    {
                        return;
                    }

                    await ClipboardExtensions.SetTextAsync(clipboard, Message);
                }
            });

            DefaultButtonCommand = new((db) =>
            {
                if (db != null)
                {
                    Result = db.ButtonType;

                    if (db.Command != null && db.Command.CanExecute(db.CommandParameter))
                    {
                        db.Command.Execute(db.CommandParameter);
                    }

                    if (db.IsCancel)
                    {
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            });

            KeyBindings.Add(new() { Command = new RelayCommand(Close), Gesture = new KeyGesture(Key.Escape) });
            KeyBindings.Add(new() { Command = CopyMessageCommand     , Gesture = new KeyGesture(Key.C, KeyModifiers.Control) });

            if (!Design.IsDesignMode)
            {
                Loaded += OnWindowLoaded;
            }

            Application.Current?.PlatformSettings?.ColorValuesChanged += (s, e) =>
            {
                SetBackgroundColor();
            };
        }

        #endregion

        #region XAML Properties

        /// <summary>
        /// Identifies the dialog buttons property, which specifies the collection of buttons displayed in the dialog.
        /// </summary>
        public static readonly AvaloniaProperty<IList<DialogButton>> DialogButtonsProperty =
            AvaloniaProperty.Register<DialogBase, IList<DialogButton>>(nameof(DialogButtons));

        /// <summary>
        /// Identifies the message property, which represents the message text displayed in the dialog.
        /// </summary>
        public static readonly AvaloniaProperty<string> MessageProperty =
            AvaloniaProperty.Register<DialogBase, string>(nameof(Message));

        #endregion

        #region Properties

        /// <summary>
        /// <inheritdoc cref="Window.StyleKeyOverride"/>
        /// </summary>
        protected override Type StyleKeyOverride => typeof(DialogBase);

        /// <summary>
        /// Gets the result of the dialog as a value indicating which button was selected.
        /// </summary>
        public DialogButtonType Result { get; protected set; }

        /// <summary>
        /// Gets the icon image displayed in the dialog.
        /// </summary>
        public Geometry IconData { get; protected set; }

        /// <summary>
        /// Gets the command that asynchronously copies the current message to the clipboard.
        /// </summary>
        public AsyncRelayCommand CopyMessageCommand { get; protected set; }

        /// <summary>
        /// Gets the command that is executed when the default button in the dialog is activated.
        /// </summary>
        public RelayCommand<DialogButton> DefaultButtonCommand { get; }

        /// <inheritdoc cref="DialogButtonsProperty"/>
        public IList<DialogButton> DialogButtons
        {
            get { return GetValue(DialogButtonsProperty) as IList<DialogButton>; }
            set { SetValue(DialogButtonsProperty, value); }
        }

        /// <inheritdoc cref="MessageProperty"/>
        public string Message
        {
            get { return GetValue(MessageProperty) as string; }
            set { SetValue(MessageProperty, value); }
        }

        #endregion

        #region Public

        /// <summary>
        /// Displays the dialog using the current title and message, with an OK button.
        /// </summary>
        public override void Show()
        {
            Show(Title, Message, MVVM.Dialogs.Enums.DialogButtons.OK, null);
        }

        /// <summary>
        /// Displays a dialog box with the specified title and message, using a default OK button.
        /// </summary>
        /// <param name="title">The text to display in the title bar of the dialog box. Cannot be null.</param>
        /// <param name="message">The message to display in the body of the dialog box. Cannot be null.</param>
        public void Show(string title, string message)
        {
            Show(title, message, MVVM.Dialogs.Enums.DialogButtons.OK, null);
        }

        /// <summary>
        /// Displays a dialog box with the specified title, message and a set of buttons.
        /// </summary>
        /// <param name="title">The text to display in the title bar of the dialog box. Cannot be null.</param>
        /// <param name="message">The message to display in the body of the dialog box. Cannot be null.</param>
        /// <param name="dialogButtons">A value that specifies which buttons to display in the dialog box.</param>
        public void Show(string title, string message, DialogButtons dialogButtons)
        {
            Show(title, message, dialogButtons, null);
        }

        /// <summary>
        /// Displays a dialog box with the specified title, message and a set of buttons.
        /// </summary>
        /// <param name="title">The text to display in the title bar of the dialog box. Cannot be null.</param>
        /// <param name="message">The message to display in the body of the dialog box. Cannot be null.</param>
        /// <param name="dialogButtons">A value that specifies which buttons to display in the dialog box.</param>
        /// <param name="customButtons">A collection of custom buttons to display in addition to or instead of the standard buttons.<br/>
        /// Can be null or empty if no custom buttons are needed.</param>
        public void Show(string title, string message, DialogButtons dialogButtons, IEnumerable<DialogButton> customButtons)
        {
            ShowWindow(title, message, dialogButtons, customButtons, false);
        }

        /// <summary>
        /// Displays the dialog using the current title and message, with an OK button.
        /// </summary>
        public void ShowDialog()
        {
            ShowDialog(Title, Message, MVVM.Dialogs.Enums.DialogButtons.OK, null);
        }

        /// <summary>
        /// Displays a dialog box with the specified title and message, using a default OK button.
        /// </summary>
        /// <param name="title">The text to display in the title bar of the dialog box. Cannot be null.</param>
        /// <param name="message">The message to display in the body of the dialog box. Cannot be null.</param>
        public void ShowDialog(string title, string message)
        {
            ShowDialog(title, message, MVVM.Dialogs.Enums.DialogButtons.OK, null);
        }

        /// <summary>
        /// Displays a dialog box with the specified title, message and a set of buttons.
        /// </summary>
        /// <param name="title">The text to display in the title bar of the dialog box. Cannot be null.</param>
        /// <param name="message">The message to display in the body of the dialog box. Cannot be null.</param>
        /// <param name="dialogButtons">A value that specifies which buttons to display in the dialog box.</param>
        public void ShowDialog(string title, string message, DialogButtons dialogButtons)
        {
            ShowDialog(title, message, dialogButtons, null);
        }

        /// <summary>
        /// Displays a dialog box with the specified title, message and a set of buttons.
        /// </summary>
        /// <param name="title">The text to display in the title bar of the dialog box. Cannot be null.</param>
        /// <param name="message">The message to display in the body of the dialog box. Cannot be null.</param>
        /// <param name="dialogButtons">A value that specifies which buttons to display in the dialog box.</param>
        /// <param name="customButtons">A collection of custom buttons to display in addition to or instead of the standard buttons.<br/>
        /// Can be null or empty if no custom buttons are needed.</param>
        public void ShowDialog(string title, string message, DialogButtons dialogButtons, IEnumerable<DialogButton> customButtons)
        {
            ShowWindow(title, message, dialogButtons, customButtons, true);
        }

        /// <summary>
        /// Sets the size of the dialog based on the specified dialog size option.
        /// </summary>
        /// <param name="dialogSize">A value that specifies the desired size of the dialog.<br/>
        /// Determines the width and height to apply.</param>
        public void SetSize(DialogSize dialogSize)
        {
            var size = DialogManager.GetDialogSize(dialogSize);

            Width  = size.Width ;
            Height = size.Height;
        }

        /// <summary>
        /// Sets the icon displayed for the specified dialog type.
        /// </summary>
        /// <param name="dialogType">The type of dialog for which to set the icon.<br/>
        /// Determines which icon will be shown.</param>
        public void SetIcon(DialogType dialogType)
        {
            switch (dialogType)
            {
                case DialogType.Information:
                    IconData = GeometryManager.GetGeometry(GeometryIcon.Information);
                    break;
                case DialogType.Confirmation:
                    IconData = GeometryManager.GetGeometry(GeometryIcon.Question);
                    break;
                case DialogType.Warning:
                    IconData = GeometryManager.GetGeometry(GeometryIcon.Exclamation);
                    break;
                case DialogType.Error:
                    IconData = GeometryManager.GetGeometry(GeometryIcon.Error);
                    break;
            }
        }

        #endregion

        #region Protected

        //protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        //{
        //    base.OnApplyTemplate(e);
        //}

        #endregion

        #region Private

        DialogButtonType ShowWindow(string title, string message, DialogButtons dialogButtons, IEnumerable<DialogButton> customButtons, bool dialog)
        {
            //Add default buttons
            DialogButtons = DialogButton.CreateDefaultButtons(dialogButtons);

            //Add custom buttons
            customButtons?.ToList().ForEach(DialogButtons.Add);

            Title = title;
            Message = message;

            UpdateLayout();

            var wnd = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (dialog)
            {
                //Block
                using (var source = new CancellationTokenSource())
                {
                    ShowDialog(wnd).ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                    Dispatcher.UIThread.MainLoop(source.Token);
                }
            }
            else
            {
                Show(wnd);
            }

            return Result;
        }

        void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetBackgroundColor();
        }

        void SetBackgroundColor()
        {
            var b = SystemColors.GetSystemBackground();
            if (b != null)
            {
                Background = b;
            }
        }

        #endregion
    }
}
