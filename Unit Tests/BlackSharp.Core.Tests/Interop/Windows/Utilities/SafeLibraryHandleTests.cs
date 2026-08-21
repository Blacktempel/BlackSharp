using BlackSharp.Core.Interop.Windows.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackSharp.Core.Tests.Interop.Windows.Utilities
{
    [TestClass]
    public class SafeLibraryHandleTests
    {
        #region Public

        [TestMethod]
        public void ConstructorsPreserveInvalidHandleState()
        {
            // Arrange
            using var defaultHandle = new SafeLibraryHandle();
            using var existingHandle = new SafeLibraryHandle(IntPtr.Zero, false);

            // Act
            bool defaultIsInvalid = defaultHandle.IsInvalid;
            bool existingIsInvalid = existingHandle.IsInvalid;

            // Assert
            Assert.IsTrue(defaultIsInvalid);
            Assert.IsTrue(existingIsInvalid);
        }

        #endregion
    }
}
