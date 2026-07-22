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
    public class ByteArrayExtensionsTests
    {
        [TestMethod]
        public void ReadNullTerminatedASCIIString()
        {
            byte[] data = { 0x20, 0x41, 0x42, 0x20, 0x00, 0x43 };

            Assert.AreEqual("AB", data.ReadNullTerminatedASCIIString(0, data.Length));
            Assert.AreEqual("AB", data.ReadNullTerminatedASCIIString(1, 2));
            Assert.AreEqual("C", data.ReadNullTerminatedASCIIString(5, 10));
        }

        [TestMethod]
        public void ToSingleLittleEndian()
        {
            byte[] data = { 0xFF, 0x00, 0x00, 0x80, 0x3F, 0xFF };

            Assert.AreEqual(1F, data.ToSingleLittleEndian(1));
        }

        [TestMethod]
        public void ConvertEndianIntegers()
        {
            byte[] data = { 0xFF, 0x80, 0x34, 0x12, 0xEF, 0xCD, 0xAB, 0x89, 0x01 };

            Assert.AreEqual((ushort)0x3480, data.ToUInt16LittleEndian(1));
            Assert.AreEqual(unchecked((short)0x80FF), data.ToInt16LittleEndian(0));
            Assert.AreEqual((ushort)0x8034, data.ToUInt16BigEndian(1));
            Assert.AreEqual(unchecked((int)0xABCDEF12), data.ToInt32LittleEndian(3));
            Assert.AreEqual(0xABCDEF12U, data.ToUInt32LittleEndian(3));
            Assert.AreEqual(0x89ABCDEF1234UL, data.ToUInt48LittleEndian(2));
            Assert.AreEqual(0x803412EFU, data.ToUInt32BigEndian(1));
            Assert.AreEqual(0x803412EFCDAB8901UL, data.ToUInt64BigEndian(1));
        }

        [TestMethod]
        public void ConvertEndianIntegers_ValidateInputRangeExceptions()
        {
            byte[] data = { 0x01 };

            Assert.ThrowsExactly<ArgumentNullException>(
                () => ByteArrayExtensions.ToUInt16LittleEndian(null, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => data.ToUInt16LittleEndian(0));
        }
    }
}
