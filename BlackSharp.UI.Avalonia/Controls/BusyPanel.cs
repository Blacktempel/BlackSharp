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
using Avalonia.Data;

namespace BlackSharp.UI.Avalonia.Controls
{
    public class BusyPanel : ContentControl
    {
        #region Constructor

        static BusyPanel()
        {
            AffectsRender<BusyPanel>(IsBusyProperty,
                                     ShowProgressRingProperty,
                                     BusyMessageProperty);
        }

        #endregion

        #region XAML Properties

        public static readonly AvaloniaProperty<bool> IsBusyProperty =
            AvaloniaProperty.Register<BusyPanel, bool>(nameof(IsBusy));

        public static readonly AvaloniaProperty<bool> ShowProgressRingProperty =
            AvaloniaProperty.Register<BusyPanel, bool>(nameof(ShowProgressRing), true);

        public static readonly AvaloniaProperty<string> BusyMessageProperty =
            AvaloniaProperty.Register<BusyPanel, string>(nameof(ShowProgressRing), defaultBindingMode: BindingMode.TwoWay);

        #endregion

        #region Properties

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public bool ShowProgressRing
        {
            get { return (bool)GetValue(ShowProgressRingProperty); }
            set { SetValue(ShowProgressRingProperty, value); }
        }

        public string BusyMessage
        {
            get { return GetValue(BusyMessageProperty) as string; }
            set { SetValue(BusyMessageProperty, value); }
        }

        #endregion
    }
}
