/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        #region Public

        /// <summary>
        /// Converts a duration to non-negative whole milliseconds without under-reporting partial milliseconds.
        /// </summary>
        /// <param name="duration">Duration to convert.</param>
        /// <returns>The non-negative duration in whole milliseconds, rounded towards positive infinity.</returns>
        public static long ToDurationMilliseconds(this TimeSpan duration)
        {
            return Math.Max(0, (long)Math.Ceiling(duration.TotalMilliseconds));
        }

        #endregion
    }
}
