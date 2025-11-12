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
using BlackSharp.Core.Extensions;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// Extension of a standard <see cref="Grid"/>, which automatically adds a margin between its childs.
    /// </summary>
    public class GridEx : Grid
    {
        #region Constructor

        static GridEx()
        {
            ChildMarginProperty.Changed.AddClassHandler<GridEx>((g, e) =>
            {
                g.UpdateChildMargins();
            });

            ChildMarginOffsetProperty.Changed.AddClassHandler<GridEx>((g, e) =>
            {
                g.UpdateChildMargins();
            });
        }

        #endregion

        #region Fields

        static readonly IReadOnlyCollection<Type>   ChildTypesWithIdent         = new List<Type> { typeof(Label), typeof(TextBlock) };
        static readonly Thickness                   DefaultChildMargin          = new Thickness(5, 3, 5, 3);
        static readonly int                         DefaultChildMarginOffset    = 10;

        #endregion

        #region XAML Properties

        /// <summary>
        /// Desired margin of children.
        /// </summary>
        public static readonly AvaloniaProperty<Thickness> ChildMarginProperty =
            AvaloniaProperty.Register<GridEx, Thickness>(nameof(ChildMargin), DefaultChildMargin);

        /// <summary>
        /// Offset for left margin.
        /// </summary>
        public static readonly AvaloniaProperty<int> ChildMarginOffsetProperty =
            AvaloniaProperty.Register<GridEx, int>(nameof(ChildMarginOffset), DefaultChildMarginOffset);

        #endregion

        #region Properties

        /// <summary>
        /// <inheritdoc cref="ChildMarginProperty"/>
        /// </summary>
        public Thickness ChildMargin
        {
            get { return (Thickness)GetValue(ChildMarginProperty); }
            set { SetValue(ChildMarginProperty, value); }
        }

        /// <summary>
        /// <inheritdoc cref="ChildMarginOffsetProperty"/>
        /// </summary>
        public int ChildMarginOffset
        {
            get { return (int)GetValue(ChildMarginOffsetProperty); }
            set { SetValue(ChildMarginOffsetProperty, value); }
        }

        #endregion

        #region Protected

        /// <inheritdoc cref="Grid.MeasureOverride"/>
        protected override Size MeasureOverride(Size constraint)
        {
            UpdateChildMargins();
            return base.MeasureOverride(constraint);
        }

        #endregion

        #region Private

        void UpdateChildMargins()
        {
            int maxCol = 0,
                maxRow = 0;

            foreach (Control child in Children)
            {
                if (!child.IsVisible)
                    continue;
                maxCol = Math.Max(maxCol, GetColumn(child));
                maxRow = Math.Max(maxRow, GetRow(child));
            }

            foreach (Control child in Children)
            {
                int col     = GetColumn     (child),
                    colSpan = GetColumnSpan (child),
                    row     = GetRow        (child),
                    rowSpan = GetRowSpan    (child);

                double  factorLeft      = col == 0 ? 0 : 0.5,
                        factorTop       = row == 0 ? 0 : 0.5,
                        factorRight     = (col + colSpan - 1) >= maxCol ? 0 : 0.5,
                        factorBottom    = (row + rowSpan - 1) >= maxRow ? 0 : 0.5;

                double marginLeft = ChildMargin.Left * factorLeft;

                if (ChildTypesWithIdent.Any(t => child.GetType().IsTypeOrSubclassOf(t)))
                    marginLeft = (ChildMargin.Left + ChildMarginOffset) * factorLeft;

                child.Margin = new Thickness
                (
                    marginLeft,
                    ChildMargin.Top     * factorTop,
                    ChildMargin.Right   * factorRight,
                    ChildMargin.Bottom  * factorBottom
                );
            }
        }

        #endregion
    }
}
