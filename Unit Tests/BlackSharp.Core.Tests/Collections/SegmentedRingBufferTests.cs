/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Collections;

namespace BlackSharp.Core.Tests.Collections
{
    /// <summary>
    /// Verifies ordering and trimming across segment boundaries.
    /// </summary>
    [TestClass]
    public sealed class SegmentedRingBufferTests
    {
        #region Public

        /// <summary>
        /// Verifies that trimming retains newest values in insertion order.
        /// </summary>
        [TestMethod]
        public void TrimmingAcrossSegmentsRetainsNewestValues()
        {
            // Arrange
            var buffer = new SegmentedRingBuffer<int>(2);

            foreach (var value in Enumerable.Range(1, 7))
            {
                buffer.Add(value);
            }

            // Act
            buffer.TrimWhile(value => value < 3);
            buffer.TrimToCount(3);

            var values  = buffer.ToArray();
            var hasLast = buffer.TryGetLast(out var last);

            // Assert
            CollectionAssert.AreEqual(new[] { 5, 6, 7 }, values);
            Assert.AreEqual(3, buffer.Count);

            Assert.IsTrue(hasLast);

            Assert.AreEqual(7, last);
        }

        /// <summary>
        /// Verifies that a fully trimmed buffer can be reused without stale entries.
        /// </summary>
        [TestMethod]
        public void EmptyBufferCanBeReused()
        {
            // Arrange
            var buffer = new SegmentedRingBuffer<string>(2);
            buffer.Add("old");

            // Act
            buffer.TrimToCount(0);

            var hadLast = buffer.TryGetLast(out _);
            buffer.Add("new");

            // Assert
            Assert.IsFalse(hadLast);

            CollectionAssert.AreEqual(new[] { "new" }, buffer.ToArray());
        }

        /// <summary>
        /// Verifies that invalid segment capacities and trim limits are rejected.
        /// </summary>
        [TestMethod]
        public void InvalidArgumentsAreRejected()
        {
            // Arrange
            var buffer = new SegmentedRingBuffer<int>();

            // Act and Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SegmentedRingBuffer<int>(0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.TrimToCount(-1));
            Assert.ThrowsExactly<ArgumentNullException>(() => buffer.TrimWhile(null));
        }

        #endregion
    }
}
