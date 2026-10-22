// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class EventHandlerExtensionsTests
    {
        #region Public

        [TestMethod]
        public void InvokeAllSafely()
        {
            var invocationCount = 0;
            EventHandler handlers = (_, _) => throw new InvalidOperationException();
            handlers += (_, _) => ++invocationCount;

            handlers.InvokeAllSafely(this, EventArgs.Empty);

            Assert.AreEqual(1, invocationCount);
        }

        [TestMethod]
        public void InvokeAllSafely_Generic()
        {
            var invocationCount = 0;
            EventHandler<EventArgs> handlers = (_, _) => throw new InvalidOperationException();
            handlers += (_, _) => ++invocationCount;

            handlers.InvokeAllSafely(this, EventArgs.Empty);

            Assert.AreEqual(1, invocationCount);
        }

        #endregion
    }
}
