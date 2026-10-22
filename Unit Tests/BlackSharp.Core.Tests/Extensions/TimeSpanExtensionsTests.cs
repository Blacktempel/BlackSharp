// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class TimeSpanExtensionsTests
    {
        [TestMethod]
        public void ToDurationMilliseconds()
        {
            Assert.AreEqual(0L, TimeSpan.Zero.ToDurationMilliseconds());
            Assert.AreEqual(0L, TimeSpan.FromTicks(-1).ToDurationMilliseconds());
            Assert.AreEqual(1L, TimeSpan.FromTicks(1).ToDurationMilliseconds());
            Assert.AreEqual(1L, TimeSpan.FromMilliseconds(1).ToDurationMilliseconds());
            Assert.AreEqual(2L, TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond + 1).ToDurationMilliseconds());
        }
    }
}
