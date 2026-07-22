/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Text;

namespace BlackSharp.Core.Utilities
{
    /// <summary>
    /// Provides helpers for building formatted text.
    /// </summary>
    public static class StringBuilderUtilities
    {
        #region Public

        /// <summary>
        /// Appends a line indented by two spaces per indentation level.
        /// </summary>
        /// <param name="builder">The target builder.</param>
        /// <param name="indentationLevel">The non-negative indentation level.</param>
        /// <param name="text">The text to append.</param>
        public static void AppendLine(
            StringBuilder builder,
            int indentationLevel,
            string text)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Append(' ', Math.Max(0, indentationLevel) * 2);
            builder.AppendLine(text ?? string.Empty);
        }

        #endregion
    }
}
