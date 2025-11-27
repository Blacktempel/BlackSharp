/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia;

namespace BlackSharp.UI.Avalonia.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="PixelRect"/> structure to support additional rectangle operations.
    /// </summary>
    public static class PixelRectExtensions
    {
        #region Public

        /// <summary>
        /// Returns a new <see cref="PixelRect"/> that is offset from the specified source rectangle by the given horizontal and vertical amounts.
        /// </summary>
        /// <param name="source">The <see cref="PixelRect"/> to offset. Represents the original rectangle whose position will be adjusted.</param>
        /// <param name="offsetX">The number of pixels to offset the rectangle horizontally. Positive values move the rectangle to the right;
        /// negative values move it to the left.</param>
        /// <param name="offsetY">The number of pixels to offset the rectangle vertically. Positive values move the rectangle downward;
        /// negative values move it upward.</param>
        /// <returns>A new <see cref="PixelRect"/> whose position is offset by the specified amounts. The width and height remain unchanged.</returns>
        public static PixelRect Offset(this PixelRect source, int offsetX, int offsetY)
        {
            return new PixelRect(source.X + offsetX, source.Y + offsetY, source.Width, source.Height);
        }

        #endregion
    }
}
