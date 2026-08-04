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
using Avalonia.Xaml.Interactivity;
using BlackSharp.MVVM.Events;
using System.Collections;
using System.Windows.Input;

namespace BlackSharp.UI.Avalonia.Behaviors
{
    /// <summary>
    /// Adds pointer-based row reordering to an Avalonia DataGrid-compatible control.
    /// </summary>
    /// <remarks>
    /// The behavior intentionally targets <see cref="Control"/> instead of a concrete DataGrid type.
    /// It can therefore be used with both Avalonia DataGrid and compatible implementations such as
    /// ProDataGrid. The associated control must expose registered <c>ItemsSource</c> and
    /// <c>SelectedItem</c> properties.
    /// </remarks>
    public class ReorderableDataGridBehavior : Behavior<Control>
    {
        #region Fields

        const double DefaultDragThreshold = 6;
        const double DropAreaMargin        = 16;
        const double PreviewOffset         = 12;

        private Control      _draggedContainer;
        private object       _draggedItem;
        private int          _draggedItemIndex = -1;
        private Point        _dragStart;
        private Border       _dragPreview;
        private IPointer     _dragPointer;
        private Border       _dropIndicator;
        private int          _dropItemIndex = -1;
        private object[]     _items = [];
        private bool         _isDragging;
        private OverlayLayer _overlayLayer;

        #endregion

        #region Avalonia Properties

        /// <summary>
        /// Defines the <see cref="DragPreviewBackground"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DragPreviewBackgroundProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, IBrush>(nameof(DragPreviewBackground));

        /// <summary>
        /// Defines the <see cref="DragPreviewBorderBrush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DragPreviewBorderBrushProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, IBrush>(nameof(DragPreviewBorderBrush));

        /// <summary>
        /// Defines the <see cref="DragPreviewOpacity"/> property.
        /// </summary>
        public static readonly StyledProperty<double> DragPreviewOpacityProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, double>(nameof(DragPreviewOpacity), 0.88);

        /// <summary>
        /// Defines the <see cref="DragPreviewTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> DragPreviewTemplateProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, IDataTemplate>(nameof(DragPreviewTemplate));

        /// <summary>
        /// Defines the <see cref="DragThreshold"/> property.
        /// </summary>
        public static readonly StyledProperty<double> DragThresholdProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, double>(
                nameof(DragThreshold),
                DefaultDragThreshold);

        /// <summary>
        /// Defines the <see cref="DropIndicatorBrush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DropIndicatorBrushProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, IBrush>(
                nameof(DropIndicatorBrush),
                Brushes.DodgerBlue);

        /// <summary>
        /// Defines the <see cref="DropIndicatorThickness"/> property.
        /// </summary>
        public static readonly StyledProperty<double> DropIndicatorThicknessProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, double>(
                nameof(DropIndicatorThickness),
                3);

        /// <summary>
        /// Defines the <see cref="IsReorderingEnabled"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsReorderingEnabledProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, bool>(nameof(IsReorderingEnabled), true);

        /// <summary>
        /// Defines the optional <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, IEnumerable>(nameof(ItemsSource));

        /// <summary>
        /// Defines the <see cref="ReorderCommand"/> property.
        /// </summary>
        public static readonly StyledProperty<ICommand> ReorderCommandProperty =
            AvaloniaProperty.Register<ReorderableDataGridBehavior, ICommand>(nameof(ReorderCommand));

        /// <summary>
        /// Defines the attached property that excludes an element from initiating a row drag.
        /// </summary>
        public static readonly AttachedProperty<bool> IsDragIgnoredProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsDragIgnored",
                typeof(ReorderableDataGridBehavior));

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
        /// If no template is assigned, the behavior displays a live visual copy of the dragged row.
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
        /// Gets or sets whether pointer drag and drop can reorder rows.
        /// </summary>
        public bool IsReorderingEnabled
        {
            get => GetValue(IsReorderingEnabledProperty);
            set => SetValue(IsReorderingEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets an optional items source override.
        /// </summary>
        /// <remarks>
        /// When omitted, the behavior reads the associated control's registered
        /// <c>ItemsSource</c> property.
        /// </remarks>
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the command that handles requested row reorder operations.
        /// </summary>
        /// <remarks>
        /// The command parameter is an <see cref="ItemReorderRequestedEventArgs"/> instance.
        /// If no command or event handler handles the request, the behavior attempts to move the
        /// item in its writable items source.
        /// </remarks>
        public ICommand ReorderCommand
        {
            get => GetValue(ReorderCommandProperty);
            set => SetValue(ReorderCommandProperty, value);
        }

        #endregion

        #region Public

        /// <summary>
        /// Gets whether an element is excluded from initiating a row drag.
        /// </summary>
        /// <param name="element">Element to inspect.</param>
        /// <returns><see langword="true"/> if dragging is ignored; otherwise <see langword="false"/>.</returns>
        public static bool GetIsDragIgnored(AvaloniaObject element)
        {
            return element.GetValue(IsDragIgnoredProperty);
        }

        /// <summary>
        /// Sets whether an element is excluded from initiating a row drag.
        /// </summary>
        /// <param name="element">Element to configure.</param>
        /// <param name="value"><see langword="true"/> to ignore drag initiation; otherwise <see langword="false"/>.</param>
        public static void SetIsDragIgnored(AvaloniaObject element, bool value)
        {
            element.SetValue(IsDragIgnoredProperty, value);
        }

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            // DataGrid cells usually mark pointer events as handled for selection and editing.
            // The behavior must still observe those events without replacing the grid's normal handlers.
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent   , OnPointerMoved   , RoutingStrategies.Bubble, handledEventsToo: true);
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent , OnPointerPressed , RoutingStrategies.Bubble, handledEventsToo: true);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);

            AssociatedObject.PointerCaptureLost += OnPointerCaptureLost;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            CancelDrag();

            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent   , OnPointerMoved   );
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent , OnPointerPressed );
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);

            AssociatedObject.PointerCaptureLost -= OnPointerCaptureLost;

            base.OnDetaching();
        }

        /// <summary>
        /// Raises <see cref="ReorderRequested"/> and performs the default collection move if needed.
        /// </summary>
        /// <param name="e">Requested reorder operation.</param>
        protected virtual void OnReorderRequested(ItemReorderRequestedEventArgs e)
        {
            // Keep a CLR event for non-MVVM consumers, then prefer the bound command.
            // Automatic collection reordering remains the final fallback for simple grids.
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
            if (_draggedContainer == null
             || _draggedItemIndex < 0
             || !_draggedContainer.IsVisible
             || _draggedContainer.Bounds.Width <= 0
             || _draggedContainer.Bounds.Height <= 0)
            {
                return false;
            }

            _overlayLayer = OverlayLayer.GetOverlayLayer(AssociatedObject);

            if (_overlayLayer == null)
            {
                return false;
            }

            // The default preview mirrors the complete rendered row.
            // A custom template instead receives the original item and can render a smaller application-specific preview.
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
                Background       = previewContent == null
                    ? new VisualBrush(_draggedContainer) { Stretch = Stretch.Fill }
                    : DragPreviewBackground,
                BorderBrush      = DragPreviewBorderBrush ?? DropIndicatorBrush,
                BorderThickness  = new Thickness(1),
                BoxShadow        = BoxShadows.Parse("0 10 24 0 #59000000"),
                Child            = previewContent,
                CornerRadius     = new CornerRadius(5),
                Height           = _draggedContainer.Bounds.Height,
                IsHitTestVisible = false,
                Opacity          = Math.Clamp(DragPreviewOpacity, 0, 1),
                Width            = _draggedContainer.Bounds.Width,
            };

            _dropIndicator = new Border
            {
                Background       = DropIndicatorBrush,
                BoxShadow        = BoxShadows.Parse("0 0 7 0 #80000000"),
                CornerRadius     = new CornerRadius(2),
                IsHitTestVisible = false,
                ZIndex           = 1,
            };

            _dragPreview.Classes.Add("data-grid-row-drag-preview");
            _dropIndicator.Classes.Add("data-grid-row-drop-indicator");

            _overlayLayer.Children.Add(_dragPreview);
            _overlayLayer.Children.Add(_dropIndicator);

            _isDragging = true;

            return true;
        }

        void CancelDrag()
        {
            var pointer = _dragPointer;

            ResetDrag();

            if (ReferenceEquals(pointer?.Captured, AssociatedObject))
            {
                pointer.Capture(null);
            }
        }

        Control FindItemContainer(object source, out int itemIndex)
        {
            itemIndex = -1;

            if (source is not Visual visual)
            {
                return null;
            }

            Control result = null;

            // Cells, presenters and the row commonly inherit the same item as DataContext.
            // Walking outward and keeping the last match yields the complete row without referencing a
            // concrete DataGridRow type from either Avalonia DataGrid or ProDataGrid.
            for (Visual current = visual; current != null; current = current.GetVisualParent())
            {
                if (ReferenceEquals(current, AssociatedObject))
                {
                    break;
                }

                if (current is not Control control)
                {
                    continue;
                }

                var currentIndex = FindItemIndex(control.DataContext);

                if (currentIndex >= 0)
                {
                    result    = control;
                    itemIndex = currentIndex;
                }
            }

            return result;
        }

        int FindItemIndex(object item)
        {
            if (item == null)
            {
                return -1;
            }

            for (int index = 0; index < _items.Length; ++index)
            {
                // Reference identity distinguishes separate record instances that happen to compare equal.
                // Value types cannot provide identity and therefore use value equality.
                if (ReferenceEquals(_items[index], item)
                 || item.GetType().IsValueType && Equals(_items[index], item))
                {
                    return index;
                }
            }

            return -1;
        }

        IEnumerable GetItemsSource()
        {
            if (ItemsSource != null)
            {
                return ItemsSource;
            }

            // Looking up the registered property by name avoids a compile-time dependency on one
            // particular DataGrid package while still using Avalonia's property system directly.
            var itemsSourceProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(
                AssociatedObject,
                "ItemsSource");

            return itemsSourceProperty == null
                ? null
                : AssociatedObject.GetValue(itemsSourceProperty) as IEnumerable;
        }

        RowLayout[] GetRealizedRows()
        {
            // DataGrids virtualize rows, so only currently realized visuals participate in hit testing.
            // Their stored item indices still refer to the complete source collection.
            var rows = new List<RowLayout>();

            foreach (var control in AssociatedObject.GetVisualDescendants().OfType<Control>())
            {
                var itemIndex = FindItemIndex(control.DataContext);

                // Several descendants share the row item as DataContext.
                // Keep only the outermost matching control, otherwise a cell and its row would create duplicate targets.
                if (itemIndex < 0
                 || HasItemContainerAncestor(control, itemIndex)
                 || !control.IsVisible
                 || control.Bounds.Width <= 0
                 || control.Bounds.Height <= 0)
                {
                    continue;
                }

                var position = control.TranslatePoint(default, AssociatedObject);

                if (position.HasValue)
                {
                    rows.Add(new RowLayout(control, itemIndex, position.Value));
                }
            }

            return rows
                .OrderBy(row => row.Bounds.Top)
                .ToArray();
        }

        bool HasItemContainerAncestor(Control control, int itemIndex)
        {
            for (Visual current = control.GetVisualParent(); current != null; current = current.GetVisualParent())
            {
                if (ReferenceEquals(current, AssociatedObject))
                {
                    return false;
                }

                if (current is Control parent && FindItemIndex(parent.DataContext) == itemIndex)
                {
                    return true;
                }
            }

            return false;
        }

        bool IsDragInitiationIgnored(object source)
        {
            if (source is not Visual visual)
            {
                return false;
            }

            for (Visual current = visual; current != null; current = current.GetVisualParent())
            {
                if (current is Control control
                 && (GetIsDragIgnored(control)
                  || control is Button or ComboBox or NumericUpDown or ScrollBar or Slider or TextBox))
                {
                    return true;
                }

                if (ReferenceEquals(current, AssociatedObject))
                {
                    return false;
                }
            }

            return false;
        }

        void OnPointerCaptureLost(object sender, PointerCaptureLostEventArgs e)
        {
            ResetDrag();
        }

        void OnPointerMoved(object sender, PointerEventArgs e)
        {
            if (!ReferenceEquals(e.Pointer, _dragPointer))
            {
                return;
            }

            var currentPoint = e.GetCurrentPoint(AssociatedObject);

            if (!currentPoint.Properties.IsLeftButtonPressed)
            {
                CancelDrag();
                return;
            }

            if (!_isDragging)
            {
                var offset    = currentPoint.Position - _dragStart;
                var threshold = Math.Max(0, DragThreshold);

                // Preserve ordinary selection and editing clicks by capturing the pointer only after
                // the movement has exceeded the configured drag threshold.
                if ((offset.X * offset.X) + (offset.Y * offset.Y) < threshold * threshold)
                {
                    return;
                }

                if (!BeginDrag())
                {
                    ResetDrag();
                    return;
                }

                e.Pointer.Capture(AssociatedObject);
            }

            UpdateDrag(e);

            e.Handled = true;
        }

        void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!IsReorderingEnabled || IsDragInitiationIgnored(e.Source))
            {
                return;
            }

            CancelDrag();

            // Freeze the source order for the duration of this gesture.
            // Virtualized visuals can be recycled while scrolling, but the drag indices must continue to describe one ordering.
            _items = GetItemsSource()?.Cast<object>().ToArray() ?? [];

            if (_items.Length < 2)
            {
                return;
            }

            var currentPoint = e.GetCurrentPoint(AssociatedObject);

            if (!currentPoint.Properties.IsLeftButtonPressed)
            {
                return;
            }

            var container = FindItemContainer(e.Source, out var itemIndex);

            if (container == null || itemIndex < 0)
            {
                return;
            }

            _draggedContainer = container;
            _draggedItem      = _items[itemIndex];
            _draggedItemIndex = itemIndex;
            _dragPointer      = e.Pointer;
            _dragStart        = currentPoint.Position;
        }

        void OnPointerReleased(object sender, PointerReleasedEventArgs e)
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
                // The indicator represents a gap in the original list.
                // Removing an earlier row shifts the final item index one position to the left.
                newIndex = dropIndex > oldIndex
                    ? dropIndex - 1
                    : dropIndex;
                newIndex = Math.Clamp(newIndex, 0, _items.Length - 1);
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
                RestoreSelectedItem(item);
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
                // Break the VisualBrush reference explicitly so a recycled row is not kept alive by
                // an adorner that has already been removed from the overlay.
                _dragPreview.Background = null;
                _dragPreview.Child      = null;
            }

            _draggedContainer = null;
            _draggedItem      = null;
            _draggedItemIndex = -1;
            _dragPointer      = null;
            _dragPreview      = null;
            _dropIndicator    = null;
            _dropItemIndex    = -1;
            _isDragging       = false;
            _items            = [];
            _overlayLayer     = null;
        }

        void RestoreSelectedItem(object item)
        {
            // Resolve SelectedItem through Avalonia's registry for compatibility with both
            // supported DataGrid implementations.
            // SetCurrentValue updates the value without replacing a binding.
            var selectedItemProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(
                AssociatedObject,
                "SelectedItem");

            if (selectedItemProperty != null)
            {
                AssociatedObject.SetCurrentValue(selectedItemProperty, item);
            }
        }

        bool TryMoveItem(int oldIndex, int newIndex)
        {
            if (GetItemsSource() is not IList items
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

            // Prefer ObservableCollection.Move and its single move notification.
            // The generic IList remove/insert path remains available for collections without a dedicated Move method.
            if (moveMethod != null)
            {
                moveMethod.Invoke(items, [oldIndex, newIndex]);
                return true;
            }

            var item = items[oldIndex];

            items.RemoveAt(oldIndex);
            items.Insert(newIndex, item);

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

            var rows = GetRealizedRows();

            if (rows.Length < 2)
            {
                _dropIndicator.IsVisible = false;
                _dropItemIndex           = -1;
                return;
            }

            var pointerPosition = e.GetPosition(AssociatedObject);
            var rowBounds = rows
                .Select(row => row.Bounds)
                .Aggregate((current, next) => current.Union(next));

            if (!rowBounds.Inflate(DropAreaMargin).Contains(pointerPosition))
            {
                _dropIndicator.IsVisible = false;
                _dropItemIndex           = -1;
                return;
            }

            var nearestRow = rows.MinBy(row => Math.Abs(pointerPosition.Y - row.Bounds.Center.Y));

            // The pointer half determines which gap is selected, so the indicator and resulting index
            // always describe insertion before or after the nearest realized row.
            var insertAfter = pointerPosition.Y >= nearestRow.Bounds.Center.Y;

            var indicatorPoint = new Point(
                nearestRow.Bounds.Left + 2,
                insertAfter ? nearestRow.Bounds.Bottom : nearestRow.Bounds.Top);

            var indicatorPosition = AssociatedObject.TranslatePoint(indicatorPoint, _overlayLayer);

            if (!indicatorPosition.HasValue)
            {
                _dropIndicator.IsVisible = false;
                _dropItemIndex           = -1;
                return;
            }

            var indicatorThickness = Math.Max(1, DropIndicatorThickness);

            _dropIndicator.Width  = Math.Max(1, nearestRow.Bounds.Width - 4);
            _dropIndicator.Height = indicatorThickness;

            Canvas.SetLeft(_dropIndicator, indicatorPosition.Value.X);
            Canvas.SetTop(
                _dropIndicator,
                indicatorPosition.Value.Y - (indicatorThickness / 2));

            _dropIndicator.IsVisible = true;
            _dropItemIndex = insertAfter
                ? nearestRow.Index + 1
                : nearestRow.Index;
        }

        #endregion

        #region Nested Types

        sealed class RowLayout
        {
            public RowLayout(Control container, int index, Point position)
            {
                Bounds = new Rect(position, container.Bounds.Size);
                Index  = index;
            }

            public Rect Bounds { get; }
            public int Index { get; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a dragged row requests a different item index.
        /// </summary>
        public event EventHandler<ItemReorderRequestedEventArgs> ReorderRequested;

        #endregion
    }
}
