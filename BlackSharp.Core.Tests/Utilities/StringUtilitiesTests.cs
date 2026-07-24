/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
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

        [TestMethod]
        public void FormatHex()
        {
            Assert.AreEqual("0x0A", StringUtilities.FormatHex((byte)0x0A));
            Assert.AreEqual("0x1234", StringUtilities.FormatHex((ushort)0x1234));
            Assert.AreEqual("0x89ABCDEF", StringUtilities.FormatHex(0x89ABCDEFU));
            Assert.AreEqual("0x0000000000001234", StringUtilities.FormatHex(0x1234UL, 8));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => StringUtilities.FormatHex(0UL, 0));
        }

        [TestMethod]
        public void FormatNullableInvariant()
        {
            string formatted = StringUtilities.FormatNullableInvariant<double>(12.5, "F2", "none");
            string fallback  = StringUtilities.FormatNullableInvariant<double>(null, "F2", "none");

            Assert.AreEqual("12.50", formatted);
            Assert.AreEqual("none", fallback);
        }

        [TestMethod]
        public void FormatNullOrEmpty()
        {
            Assert.AreEqual("value", StringUtilities.FormatNullOrEmpty("value", "null", "empty"));
            Assert.AreEqual("null" , StringUtilities.FormatNullOrEmpty(null, "null", "empty"));
            Assert.AreEqual("empty", StringUtilities.FormatNullOrEmpty(string.Empty, "null", "empty"));
            Assert.AreEqual(string.Empty, StringUtilities.FormatNullOrEmpty(null, null, "empty"));
            Assert.AreEqual(string.Empty, StringUtilities.FormatNullOrEmpty(string.Empty, "null", null));
        }

        [TestMethod]
        public void TrimNullPadding()
        {
            Assert.AreEqual("value", StringUtilities.TrimNullPadding("  \0value  \0\0  "));
            Assert.AreEqual(string.Empty, StringUtilities.TrimNullPadding(null));
        }

        [TestMethod]
        public void SplitLines()
        {
            // Arrange
            const string value = "first\r\nsecond\nthird\rfourth\r\n";

            // Act
            string[] lines = StringUtilities.SplitLines(value);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "first", "second", "third", "fourth", string.Empty },
                lines);
            CollectionAssert.AreEqual(
                new[] { string.Empty },
                StringUtilities.SplitLines(null));
        }
    }
}
