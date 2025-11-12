/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for basic types.
    /// </summary>
    public static class BasicTypeExtensions
    {
        #region Public

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this byte value, byte min, byte max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this short value, short min, short max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this int value, int min, int max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this long value, long min, long max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this float value, float min, float max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this double value, double min, double max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this decimal value, decimal min, decimal max)
        {
            return value <= max && value >= min;
        }

        /// <summary>
        /// Checks if value is between or equal to minimum and maximum provided value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Whether value is between or equal.</returns>
        public static bool Between(this DateTime value, DateTime min, DateTime max)
        {
            return value <= max && value >= min;
        }

        #endregion
    }
}
