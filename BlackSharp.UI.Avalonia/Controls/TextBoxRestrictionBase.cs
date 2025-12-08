/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using BlackSharp.Core.Extensions;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// Provides a base class for text boxes that restrict user input according to custom validation rules.
    /// </summary>
    /// <remarks>Derive from this class to implement text boxes that allow or reject input based on specific
    /// criteria. Override the <see cref="IsTextAllowed"/> method to define the validation logic for permitted text.<br/>
    /// Input restrictions are enforced for both direct typing and clipboard pasting operations.</remarks>
    public abstract class TextBoxRestrictionBase : TextBox
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TextBoxRestrictionBase class.
        /// </summary>
        public TextBoxRestrictionBase()
        {
            AddHandler(TextInputEvent, TextInputHandler, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AddHandler(PastingFromClipboardEvent, PastingFromClipboardHandler, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        #endregion

        #region Protected

        /// <summary>
        /// Determines whether the specified text is permitted according to the implementation's criteria.
        /// </summary>
        /// <param name="text">The text to evaluate for allowance.</param>
        /// <returns>true if the specified text is allowed; otherwise, false.</returns>
        protected abstract bool IsTextAllowed(string text);

        #endregion

        #region Private

        void TextInputHandler(object sender, TextInputEventArgs e)
        {
            var text = ProcessTextChange(e.Text);

            if (!IsTextAllowed(text))
            {
                e.Handled = true;
            }
        }

        void PastingFromClipboardHandler(object sender, RoutedEventArgs e)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

            if (clipboard == null)
            {
                return;
            }

            var pastedText = ClipboardExtensions.TryGetTextAsync(clipboard).Result;

            if (pastedText == null)
            {
                return;
            }

            var text = ProcessTextChange(pastedText);

            if (!IsTextAllowed(text))
            {
                e.Handled = true;
            }
        }

        string ProcessTextChange(string text)
        {
            var temp = string.Empty;
            var selectionBegin = Math.Min(SelectionStart, SelectionEnd);
            var selectionEnd = Math.Max(SelectionStart, SelectionEnd);

            var selectionLength = selectionEnd - selectionBegin;

            if (selectionLength > 0)
            {
                temp = Text.ReplaceAtIndex(selectionBegin, selectionLength, text);
            }
            else
            {
                if (CaretIndex != Text.Length)
                {
                    if (CaretIndex > Text.Length)
                    {
                        CaretIndex = Text.Length;
                    }

                    temp = Text.Insert(CaretIndex, text);
                }
                else
                {
                    temp = Text + text;
                }
            }

            return temp;
        }

        #endregion
    }
}
