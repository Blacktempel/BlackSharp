/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions;
using System.Collections;

namespace BlackSharp.Core.Tests.Extensions
{
    enum CarParts
    {
        Window,
        OilPump,
        Exhaust,
    }

    [TestClass]
    public class ObjectExtensionsTests
    {
        [TestMethod]
        public void Any()
        {
            Assert.IsTrue(ObjectExtensions.Any(1, [1, 2, 3]));
            Assert.IsTrue(ObjectExtensions.Any(40, 2, 3, 40, 5));
            Assert.IsTrue(ObjectExtensions.Any(90, 5, 90));
        }

        [TestMethod]
        public void AnyOf()
        {
            IEnumerable e = new List<CarParts> { CarParts.OilPump, CarParts.Window };

            Assert.IsTrue(ObjectExtensions.AnyOf(CarParts.Window, CarParts.OilPump, CarParts.Window));
            Assert.IsTrue(ObjectExtensions.AnyOf(CarParts.Window, CarParts.OilPump, CarParts.Window, CarParts.Exhaust));
            Assert.IsTrue(ObjectExtensions.AnyOf(CarParts.Window, e));
        }

        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(ObjectExtensions.IsNumber(5));
            Assert.IsTrue(ObjectExtensions.IsNumber(5.0));
            Assert.IsTrue(ObjectExtensions.IsNumber(5.0M));

            Assert.IsFalse(ObjectExtensions.IsNumber("Number"));
        }
    }
}
