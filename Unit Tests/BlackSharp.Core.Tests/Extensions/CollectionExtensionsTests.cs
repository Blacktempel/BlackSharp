// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class CollectionExtensionsTests
    {
        #region Public

        [TestMethod]
        public void IndexOfReference()
        {
            var first = new string('A', 1);
            var second = new string('A', 1);
            IReadOnlyList<string> values = new[] { first };

            int sameReferenceIndex = values.IndexOfReference(first);
            int equalValueIndex = values.IndexOfReference(second);

            Assert.AreEqual(0, sameReferenceIndex);
            Assert.AreEqual(-1, equalValueIndex);
        }

        [TestMethod]
        public void TryGetFirst()
        {
            int[] values = { 1, 4, 9 };

            bool foundMatch = values.TryGetFirst(value => value % 2 == 0, out int match);
            bool foundMissing = values.TryGetFirst(value => value > 10, out int missing);

            Assert.IsTrue(foundMatch);
            Assert.AreEqual(4, match);
            Assert.IsFalse(foundMissing);
            Assert.AreEqual(0, missing);
        }

        #endregion
    }
}
