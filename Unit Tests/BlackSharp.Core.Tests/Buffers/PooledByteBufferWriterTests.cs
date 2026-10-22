/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Buffers;

namespace BlackSharp.Core.Tests.Buffers
{
    /// <summary>
    /// Verifies growth, committed content and disposal of the pooled byte writer.
    /// </summary>
    [TestClass]
    public sealed class PooledByteBufferWriterTests
    {
        #region Public

        /// <summary>
        /// Verifies that growth preserves committed bytes and exposes only written content.
        /// </summary>
        [TestMethod]
        public void GrowthPreservesCommittedBytes()
        {
            // Arrange
            using var writer = new PooledByteBufferWriter(1);
            writer.GetSpan(1)[0] = 0x12;
            writer.Advance(1);

            // Act
            new byte[] { 0x34, 0x56, 0x78 }.CopyTo(writer.GetSpan(3));
            writer.Advance(3);

            // Assert
            Assert.AreEqual(4, writer.WrittenCount);

            CollectionAssert.AreEqual(new byte[] { 0x12, 0x34, 0x56, 0x78 }, writer.WrittenMemory.ToArray());
        }

        /// <summary>
        /// Verifies that invalid advances and access after disposal are rejected.
        /// </summary>
        [TestMethod]
        public void InvalidOperationsAreRejected()
        {
            // Arrange
            var writer = new PooledByteBufferWriter(1);

            // Act and Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.Advance(-1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.Advance(writer.GetSpan().Length + 1));

            writer.Dispose();
            Assert.ThrowsExactly<ObjectDisposedException>(() => writer.GetMemory());
        }

        #endregion
    }
}
