/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using BlackSharp.MVVM.Events;
using System.Collections;
using System.Windows.Input;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// A <see cref="TabControl"/> whose tabs can be reordered with pointer drag and drop.
    /// </summary>
    /// <remarks>
    /// The control displays a live visual copy of the dragged tab by default.
    /// Set <see cref="DragPreviewTemplate"/> to display a custom preview for the dragged item instead.
    /// </remarks>
    public class ReorderableTabControl : TabControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableTabControl"/> class.
        /// </summary>
        public ReorderableTabControl()
        {
            // TabItem consumes pointer events while changing the selection.
            // Listening to handled events keeps drag detection working without interfering with the normal tab behavior.
            AddHandler(PointerMovedEvent   , OnTabPointerMoved   , RoutingStrategies.Bubble, handledEventsToo: true);
            AddHandler(PointerPressedEvent , OnTabPointerPressed , RoutingStrategies.Bubble, handledEventsToo: true);
            AddHandler(PointerReleasedEvent, OnTabPointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
        }

        #endregion

        #region Fields

        const double DefaultDragThreshold = 6;
        const double DropAreaMargin       = 16;
        const double PreviewOffset        = 12;

        private object       _draggedItem;
        private int          _draggedItemIndex = -1;
        private TabItem      _draggedTab;
        private Point        _dragStart;
        private Border       _dropIndicator;
        private int          _dropItemIndex = -1;
        private Border       _dragPreview;
        private IPointer     _dragPointer;
        private bool         _isDragging;
        private OverlayLayer _overlayLayer;

        #endregion

        #region Avalonia Properties

        /// <summary>
        /// Defines the <see cref="DragPreviewBackground"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DragPreviewBackgroundProperty =
            AvaloniaProperty.Register<ReorderableTabControl, IBrush>(nameof(DragPreviewBackground));

        /// <summary>
        /// Defines the <see cref="DragPreviewBorderBrush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DragPreviewBorderBrushProperty =
            AvaloniaProperty.Register<ReorderableTabControl, IBrush>(nameof(DragPreviewBorderBrush));

        /// <summary>
        /// Defines the <see cref="DragPreviewOpacity"/> property.
        /// </summary>
        public static readonly StyledProperty<double> DragPreviewOpacityProperty =
            AvaloniaProperty.Register<ReorderableTabControl, double>(nameof(DragPreviewOpacity), 0.88);

        /// <summary>
        /// Defines the <see cref="DragPreviewTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> DragPreviewTemplateProperty =
            AvaloniaProperty.Register<ReorderableTabControl, IDataTemplate>(nameof(DragPreviewTemplate));

        /// <summary>
        /// Defines the <see cref="DragThreshold"/> property.
        /// </summary>
        public static readonly StyledProperty<double> DragThresholdProperty =
            AvaloniaProperty.Register<ReorderableTabControl, double>(nameof(DragThreshold), DefaultDragThreshold);

        /// <summary>
        /// Defines the <see cref="DropIndicatorBrush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DropIndicatorBrushProperty =
            AvaloniaProperty.Register<ReorderableTabControl, IBrush>(
                nameof(DropIndicatorBrush),
                Brushes.DodgerBlue);

        /// <summary>
        /// Defines the <see cref="DropIndicatorThickness"/> property.
        /// </summary>
        public static readonly StyledProperty<double> DropIndicatorThicknessProperty =
            AvaloniaProperty.Register<ReorderableTabControl, double>(nameof(DropIndicatorThickness), 3);

        /// <summary>
        /// Defines the <see cref="IsReorderingEnabled"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsReorderingEnabledProperty =
            AvaloniaProperty.Register<ReorderableTabControl, bool>(nameof(IsReorderingEnabled), true);

        /// <summary>
        /// Defines the <see cref="ReorderCommand"/> property.
        /// </summary>
        public static readonly StyledProperty<ICommand> ReorderCommandProperty =
            AvaloniaProperty.Register<ReorderableTabControl, ICommand>(nameof(ReorderCommand));

        /// <summary>
        /// Defines the attached property that excludes a header element from initiating a tab drag.
        /// </summary>
        public static readonly AttachedProperty<bool> IsDragIgnoredProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsDragIgnored",
                typeof(ReorderableTabControl));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the background used behind a custom drag preview template.
        /// </summary>
        public IBrush DragPreviewBackground
        {
            get => GetValue(DragPreviewBackgroundProperty);
            set => SetValue(DragPreviewBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the drag preview border brush.
        /// </summary>
        public IBrush DragPreviewBorderBrush
        {
            get => GetValue(DragPreviewBorderBrushProperty);
            set => SetValue(DragPreviewBorderBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the drag preview opacity.
        /// </summary>
        public double DragPreviewOpacity
        {
            get => GetValue(DragPreviewOpacityProperty);
            set => SetValue(DragPreviewOpacityProperty, value);
        }

        /// <summary>
        /// Gets or sets the template used to display the dragged item.
        /// </summary>
        /// <remarks>
        /// If no template is assigned, the control displays a live visual copy of the dragged tab.
        /// </remarks>
        public IDataTemplate DragPreviewTemplate
        {
            get => GetValue(DragPreviewTemplateProperty);
            set => SetValue(DragPreviewTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum pointer distance required to begin dragging.
        /// </summary>
        public double DragThreshold
        {
            get => GetValue(DragThresholdProperty);
            set => SetValue(DragThresholdProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush used to draw the drop indicator.
        /// </summary>
        public IBrush DropIndicatorBrush
        {
            get => GetValue(DropIndicatorBrushProperty);
            set => SetValue(DropIndicatorBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the thickness of the drop indicator.
        /// </summary>
        public double DropIndicatorThickness
        {
            get => GetValue(DropIndicatorThicknessProperty);
            set => SetValue(DropIndicatorThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets whether pointer drag and drop can reorder tabs.
        /// </summary>
        public bool IsReorderingEnabled
        {
            get => GetValue(IsReorderingEnabledProperty);
            set => SetValue(IsReorderingEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the command that handles requested item reorder operations.
        /// </summary>
        /// <remarks>
        /// The command parameter is an <see cref="ItemReorderRequestedEventArgs"/> instance.
        /// If no command or event handler handles the request, the control attempts to move the
        /// item in its writable items source.
        /// </remarks>
        public ICommand ReorderCommand
        {
            get => GetValue(ReorderCommandProperty);
            set => SetValue(ReorderCommandProperty, value);
        }

        /// <inheritdoc/>
        protected override Type StyleKeyOverride => typeof(TabControl);

        #endregion

        #region Public

        /// <summary>
        /// Gets whether a header element is excluded from initiating a tab drag.
        /// </summary>
        /// <param name="element">Header element to inspect.</param>
        /// <returns><see langword="true"/> if dragging is ignored for the element; otherwise <see langword="false"/>.</returns>
        public static bool GetIsDragIgnored(AvaloniaObject element)
        {
            return element.GetValue(IsDragIgnoredProperty);
        }

        /// <summary>
        /// Sets whether a header element is excluded from initiating a tab drag.
        /// </summary>
        /// <param name="element">Header element to configure.</param>
        /// <param name="value"><see langword="true"/> to ignore drag initiation; otherwise <see langword="false"/>.</param>
        public static void SetIsDragIgnored(AvaloniaObject element, bool value)
        {
            element.SetValue(IsDragIgnoredProperty, value);
        }

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            CancelDrag();

            base.OnDetachedFromVisualTree(e);
        }

        /// <inheritdoc/>
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            ResetDrag();

            base.OnPointerCaptureLost(e);
        }

        /// <summary>
        /// Raises <see cref="ReorderRequested"/> and performs the default collection move if needed.
        /// </summary>
        /// <param name="e">Requested reorder operation.</param>
        protected virtual void OnReorderRequested(ItemReorderRequestedEventArgs e)
        {
            // Keep a CLR event for non-MVVM consumers, then prefer the bound command.
            // If neither handles the request, the control provides a useful automatic collection fallback.
            ReorderRequested?.Invoke(this, e);

            if (!e.Handled && ReorderCommand?.CanExecute(e) == true)
            {
                ReorderCommand.Execute(e);
                e.Handled = true;
            }

            if (!e.Handled)
            {
                e.Handled = TryMoveItem(e.OldIndex, e.NewIndex);
            }
        }

        #endregion

        #region Private

        bool BeginDrag()
        {
            if (_draggedTab == null
             || _draggedItemIndex < 0
             || !_draggedTab.IsVisible
             || _draggedTab.Bounds.Width <= 0
             || _draggedTab.Bounds.Height <= 0)
            {
                return false;
            }

            _overlayLayer = OverlayLayer.GetOverlayLayer(this);

            if (_overlayLayer == null)
            {
                return false;
            }

            // A custom template receives the dragged item.
            // Without one, a VisualBrush mirrors the complete rendered tab
            // so applications do not have to recreate their header design.
            var previewContent = DragPreviewTemplate == null
                ? null
                : new ContentControl
                {
                    Content                    = _draggedItem,
                    ContentTemplate            = DragPreviewTemplate,
                    HorizontalContentAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalContentAlignment   = global::Avalonia.Layout.VerticalAlignment.Stretch,
                };

            _dragPreview = new Border
            {
                Background      = previewContent == null
                    ? new VisualBrush(_draggedTab) { Stretch = Stretch.Fill }
                    : DragPreviewBackground,
                BorderBrush     = DragPreviewBorderBrush ?? DropIndicatorBrush,
                BorderThickness = new Thickness(1),
                BoxShadow       = BoxShadows.Parse("0 10 24 0 #59000000"),
                Child           = previewContent,
                CornerRadius    = new CornerRadius(5),
                Height          = _draggedTab.Bounds.Height,
                IsHitTestVisible = false,
                Opacity         = Math.Clamp(DragPreviewOpacity, 0, 1),
                Width           = _draggedTab.Bounds.Width,
            };

            _dropIndicator = new Border
            {
                Background       = DropIndicatorBrush,
                BoxShadow        = BoxShadows.Parse("0 0 7 0 #80000000"),
                CornerRadius     = new CornerRadius(2),
                IsHitTestVisible = false,
                ZIndex           = 1,
            };

            _dragPreview.Classes.Add("tab-drag-preview");
            _dropIndicator.Classes.Add("tab-drop-indicator");

            _overlayLayer.Children.Add(_dragPreview);
            _overlayLayer.Children.Add(_dropIndicator);

            _isDragging = true;

            return true;
        }

        void CancelDrag()
        {
            var pointer = _dragPointer;

            ResetDrag();

            if (ReferenceEquals(pointer?.Captured, this))
            {
                pointer.Capture(null);
            }
        }

        TabItem FindTab(object source)
        {
            if (source is not Visual visual)
            {
                return null;
            }

            for (Visual current = visual; current != null; current = current.GetVisualParent())
            {
                if (current is TabItem tabItem)
                {
                    return tabItem;
                }

                if (ReferenceEquals(current, this))
                {
                    break;
                }
            }

            return null;
        }

        TabLayout[] GetRealizedTabs()
        {
            // ContainerFromIndex returns null for tabs that are not currently materialized.
            // Only visible containers can supply reliable bounds for hit testing and indicator placement.
            var result = new List<TabLayout>(ItemCount);

            for (int index = 0; index < ItemCount; ++index)
            {
                if (ContainerFromIndex(index) is not TabItem tab
                 || !tab.IsVisible
                 || tab.Bounds.Width <= 0
                 || tab.Bounds.Height <= 0)
                {
                    continue;
                }

                var position = tab.TranslatePoint(default, this);

                if (position.HasValue)
                {
                    result.Add(new TabLayout(tab, index, position.Value));
                }
            }

            return result.ToArray();
        }

        bool IsDragInitiationIgnored(object source)
        {
            if (source is not Visual visual)
            {
                return false;
            }

            for (Visual current = visual; current != null; current = current.GetVisualParent())
            {
                // Header buttons and editors must retain their normal click and input behavior.
                if (current is Control control
                 && (GetIsDragIgnored(control)
                  || control is Button or ComboBox or NumericUpDown or Slider or TextBox))
                {
                    return true;
                }

                if (current is TabItem || ReferenceEquals(current, this))
                {
                    return false;
                }
            }

            return false;
        }

        static double GetDistanceSquared(Point point, Rect bounds)
        {
            var horizontalDistance = point.X < bounds.Left
                ? bounds.Left - point.X
                : point.X > bounds.Right
                    ? point.X - bounds.Right
                    : 0;

            var verticalDistance = point.Y < bounds.Top
                ? bounds.Top - point.Y
                : point.Y > bounds.Bottom
                    ? point.Y - bounds.Bottom
                    : 0;

            return (horizontalDistance * horizontalDistance)
                 + (verticalDistance   * verticalDistance  );
        }

        void OnTabPointerMoved(object sender, PointerEventArgs e)
        {
            if (!ReferenceEquals(e.Pointer, _dragPointer))
            {
                return;
            }

            var currentPoint = e.GetCurrentPoint(this);

            if (!currentPoint.Properties.IsLeftButtonPressed)
            {
                CancelDrag();
                return;
            }

            if (!_isDragging)
            {
                var offset    = currentPoint.Position - _dragStart;
                var threshold = Math.Max(0, DragThreshold);

                // Do not capture the pointer for an ordinary click.
                // Capture starts only after the configured movement threshold has clearly turned the gesture into a drag.
                if ((offset.X * offset.X) + (offset.Y * offset.Y) < threshold * threshold)
                {
                    return;
                }

                if (!BeginDrag())
                {
                    ResetDrag();
                    return;
                }

                e.Pointer.Capture(this);
            }

            UpdateDrag(e);

            e.Handled = true;
        }

        void OnTabPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!IsReorderingEnabled
             || ItemCount < 2
             || IsDragInitiationIgnored(e.Source))
            {
                return;
            }

            var currentPoint = e.GetCurrentPoint(this);

            if (!currentPoint.Properties.IsLeftButtonPressed)
            {
                return;
            }

            var tab   = FindTab(e.Source);
            var index = tab == null ? -1 : IndexFromContainer(tab);

            if (index < 0)
            {
                return;
            }

            CancelDrag();

            _draggedItem      = ItemsView.GetAt(index);
            _draggedItemIndex = index;
            _draggedTab       = tab;
            _dragPointer      = e.Pointer;
            _dragStart        = currentPoint.Position;
        }

        void OnTabPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (!ReferenceEquals(e.Pointer, _dragPointer))
            {
                return;
            }

            var wasDragging = _isDragging;
            var item        = _draggedItem;
            var oldIndex    = _draggedItemIndex;
            var dropIndex   = _dropItemIndex;
            var newIndex    = -1;

            if (wasDragging && oldIndex >= 0 && dropIndex >= 0)
            {
                // The drop index describes a gap in the original collection.
                // Removing an item from before that gap shifts the final item index one position to the left.
                newIndex = dropIndex > oldIndex
                    ? dropIndex - 1
                    : dropIndex;

                newIndex = Math.Clamp(newIndex, 0, ItemCount - 1);
            }

            CancelDrag();

            if (newIndex >= 0 && newIndex != oldIndex)
            {
                OnReorderRequested(
                    new ItemReorderRequestedEventArgs(
                        item,
                        oldIndex,
                        newIndex));
            }

            if (wasDragging && item != null)
            {
                // A collection move can make the selector temporarily choose another tab.
                // Restore the dragged item with SetCurrentValue so an existing two-way binding is preserved.
                SetCurrentValue(SelectedItemProperty, item);
            }

            e.Handled = wasDragging;
        }

        void ResetDrag()
        {
            if (_overlayLayer != null)
            {
                if (_dragPreview != null)
                {
                    _overlayLayer.Children.Remove(_dragPreview);
                }

                if (_dropIndicator != null)
                {
                    _overlayLayer.Children.Remove(_dropIndicator);
                }
            }

            if (_dragPreview != null)
            {
                // VisualBrush retains its source visual.
                // Clear it explicitly when the adorner is removed so an obsolete
                // tab cannot remain referenced after closing a connection.
                _dragPreview.Background = null;
                _dragPreview.Child      = null;
            }

            _draggedItem      = null;
            _draggedItemIndex = -1;
            _draggedTab       = null;
            _dragPointer      = null;
            _dropIndicator    = null;
            _dropItemIndex    = -1;
            _dragPreview      = null;
            _isDragging       = false;
            _overlayLayer     = null;
        }

        bool TryMoveItem(int oldIndex, int newIndex)
        {
            if (ItemsSource is not IList items
             || items.IsFixedSize
             || items.IsReadOnly
             || oldIndex < 0
             || oldIndex >= items.Count
             || newIndex < 0
             || newIndex >= items.Count)
            {
                return false;
            }

            var moveMethod = items.GetType().GetMethod(
                "Move",
                [typeof(int), typeof(int)]);

            // ObservableCollection.Move emits one move notification.
            // Prefer it over remove/insert because selectors and virtualized presenters preserve their state more reliably.
            if (moveMethod != null)
            {
                moveMethod.Invoke(items, [oldIndex, newIndex]);
                return true;
            }

            var selectedItem = SelectedItem;
            var item         = items[oldIndex];

            items.RemoveAt(oldIndex);
            items.Insert(newIndex, item);

            SelectedItem = selectedItem;

            return true;
        }

        void UpdateDrag(PointerEventArgs e)
        {
            if (_overlayLayer == null || _dragPreview == null || _dropIndicator == null)
            {
                return;
            }

            var overlayPosition = e.GetPosition(_overlayLayer);
            var overlaySize     = _overlayLayer.AvailableSize;
            var maximumLeft     = Math.Max(0, overlaySize.Width  - _dragPreview.Width );
            var maximumTop      = Math.Max(0, overlaySize.Height - _dragPreview.Height);

            Canvas.SetLeft(
                _dragPreview,
                Math.Clamp(overlayPosition.X + PreviewOffset, 0, maximumLeft));
            Canvas.SetTop(
                _dragPreview,
                Math.Clamp(overlayPosition.Y + PreviewOffset, 0, maximumTop));

            var tabs = GetRealizedTabs();

            if (tabs.Length < 2)
            {
                _dropIndicator.IsVisible = false;
                _dropItemIndex           = -1;
                return;
            }

            var pointerPosition = e.GetPosition(this);
            var stripBounds = tabs
                .Select(tab => tab.Bounds)
                .Aggregate((current, next) => current.Union(next));

            if (!stripBounds.Inflate(DropAreaMargin).Contains(pointerPosition))
            {
                _dropIndicator.IsVisible = false;
                _dropItemIndex           = -1;
                return;
            }

            var nearestTab = tabs.MinBy(tab => GetDistanceSquared(pointerPosition, tab.Bounds));

            // Tab strips may be horizontal, vertical or wrapped.
            // Choosing the geometrically nearest realized tab is more robust
            // than assuming that all headers share one coordinate axis.
            var horizontal = TabStripPlacement is Dock.Top or Dock.Bottom;

            var insertAfter = horizontal
                ? pointerPosition.X >= nearestTab.Bounds.Center.X
                : pointerPosition.Y >= nearestTab.Bounds.Center.Y;

            var indicatorPoint = horizontal
                ? new Point(
                    insertAfter ? nearestTab.Bounds.Right : nearestTab.Bounds.Left,
                    nearestTab.Bounds.Top + 2)
                : new Point(
                    nearestTab.Bounds.Left + 2,
                    insertAfter ? nearestTab.Bounds.Bottom : nearestTab.Bounds.Top);

            var indicatorPosition = this.TranslatePoint(indicatorPoint, _overlayLayer);

            if (!indicatorPosition.HasValue)
            {
                _dropIndicator.IsVisible = false;
                _dropItemIndex           = -1;
                return;
            }

            var indicatorThickness = Math.Max(1, DropIndicatorThickness);

            if (horizontal)
            {
                _dropIndicator.Width  = indicatorThickness;
                _dropIndicator.Height = Math.Max(1, nearestTab.Bounds.Height - 4);

                Canvas.SetLeft(
                    _dropIndicator,
                    indicatorPosition.Value.X - (indicatorThickness / 2));
                Canvas.SetTop(_dropIndicator, indicatorPosition.Value.Y);
            }
            else
            {
                _dropIndicator.Width  = Math.Max(1, nearestTab.Bounds.Width - 4);
                _dropIndicator.Height = indicatorThickness;

                Canvas.SetLeft(_dropIndicator, indicatorPosition.Value.X);
                Canvas.SetTop(
                    _dropIndicator,
                    indicatorPosition.Value.Y - (indicatorThickness / 2));
            }

            _dropIndicator.IsVisible = true;
            _dropItemIndex = insertAfter
                ? nearestTab.Index + 1
                : nearestTab.Index;
        }

        #endregion

        #region Nested Types

        sealed class TabLayout
        {
            public TabLayout(TabItem tab, int index, Point position)
            {
                Bounds = new Rect(position, tab.Bounds.Size);
                Index  = index;
            }

            public Rect Bounds { get; }
            public int Index { get; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a dragged tab requests a different item index.
        /// </summary>
        public event EventHandler<ItemReorderRequestedEventArgs> ReorderRequested;

        #endregion
    }
}
