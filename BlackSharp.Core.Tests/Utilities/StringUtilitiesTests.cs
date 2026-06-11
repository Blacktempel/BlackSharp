/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using BlackSharp.Core.Utilities;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class StringUtilitiesTests
    {
        [TestMethod]
        public void ToHexString()
        {
            Assert.AreEqual(string.Empty, StringUtilities.ToHexString([]));
            Assert.AreEqual("00",         StringUtilities.ToHexString([0x00]));
            Assert.AreEqual("FF",         StringUtilities.ToHexString([0xFF]));
            Assert.AreEqual("0A",         StringUtilities.ToHexString([0x0A]));
            Assert.AreEqual("000F10FFAB", StringUtilities.ToHexString([0x00, 0x0F, 0x10, 0xFF, 0xAB]));
        }

        [TestMethod]
        public void GetHexChar()
        {
            Assert.AreEqual('0', StringUtilities.GetHexChar(0));
            Assert.AreEqual('5', StringUtilities.GetHexChar(5));
            Assert.AreEqual('9', StringUtilities.GetHexChar(9));
            Assert.AreEqual('A', StringUtilities.GetHexChar(10));
            Assert.AreEqual('F', StringUtilities.GetHexChar(15));
        }
    }
}
