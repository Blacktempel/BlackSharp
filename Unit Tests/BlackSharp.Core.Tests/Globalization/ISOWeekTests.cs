// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Globalization;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class ISOWeekTests
    {
        [TestMethod]
        public void ToDateTime()
        {
            var one   = ISOWeek.ToDateTime(2025, 2);
            var two   = ISOWeek.ToDateTime(2025, 20);
            var three = ISOWeek.ToDateTime(2025, 52);

            Assert.AreEqual(new DateTime(2025, 01, 06), one);
            Assert.AreEqual(new DateTime(2025, 05, 12), two);
            Assert.AreEqual(new DateTime(2025, 12, 22), three);

            Assert.AreEqual(DayOfWeek.Monday, one   .DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, two   .DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, three .DayOfWeek);
        }
    }
}
