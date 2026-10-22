// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        #region Public

        /// <summary>
        /// Converts a duration to non-negative whole milliseconds, rounding partial milliseconds upward.
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
