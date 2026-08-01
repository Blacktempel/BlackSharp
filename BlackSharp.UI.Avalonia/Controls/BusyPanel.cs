/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// A control for a simple "busy" overlay.
    /// </summary>
    public class BusyPanel : ContentControl
    {
        #region Constructor

        static BusyPanel()
        {
            AffectsRender<BusyPanel>(IsBusyProperty,
                                     ShowProgressIndicatorProperty,
                                     BusyMessageProperty);
        }

        #endregion

        #region XAML Properties

        /// <summary>
        /// Changes the state of the control, enabling or disabling it.
        /// </summary>
        public static readonly AvaloniaProperty<bool> IsBusyProperty =
            AvaloniaProperty.Register<BusyPanel, bool>(nameof(IsBusy));

        /// <summary>
        /// Shows a progress control to inform user that control is busy.
        /// </summary>
        public static readonly AvaloniaProperty<bool> ShowProgressIndicatorProperty =
            AvaloniaProperty.Register<BusyPanel, bool>(nameof(ShowProgressIndicator), true);

        /// <summary>
        /// Message to show as "busy" message.
        /// </summary>
        public static readonly AvaloniaProperty<string> BusyMessageProperty =
            AvaloniaProperty.Register<BusyPanel, string>(nameof(BusyMessage), defaultBindingMode: BindingMode.TwoWay);

        #endregion

        #region Properties

        /// <summary>
        /// <inheritdoc cref="IsBusyProperty"/>
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        /// <summary>
        /// <inheritdoc cref="ShowProgressIndicatorProperty"/>
        /// </summary>
        public bool ShowProgressIndicator
        {
            get { return (bool)GetValue(ShowProgressIndicatorProperty); }
            set { SetValue(ShowProgressIndicatorProperty, value); }
        }

        /// <summary>
        /// <inheritdoc cref="BusyMessageProperty"/>
        /// </summary>
        public string BusyMessage
        {
            get { return GetValue(BusyMessageProperty) as string; }
            set { SetValue(BusyMessageProperty, value); }
        }

        #endregion
    }
}
