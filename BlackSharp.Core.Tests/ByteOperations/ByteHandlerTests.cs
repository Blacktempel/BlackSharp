/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.ByteOperations;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class ByteHandlerTests
    {
        [TestMethod]
        public void NormalizeBcd()
        {
            const byte bcd = 0x42;
            const byte expected = 42;

            Assert.AreEqual(expected, BinaryHandler.NormalizeBcd(bcd));
        }
    }
}
