// SPDX-License-Identifier: MPL-2.0

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
