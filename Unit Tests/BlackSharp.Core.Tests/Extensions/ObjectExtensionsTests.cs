// SPDX-License-Identifier: MPL-2.0

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

            Assert.IsTrue (ObjectExtensions.Any("NVMe", ["SATA", "nvme"], StringComparer.OrdinalIgnoreCase));
            Assert.IsFalse(ObjectExtensions.Any("SAS" , ["SATA", "NVMe"], StringComparer.OrdinalIgnoreCase));
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
