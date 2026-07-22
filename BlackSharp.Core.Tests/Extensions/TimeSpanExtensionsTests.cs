/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

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
