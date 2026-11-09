// SPDX-License-Identifier: MPL-2.0

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Collections.Specialized;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// Adds a keyboard driven tab switcher to a reorderable tab control.
    /// </summary>
    /// <remarks>
    /// Ctrl+Tab and Ctrl+Shift+Tab move the preview selection. Releasing Ctrl activates it,
    /// while Escape cancels the preview. The displayed item template is supplied by the application.
    /// When focus is lost or remains on the outgoing tab, the selected tab header receives it.
    /// The overlay border receives the <c>tab-switcher-popup</c> class and its list receives
    /// <c>tab-switcher-items</c>. Applications can style those elements and the list's
    /// <see cref="ListBoxItem"/> containers. Avalonia's default list appearance supports
    /// the keyboard behavior as well.
    /// </remarks>
    public class KeyboardSwitchingTabControl : ReorderableTabControl
    {
        #region Fields

        /// <summary>
        /// Provides the default text preview for switcher items.
        /// </summary>
        private static readonly IDataTemplate s_defaultSwitchItemTemplate =
            new FuncDataTemplate<object>((item, _) => new TextBlock
            {
                Text = item is TabItem tab ? tab.Header?.ToString() : item?.ToString(),
            });

        /// <summary>
        /// Holds the visible preview popup while a keyboard switch is in progress.
        /// </summary>
        private Border _switcher;

        /// <summary>
        /// Displays the available tabs and highlights the previewed one.
        /// </summary>
        private ListBox _switcherList;

        /// <summary>
        /// Preserves the tab order for the lifetime of the current preview.
        /// </summary>
        private object[] _switcherItems;

        /// <summary>
        /// Identifies the item currently highlighted in the preview.
        /// </summary>
        private int _switcherIndex;

        /// <summary>
        /// Hosts the tab-switching popup above the window content.
        /// </summary>
        private OverlayLayer _overlayLayer;

        /// <summary>
        /// Receives shortcuts and pointer events even when a child control has focus.
        /// </summary>
        private TopLevel _topLevel;

        /// <summary>
        /// Provides the deactivation event used to cancel an unfinished preview.
        /// </summary>
        private Window _window;

        #endregion

        #region Avalonia Properties

        /// <summary>
        /// Defines the <see cref="SwitchItemTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> SwitchItemTemplateProperty =
            AvaloniaProperty.Register<KeyboardSwitchingTabControl, IDataTemplate>(nameof(SwitchItemTemplate));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the template used to display items in the keyboard switcher.
        /// </summary>
        /// <remarks>
        /// When unset, the control uses its regular tab item template.
        /// </remarks>
        public IDataTemplate SwitchItemTemplate
        {
            get => GetValue(SwitchItemTemplateProperty);
            set => SetValue(SwitchItemTemplateProperty, value);
        }

        /// <inheritdoc/>
        protected override Type StyleKeyOverride => typeof(TabControl);

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            _topLevel = TopLevel.GetTopLevel(this);
            _window   = _topLevel as Window;

            // Listen at the window so shortcuts also work while a sensor grid or another child has focus.
            _topLevel?.AddHandler(KeyDownEvent       , OnTopLevelKeyDown       , RoutingStrategies.Tunnel, handledEventsToo: true);
            _topLevel?.AddHandler(KeyUpEvent         , OnTopLevelKeyUp         , RoutingStrategies.Tunnel, handledEventsToo: true);
            _topLevel?.AddHandler(PointerPressedEvent, OnTopLevelPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);

            if (_topLevel != null)
            {
                _topLevel.SizeChanged += OnTopLevelSizeChanged;
            }

            Items.CollectionChanged += OnItemsCollectionChanged;

            if (_window != null)
            {
                _window.Deactivated += OnWindowDeactivated;
            }
        }

        /// <inheritdoc/>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            CloseSwitcher(false);

            _topLevel?.RemoveHandler(KeyDownEvent       , OnTopLevelKeyDown       );
            _topLevel?.RemoveHandler(KeyUpEvent         , OnTopLevelKeyUp         );
            _topLevel?.RemoveHandler(PointerPressedEvent, OnTopLevelPointerPressed);

            if (_topLevel != null)
            {
                _topLevel.SizeChanged -= OnTopLevelSizeChanged;
            }

            Items.CollectionChanged -= OnItemsCollectionChanged;

            if (_window != null)
            {
                _window.Deactivated -= OnWindowDeactivated;
            }

            _topLevel = null;
            _window   = null;

            base.OnDetachedFromVisualTree(e);
        }

        #endregion

        #region Private

        /// <summary>
        /// Checks whether an item at the given index can be activated by the switcher.
        /// </summary>
        /// <param name="index">The index in the tab items collection.</param>
        /// <returns>Whether the item exists and its available header is visible and enabled.</returns>
        private bool CanSwitchToTab(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return false;
            }

            var tab = ContainerFromIndex(index) as TabItem ?? Items[index] as TabItem;

            return tab == null || (tab.IsEffectivelyVisible && tab.IsEffectivelyEnabled);
        }

        /// <summary>
        /// Closes the switcher and optionally activates its preview selection.
        /// </summary>
        /// <param name="activateSelection">Whether to activate the previewed item.</param>
        private void CloseSwitcher(bool activateSelection)
        {
            if (_switcher == null)
            {
                return;
            }

            var selectedItem = _switcherItems[_switcherIndex];

            _overlayLayer?.Children.Remove(_switcher);

            _switcher      = null;
            _switcherList  = null;
            _switcherItems = null;
            _overlayLayer  = null;

            // A tab can be removed, hidden or disabled while the preview is open.
            if (!activateSelection
             || !CanSwitchToTab(Items.IndexOf(selectedItem))
             || Equals(SelectedItem, selectedItem))
            {
                return;
            }

            var topLevel      = _topLevel;
            var previousFocus = topLevel?.FocusManager?.GetFocusedElement();

            SelectedItem = selectedItem;

            if (topLevel == null || TryRestoreTabFocus(topLevel, selectedItem, previousFocus))
            {
                return;
            }

            // Selection bindings can replace focused content during the next layout pass.
            Dispatcher.UIThread.Post(() =>
            {
                if (topLevel is not Window { IsActive: false })
                {
                    TryRestoreTabFocus(topLevel, selectedItem, previousFocus);
                }
            }, DispatcherPriority.Loaded);
        }

        /// <summary>
        /// Finds the preview item represented by a pointer event inside the switcher list.
        /// </summary>
        /// <param name="source">The pointer event source.</param>
        /// <returns>The clicked item, or null when no item was clicked.</returns>
        private object FindClickedItem(object source)
        {
            if (source is not Visual visual)
            {
                return null;
            }

            // Walk up the visual tree to find the clicked ListBoxItem, but stop at the switcher list.
            for (Visual current = visual; current != null; current = current.GetVisualParent())
            {
                if (current is ListBoxItem item)
                {
                    return item.Content;
                }

                if (ReferenceEquals(current, _switcherList))
                {
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// Opens the preview and advances it in the requested direction.
        /// </summary>
        /// <param name="direction">One for the next tab, or minus one for the previous tab.</param>
        /// <returns>Whether the preview moved.</returns>
        private bool MovePreview(int direction)
        {
            if (_switcher == null)
            {
                if (!IsEffectivelyEnabled || !IsEffectivelyVisible || Items.Count < 2)
                {
                    return false;
                }

                var switcherItems = Items.Where((_, index) => CanSwitchToTab(index)).ToArray();

                if (switcherItems.Length < 2)
                {
                    return false;
                }

                _overlayLayer = OverlayLayer.GetOverlayLayer(this);

                if (_overlayLayer == null)
                {
                    return false;
                }

                _switcherItems = switcherItems;
                _switcherIndex = Array.IndexOf(_switcherItems, SelectedItem);

                if (_switcherIndex < 0 && direction < 0)
                {
                    _switcherIndex = 0;
                }

                _switcherList = new ListBox
                {
                    Focusable    = false,
                    ItemTemplate = SwitchItemTemplate ?? ItemTemplate ?? s_defaultSwitchItemTemplate,
                    ItemsSource  = _switcherItems,
                };

                _switcherList.Classes.Add("tab-switcher-items");
                _switcherList.AddHandler(PointerPressedEvent, OnSwitcherPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);

                _switcher = new Border
                {
                    Child   = _switcherList,
                    Padding = new Thickness(6),
                    ZIndex  = 100,
                };

                _switcher.Classes.Add("tab-switcher-popup");
                _overlayLayer.Children.Add(_switcher);

                PositionSwitcher();
            }

            _switcherIndex = (_switcherIndex + direction + _switcherItems.Length) % _switcherItems.Length;

            _switcherList.SelectedIndex = _switcherIndex;
            _switcherList.ScrollIntoView(_switcherItems[_switcherIndex]);

            return true;
        }

        /// <summary>
        /// Cancels the preview when the window loses keyboard focus.
        /// </summary>
        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            CloseSwitcher(false);
        }

        /// <summary>
        /// Dismisses a preview whose item order may no longer match the available tabs.
        /// </summary>
        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CloseSwitcher(false);
        }

        /// <summary>
        /// Handles tab switching shortcuts before focused child controls consume them.
        /// </summary>
        private void OnTopLevelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _switcher != null)
            {
                CloseSwitcher(false);

                e.Handled = true;
                return;
            }

            if (e.Key != Key.Tab
             || (e.KeyModifiers != KeyModifiers.Control
              && e.KeyModifiers != (KeyModifiers.Control | KeyModifiers.Shift)))
            {
                return;
            }

            if (MovePreview(e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? -1 : 1))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Activates the previewed tab when either Ctrl key is released.
        /// </summary>
        private void OnTopLevelKeyUp(object sender, KeyEventArgs e)
        {
            if (_switcher != null && e.Key is Key.LeftCtrl or Key.RightCtrl)
            {
                CloseSwitcher(true);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Dismisses the switcher when the user clicks outside it.
        /// </summary>
        private void OnTopLevelPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (_switcher == null || e.Source is not Visual visual)
            {
                return;
            }

            // Walk up the visual tree to see if the click was inside the switcher.
            for (Visual current = visual; current != null; current = current.GetVisualParent())
            {
                if (ReferenceEquals(current, _switcher))
                {
                    return;
                }
            }

            CloseSwitcher(false);
        }

        /// <summary>
        /// Keeps the switcher centered if the window is resized while it is visible.
        /// </summary>
        private void OnTopLevelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionSwitcher();
        }

        /// <summary>
        /// Activates a tab immediately when its preview item is clicked.
        /// </summary>
        private void OnSwitcherPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(_switcherList).Properties.IsLeftButtonPressed)
            {
                return;
            }

            var item = FindClickedItem(e.Source);
            var index = item == null ? -1 : Array.IndexOf(_switcherItems, item);

            if (index < 0)
            {
                return;
            }

            _switcherIndex = index;

            CloseSwitcher(true);

            e.Handled = true;
        }

        /// <summary>
        /// Positions the keyboard switcher within its overlay layer.
        /// </summary>
        private void PositionSwitcher()
        {
            if (_switcher == null || _overlayLayer == null)
            {
                return;
            }

            var size   = _overlayLayer.Bounds.Size;

            if (size.Width <= 0 || size.Height <= 0)
            {
                size = _topLevel?.ClientSize ?? default;
            }

            var width     = Math.Max(1, Math.Min(390, size.Width  - 32));
            var maxHeight = Math.Max(1, Math.Min(440, size.Height - 32));

            _switcher.Width     = width;
            _switcher.MaxHeight = maxHeight;

            _switcher.Measure(new Size(width, maxHeight));

            var height = Math.Min(maxHeight, _switcher.DesiredSize.Height);

            Canvas.SetLeft(_switcher, Math.Max(0, (size.Width  - width ) / 2));
            Canvas.SetTop (_switcher, Math.Max(0, (size.Height - height) / 2));
        }

        /// <summary>
        /// Moves lost or outgoing tab focus to the selected tab header.
        /// </summary>
        /// <param name="topLevel">The host where the switch was committed.</param>
        /// <param name="selectedItem">The item activated by that switch.</param>
        /// <param name="previousFocus">The previously focused element, or null if no element was focused.</param>
        /// <returns>Whether focus was restored, moved elsewhere, or the switch no longer applies.</returns>
        private bool TryRestoreTabFocus(TopLevel topLevel, object selectedItem, IInputElement previousFocus)
        {
            if (!ReferenceEquals(_topLevel, topLevel)
             || _switcher != null
             || !Equals(SelectedItem, selectedItem))
            {
                return true;
            }

            if (topLevel.FocusManager?.GetFocusedElement()
                is InputElement { IsEffectivelyEnabled: true, IsEffectivelyVisible: true } focused
             && ReferenceEquals(TopLevel.GetTopLevel(focused), topLevel))
            {
                // Respect focus explicitly assigned by the consumer in response to the selection change.
                if (!ReferenceEquals(focused, previousFocus))
                {
                    return true;
                }

                // An unchanged, external target can still be valid, for example a shared sensor grid.
                if (!ReferenceEquals(focused, this) && !this.IsVisualAncestorOf(focused))
                {
                    return false;
                }
            }

            return ContainerFromIndex(SelectedIndex) is TabItem tab && tab.Focus(NavigationMethod.Unspecified);
        }

        #endregion
    }
}
