/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Converters;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class TemperatureConverterTests
    {
        [TestMethod]
        public void CelsiusToFahrenheit()
        {
            Assert.AreEqual(32, TemperatureConverter.CelsiusToFahrenheit(0));
        }

        [TestMethod]
        public void FahrenheitToCelsius()
        {
            Assert.AreEqual(0, TemperatureConverter.FahrenheitToCelsius(32));
        }

        [TestMethod]
        public void CelsiusToKelvin()
        {
            Assert.AreEqual(273.15f, TemperatureConverter.CelsiusToKelvin(0));
        }

        [TestMethod]
        public void KelvinToCelsius()
        {
            Assert.AreEqual(-273.15f, TemperatureConverter.KelvinToCelsius(0));
        }
    }
}
