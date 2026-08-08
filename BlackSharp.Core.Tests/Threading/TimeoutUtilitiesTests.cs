/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Threading;
using System.Diagnostics;

namespace BlackSharp.Core.Tests.Threading
{
    [TestClass]
    public class TimeoutUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void GetRemainingMillisecondsFromTickCount()
        {
            Assert.AreEqual(Timeout.Infinite, TimeoutUtilities.GetRemainingMilliseconds(0, Timeout.Infinite));
            Assert.AreEqual(0, TimeoutUtilities.GetRemainingMilliseconds(Environment.TickCount - 100, 50));
        }

        [TestMethod]
        public void GetRemainingMillisecondsFromStopwatch()
        {
            Assert.AreEqual(100, TimeoutUtilities.GetRemainingMilliseconds(100, null));
            Assert.AreEqual(0, TimeoutUtilities.GetRemainingMilliseconds(0, Stopwatch.StartNew()));
            Assert.AreEqual(Timeout.Infinite, TimeoutUtilities.GetRemainingMilliseconds(Timeout.Infinite, Stopwatch.StartNew()));
        }

        [TestMethod]
        public void GetRemainingMilliseconds_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => TimeoutUtilities.GetRemainingMilliseconds(0, -2));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => TimeoutUtilities.GetRemainingMilliseconds(-2, null));
        }

        #endregion
    }
}
