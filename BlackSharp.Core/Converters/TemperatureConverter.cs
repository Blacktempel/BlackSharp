/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Converters
{
    /// <summary>
    /// Helper class for temperature conversions.
    /// </summary>
    public static class TemperatureConverter
    {
        /// <summary>
        /// Converts given celsius temperature to fahrenheit.
        /// </summary>
        /// <param name="temperature">Temperature in celsius.</param>
        /// <returns>Returns temperature in fahrenheit.</returns>
        public static float CelsiusToFahrenheit(float temperature)
        {
            return temperature == 0 ? 32 : temperature * 9 / 5 + 32;
        }

        /// <summary>
        /// Converts given fahrenheit temperature to celsius.
        /// </summary>
        /// <param name="temperature">Temperature in fahrenheit.</param>
        /// <returns>Returns temperature in celsius.</returns>
        public static float FahrenheitToCelsius(float temperature)
        {
            return 5.0f / 9.0f * (temperature - 32);
        }
    }
}
