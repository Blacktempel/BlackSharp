// SPDX-License-Identifier: MPL-2.0

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// Provides a reusable window with a custom title bar and native window decorations.
    /// </summary>
    public class DecoratedWindow : Window
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedWindow"/> class.
        /// </summary>
        public DecoratedWindow()
        {
            ExtendClientAreaToDecorationsHint  = true;
            ExtendClientAreaTitleBarHeightHint = DefaultTitleBarHeight;
            WindowDecorations                  = WindowDecorations.Full;

            UpdateTitleBarState();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Defines the default title-bar height.
        /// </summary>
        private const double DefaultTitleBarHeight = 40.0;

        /// <summary>
        /// Stores the shared title bar height property state used across component instances.
        /// </summary>
        public static readonly StyledProperty<double> TitleBarHeightProperty =
            AvaloniaProperty.Register<DecoratedWindow, double>(nameof(TitleBarHeight), DefaultTitleBarHeight);

        /// <summary>
        /// Stores the shared title bar icon property state used across component instances.
        /// </summary>
        public static readonly StyledProperty<IImage> TitleBarIconProperty =
            AvaloniaProperty.Register<DecoratedWindow, IImage>(nameof(TitleBarIcon));

        /// <summary>
        /// Stores the shared title bar background property state used across component instances.
        /// </summary>
        public static readonly StyledProperty<IBrush> TitleBarBackgroundProperty =
            AvaloniaProperty.Register<DecoratedWindow, IBrush>(nameof(TitleBarBackground));

        /// <summary>
        /// Stores the shared title bar menu property state used across component instances.
        /// </summary>
        public static readonly StyledProperty<object> TitleBarMenuProperty =
            AvaloniaProperty.Register<DecoratedWindow, object>(nameof(TitleBarMenu));

        /// <summary>
        /// Stores the shared has title bar icon property state used across component instances.
        /// </summary>
        public static readonly DirectProperty<DecoratedWindow, bool>
            HasTitleBarIconProperty =
                AvaloniaProperty.RegisterDirect<DecoratedWindow, bool>(
                    nameof(HasTitleBarIcon),
                    window => window.HasTitleBarIcon);

        /// <summary>
        /// Stores the shared has title bar menu property state used across component instances.
        /// </summary>
        public static readonly DirectProperty<DecoratedWindow, bool>
            HasTitleBarMenuProperty =
                AvaloniaProperty.RegisterDirect<DecoratedWindow, bool>(
                    nameof(HasTitleBarMenu),
                    window => window.HasTitleBarMenu);

        /// <summary>
        /// Stores the shared has window title property state used across component instances.
        /// </summary>
        public static readonly DirectProperty<DecoratedWindow, bool>
            HasWindowTitleProperty =
                AvaloniaProperty.RegisterDirect<DecoratedWindow, bool>(
                    nameof(HasWindowTitle),
                    window => window.HasWindowTitle);

        private bool _hasTitleBarIcon;
        private bool _hasTitleBarMenu;
        private bool _hasWindowTitle;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title bar height.
        /// </summary>
        public double TitleBarHeight
        {
            get => GetValue(TitleBarHeightProperty);
            set => SetValue(TitleBarHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the title bar icon.
        /// </summary>
        public IImage TitleBarIcon
        {
            get => GetValue(TitleBarIconProperty);
            set => SetValue(TitleBarIconProperty, value);
        }

        /// <summary>
        /// Gets or sets the title bar background.
        /// </summary>
        public IBrush TitleBarBackground
        {
            get => GetValue(TitleBarBackgroundProperty);
            set => SetValue(TitleBarBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the title bar menu.
        /// </summary>
        public object TitleBarMenu
        {
            get => GetValue(TitleBarMenuProperty);
            set => SetValue(TitleBarMenuProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has title bar icon.
        /// </summary>
        public bool HasTitleBarIcon
        {
            get => _hasTitleBarIcon;
            private set => SetAndRaise(HasTitleBarIconProperty, ref _hasTitleBarIcon, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has title bar menu.
        /// </summary>
        public bool HasTitleBarMenu
        {
            get => _hasTitleBarMenu;
            private set => SetAndRaise(HasTitleBarMenuProperty, ref _hasTitleBarMenu, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has window title.
        /// </summary>
        public bool HasWindowTitle
        {
            get => _hasWindowTitle;
            private set => SetAndRaise(HasWindowTitleProperty, ref _hasWindowTitle, value);
        }

        /// <inheritdoc/>
        protected override Type StyleKeyOverride => typeof(DecoratedWindow);

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TitleProperty
             || change.Property == TitleBarIconProperty
             || change.Property == TitleBarMenuProperty)
            {
                UpdateTitleBarState();
            }

            if (change.Property == TitleBarHeightProperty)
            {
                ExtendClientAreaTitleBarHeightHint = Math.Max(0.0, TitleBarHeight);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Determines whether content.
        /// </summary>
        /// <param name="value">The value to use for has content.</param>
        /// <returns>
        /// <see langword="true"/> when the requested condition is satisfied; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasContent(object value)
        {
            return value switch
            {
                null        => false,
                string text => !string.IsNullOrWhiteSpace(text),
                _           => true,
            };
        }

        /// <summary>
        /// Updates title bar state.
        /// </summary>
        private void UpdateTitleBarState()
        {
            HasTitleBarIcon = TitleBarIcon != null;
            HasTitleBarMenu = HasContent(TitleBarMenu);
            HasWindowTitle  = !string.IsNullOrWhiteSpace(Title);
        }

        #endregion
    }
}
