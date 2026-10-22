/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace BlackSharp.UI.Avalonia.Behaviors
{
    /// <summary>
    /// Scrolls a control by one logical line while keeping the currently selected item
    /// unchanged when the user presses Ctrl+Up or Ctrl+Down.
    /// </summary>
    /// <remarks>
    /// The associated control must contain a ScrollViewer named PART_ScrollViewer. The behavior
    /// handles the key before the control's own directional navigation.
    /// </remarks>
    public sealed class PreserveSelectionScrollBehavior : Behavior<Control>
    {
        #region Fields

        /// <summary>
        /// Identifies the scroll viewer in the templated grid.
        /// </summary>
        private const string ScrollViewerPartName = "PART_ScrollViewer";

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);

            base.OnDetaching();
        }

        #endregion

        #region Private

        /// <summary>
        /// Scrolls by one logical line and keeps the currently selected item unchanged.
        /// </summary>
        /// <param name="sender">The control receiving the keyboard event.</param>
        /// <param name="e">The keyboard event.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled
             || e.KeyModifiers != KeyModifiers.Control
             || e.Key is not (Key.Up or Key.Down)
             || IsInputInsideInteractiveControl(e.Source))
            {
                return;
            }

            var scrollViewer = AssociatedObject.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault(viewer => viewer.Name == ScrollViewerPartName);

            if (scrollViewer == null)
            {
                return;
            }

            if (e.Key == Key.Up)
            {
                scrollViewer.LineUp();
            }
            else
            {
                scrollViewer.LineDown();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Leaves keyboard shortcuts of editors and other interactive controls intact.
        /// </summary>
        /// <param name="source">The element that received the keyboard input.</param>
        /// <returns>Whether the input originated inside an interactive control.</returns>
        private bool IsInputInsideInteractiveControl(object source)
        {
            if (source is not Visual visual)
            {
                return false;
            }

            // Walk up the visual tree to see if the input came from an interactive control.
            for (var current = visual; current != null; current = current.GetVisualParent())
            {
                if (ReferenceEquals(current, AssociatedObject))
                {
                    return false;
                }

                if (current is Button or ComboBox or NumericUpDown or TextBox)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
