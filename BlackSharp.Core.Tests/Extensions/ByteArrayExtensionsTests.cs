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
        public void CopyRange()
        {
            // Arrange
            byte[] data = { 0x10, 0x20, 0x30, 0x40 };

            // Act
            byte[] result = data.CopyRange(1, 2);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x20, 0x30 }, result);
        }

        [TestMethod]
        public void CopyRange_ValidateInputRangeExceptions()
        {
            // Arrange
            byte[] data = { 0x10 };

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => ByteArrayExtensions.CopyRange(null, 0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => data.CopyRange(0, 2));
        }

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
            Assert.AreEqual(0x0189ABCDEF123480UL, data.ToUInt64LittleEndian(1));
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

        [TestMethod]
        public void TryConvertLittleEndianIntegers()
        {
            byte[] data = { 0x80, 0x34, 0x12, 0xEF, 0xCD, 0xAB, 0x89, 0x01 };

            Assert.IsTrue(data.TryToUInt16LittleEndian(0, out ushort value16));
            Assert.AreEqual((ushort)0x3480, value16);
            Assert.IsTrue(data.TryToUInt32LittleEndian(1, out uint value32));
            Assert.AreEqual(0xCDEF1234U, value32);
            Assert.IsTrue(data.TryToUInt64LittleEndian(0, out ulong value64));
            Assert.AreEqual(0x0189ABCDEF123480UL, value64);

            Assert.IsFalse(data.TryToUInt16LittleEndian(7, out _));
            Assert.IsFalse(ByteArrayExtensions.TryToUInt32LittleEndian(null, 0, out _));
            Assert.IsFalse(data.TryToUInt64LittleEndian(-1, out _));
        }

        [TestMethod]
        public void WriteEndianIntegers()
        {
            // Arrange
            var data = new byte[35];

            // Act
            data.WriteInt16LittleEndian(1, unchecked((short)0xABCD));
            data.WriteUInt16LittleEndian(3, 0x1234);
            data.WriteUInt32LittleEndian(5, 0x89ABCDEF);
            data.WriteUInt64LittleEndian(9, 0x0123456789ABCDEF);
            data.WriteUInt16BigEndian(17, 0x1234);
            data.WriteUInt32BigEndian(19, 0x89ABCDEF);
            data.WriteUInt64BigEndian(23, 0x0123456789ABCDEF);
            data.WriteSingleLittleEndian(31, 1.0F);

            // Assert
            CollectionAssert.AreEqual(
                new byte[]
                {
                    0x00,
                    0xCD,
                    0xAB,
                    0x34,
                    0x12,
                    0xEF,
                    0xCD,
                    0xAB,
                    0x89,
                    0xEF,
                    0xCD,
                    0xAB,
                    0x89,
                    0x67,
                    0x45,
                    0x23,
                    0x01,
                    0x12,
                    0x34,
                    0x89,
                    0xAB,
                    0xCD,
                    0xEF,
                    0x01,
                    0x23,
                    0x45,
                    0x67,
                    0x89,
                    0xAB,
                    0xCD,
                    0xEF,
                    0x00,
                    0x00,
                    0x80,
                    0x3F,
                },
                data);
        }

        [TestMethod]
        public void ConvertSignedBigEndianInteger()
        {
            // Arrange
            var data = new byte[2];

            // Act
            data.WriteInt16BigEndian(0, unchecked((short)0xABCD));
            short result = data.ToInt16BigEndian(0);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0xAB, 0xCD }, data);
            Assert.AreEqual(unchecked((short)0xABCD), result);
        }

        [TestMethod]
        public void ConvertUInt24BigEndianInteger()
        {
            // Arrange
            byte[] data = { 0x00, 0xAB, 0xCD, 0xEF, 0x00 };

            // Act
            uint result = data.ToUInt24BigEndian(1);

            // Assert
            Assert.AreEqual(0x00ABCDEFU, result);
        }

        [TestMethod]
        public void WriteUInt48LittleEndianTruncatesHigherBits()
        {
            // Arrange
            var data = new byte[6];

            // Act
            data.WriteUInt48LittleEndian(0, 0xFEDCBA9876543210);

            // Assert
            CollectionAssert.AreEqual(
                new byte[] { 0x10, 0x32, 0x54, 0x76, 0x98, 0xBA },
                data);
        }

        [TestMethod]
        public void WriteEndianIntegers_ValidateInputRangeExceptions()
        {
            // Arrange
            byte[] data = { 0x00 };

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => ByteArrayExtensions.WriteUInt16LittleEndian(null, 0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => data.WriteUInt64BigEndian(0, 0));
        }
    }
}
