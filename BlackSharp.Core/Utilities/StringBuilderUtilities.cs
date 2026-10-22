// SPDX-License-Identifier: MPL-2.0

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
