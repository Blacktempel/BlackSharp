/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Diagnostics;

namespace BlackSharp.Core.Threading
{
    /// <summary>
    /// Provides timeout calculations that remain valid when the environment tick counter wraps.
    /// </summary>
    public static class TimeoutUtilities
    {
        #region Public

        /// <summary>
        /// Gets the remaining timeout relative to a captured <see cref="Environment.TickCount"/> value.
        /// </summary>
        /// <param name="startedTickCount">The tick count captured when the operation started.</param>
        /// <param name="timeoutMilliseconds">The configured timeout in milliseconds.</param>
        /// <returns>The remaining timeout in milliseconds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeoutMilliseconds"/> is less than <see cref="Timeout.Infinite"/>.
        /// </exception>
        public static int GetRemainingMilliseconds(int startedTickCount, int timeoutMilliseconds)
        {
            ValidateTimeout(timeoutMilliseconds);

            if (timeoutMilliseconds == Timeout.Infinite)
            {
                return Timeout.Infinite;
            }

            var elapsed = unchecked((uint)(Environment.TickCount - startedTickCount));

            return elapsed >= timeoutMilliseconds
                 ? 0
                 : timeoutMilliseconds - (int)elapsed;
        }

        /// <summary>
        /// Gets the remaining timeout relative to an optional stopwatch.
        /// </summary>
        /// <param name="timeoutMilliseconds">The configured timeout in milliseconds.</param>
        /// <param name="stopwatch">The elapsed-time source, or <see langword="null"/> before timing starts.</param>
        /// <returns>The remaining timeout in milliseconds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeoutMilliseconds"/> is less than <see cref="Timeout.Infinite"/>.
        /// </exception>
        public static int GetRemainingMilliseconds(int timeoutMilliseconds, Stopwatch stopwatch)
        {
            ValidateTimeout(timeoutMilliseconds);

            if (timeoutMilliseconds == Timeout.Infinite)
            {
                return Timeout.Infinite;
            }

            if (timeoutMilliseconds == 0)
            {
                return 0;
            }

            if (stopwatch == null)
            {
                return timeoutMilliseconds;
            }

            var remaining = timeoutMilliseconds - stopwatch.ElapsedMilliseconds;

            if (remaining <= 0)
            {
                return 0;
            }

            return remaining > int.MaxValue ? int.MaxValue : (int)remaining;
        }

        #endregion

        #region Private

        static void ValidateTimeout(int timeoutMilliseconds)
        {
            if (timeoutMilliseconds < Timeout.Infinite)
            {
                throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
            }
        }

        #endregion
    }
}
