// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.IO;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class PathUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void IsUnderDirectory()
        {
            string directory = Path.Combine(Path.GetTempPath(), "root");
            string child     = Path.Combine(directory, "child", "value.txt");
            string sibling   = directory + "-other";

            Assert.IsTrue(PathUtilities.IsUnderDirectory(child, directory));
            Assert.IsFalse(PathUtilities.IsUnderDirectory(directory, directory));
            Assert.IsTrue(PathUtilities.IsUnderDirectory(directory, directory, includeDirectory: true));
            Assert.IsFalse(PathUtilities.IsUnderDirectory(sibling, directory));
        }

        #endregion
    }
}
