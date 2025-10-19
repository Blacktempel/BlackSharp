/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class BasicTypeExtensionsTests
    {
        [TestMethod]
        public void Between()
        {
            byte  b = 1;
            short s = 30000;
            int   i = 500000;
            long  l = 1000000000000;

            float   f  = 123.45f;
            double  d  = 12345678.90d;
            decimal dc = 1234567890M;

            DateTime dt = new DateTime(2025, 10, 1);

            Assert.IsTrue(BasicTypeExtensions.Between(b, byte .MinValue, byte .MaxValue));
            Assert.IsTrue(BasicTypeExtensions.Between(s, short.MinValue, short.MaxValue));
            Assert.IsTrue(BasicTypeExtensions.Between(i, int  .MinValue, int  .MaxValue));
            Assert.IsTrue(BasicTypeExtensions.Between(l, long .MinValue, long .MaxValue));

            Assert.IsTrue(BasicTypeExtensions.Between(f , float  .MinValue, float  .MaxValue));
            Assert.IsTrue(BasicTypeExtensions.Between(d , double .MinValue, double .MaxValue));
            Assert.IsTrue(BasicTypeExtensions.Between(dc, decimal.MinValue, decimal.MaxValue));

            Assert.IsTrue(BasicTypeExtensions.Between(dt, new DateTime(2025, 9, 30), new DateTime(2025, 10, 2)));

        }
    }
}
