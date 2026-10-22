// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Linux;

namespace BlackSharp.Core.Tests.Interop
{
    [TestClass]
    public class LinuxPathUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void SearchUpwardsForEntry()
        {
            string root   = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string nested = Path.Combine(root, "one", "two");
            string marker = Path.Combine(root, "marker");

            try
            {
                Directory.CreateDirectory(nested);
                File.WriteAllText(marker, string.Empty);

                Assert.AreEqual(marker, LinuxPathUtilities.SearchUpwardsForEntry(nested, "marker"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        #endregion
    }
}
