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
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace BlackSharp.UI.Avalonia.Behaviors
{
    /// <summary>
    /// Attached property for <see cref="TextBlock"/>.<br/>
    /// Calculates and sets maximum width in order to allow <see cref="TextBlock.TextWrapping"/> inside of (e.g.) a <see cref="StackPanel"/>.
    /// </summary>
    public static class AutoWrapWidthAttached
    {
        #region Constructor

        static AutoWrapWidthAttached()
        {
            EnableProperty     .Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool  >>(OnEnableChanged     ));
            RightMarginProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnRightMarginChanged));
        }

        #endregion

        #region XAML Properties

        /// <summary>
        /// This property determines if this behavior is active.
        /// </summary>
        public static readonly AttachedProperty<bool> EnableProperty =
            AvaloniaProperty.RegisterAttached<TextBlock, bool>("Enable", typeof(AutoWrapWidthAttached));

        /// <summary>
        /// Optionally, a right margin can be set for this behavior.
        /// </summary>
        public static readonly AttachedProperty<double> RightMarginProperty =
            AvaloniaProperty.RegisterAttached<TextBlock, double>("RightMargin", typeof(AutoWrapWidthAttached), 0d);

        static readonly AttachedProperty<IDisposable> SubscriptionsProperty =
            AvaloniaProperty.RegisterAttached<TextBlock, IDisposable>("Subscriptions", typeof(AutoWrapWidthAttached));

        #endregion

        #region Public

        /// <summary>
        /// Sets value of <see cref="EnableProperty"/>.
        /// </summary>
        /// <param name="element">Element to set value to.</param>
        /// <param name="value">Value to set.</param>
        public static void SetEnable(AvaloniaObject element, bool value) =>
            element.SetValue(EnableProperty, value);

        /// <summary>
        /// Gets value of <see cref="EnableProperty"/>.
        /// </summary>
        /// <param name="element">Element to get value of.</param>
        /// <returns>Value of property.</returns>
        public static bool GetEnable(AvaloniaObject element) =>
            element.GetValue(EnableProperty);

        /// <summary>
        /// Sets value of <see cref="RightMarginProperty"/>.
        /// </summary>
        /// <param name="element">Element to set value to.</param>
        /// <param name="value">Value to set.</param>
        public static void SetRightMargin(AvaloniaObject element, double value) =>
            element.SetValue(RightMarginProperty, value);

        /// <summary>
        /// Gets value of <see cref="RightMarginProperty"/>.
        /// </summary>
        /// <param name="element">Element to get value of.</param>
        /// <returns>Value of property.</returns>
        public static double GetRightMargin(AvaloniaObject element) =>
            element.GetValue(RightMarginProperty);

        #endregion

        #region Private

        static void OnRightMarginChanged(AvaloniaPropertyChangedEventArgs<double> e)
        {
            if (e.Sender is TextBlock tb && GetEnable(tb))
            {
                UpdateWidth(tb);
            }
        }

        static void OnEnableChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is not TextBlock tb)
            {
                return;
            }

            if (e.NewValue.Value)
            {
                Attach(tb);
            }
            else
            {
                Detach(tb);
            }
        }

        static void Attach(TextBlock tb)
        {
            Detach(tb);

            void onAttach(object s, VisualTreeAttachmentEventArgs a)
            {
                var wnd = tb.GetVisualRoot() as Window;
                if (wnd == null)
                {
                    return;
                }

                //Handle TextBlock layout changes
                var subTbBounds = tb.GetObservable(Visual.BoundsProperty)
                                    .Subscribe(new AnonymousObserver<Rect>(_ => UpdateWidth(tb)));

                //Handle Window layout changes
                var subWinSize = wnd.GetObservable(TopLevel.ClientSizeProperty)
                                    .Subscribe(new AnonymousObserver<Size>(_ => UpdateWidth(tb)));

                //Initial update
                UpdateWidth(tb);

                //Save subscriptions
                var disp = new CompositeDisposable(subTbBounds, subWinSize);
                tb.SetValue(SubscriptionsProperty, disp);

                //Cleanup on detach
                void onDetach(object s2, VisualTreeAttachmentEventArgs a2)
                {
                    Detach(tb);
                    tb.DetachedFromVisualTree -= onDetach;
                }

                tb.DetachedFromVisualTree += onDetach;
            }

            tb.AttachedToVisualTree += onAttach;

            //Is attached to visual tree ?
            if (tb.GetVisualRoot() != null)
            {
                onAttach(tb, default!);
            }
        }

        static void Detach(TextBlock tb)
        {
            tb.GetValue(SubscriptionsProperty)?.Dispose();
            tb.SetValue(SubscriptionsProperty, null);
        }

        static void UpdateWidth(TextBlock tb)
        {
            //Get root
            var wnd = tb.GetVisualRoot() as Window;
            if (wnd == null)
            {
                return;
            }

            //Position of TextBlock relative to Window
            var t = tb.TransformToVisual(wnd);
            if (t == null)
            {
                return;
            }

            var leftTop = t?.Transform(new Point(0, 0));

            //Available width inside the client size of Window
            var clientWidth = wnd.ClientSize.Width;
            var rightMargin = GetRightMargin(tb);

            //Calculate available width
            var available = clientWidth - leftTop.GetValueOrDefault().X - rightMargin;

            if (double.IsNaN(available) || double.IsInfinity(available))
            {
                return;
            }

            if (available < 0)
            {
                available = 0;
            }

            static bool AreClose(double a, double b) => Math.Abs(a - b) <= 0.5;

            //Set MaxWidth for correct wrapping; update only if it has changed
            if (!AreClose(tb.MaxWidth, available))
            {
                tb.MaxWidth = available;
            }
        }

        //Helper for multi-dispose
        sealed class CompositeDisposable : IDisposable
        {
            public CompositeDisposable(params IDisposable[] items) => _items = items;

            IDisposable[] _items;

            public void Dispose()
            {
                foreach (var d in _items)
                    d.Dispose();
                _items = Array.Empty<IDisposable>();
            }
        }

        #endregion
    }
}
