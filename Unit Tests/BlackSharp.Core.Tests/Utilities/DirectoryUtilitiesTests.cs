// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.IO;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class DirectoryUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void Copy()
        {
            string source = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string target = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            try
            {
                Directory.CreateDirectory(Path.Combine(source, "nested"));
                File.WriteAllText(Path.Combine(source, "nested", "value.txt"), "value");

                DirectoryUtilities.Copy(source, target);

                Assert.AreEqual("value", File.ReadAllText(Path.Combine(target, "nested", "value.txt")));
            }
            finally
            {
                Directory.Delete(source, true);

                if (Directory.Exists(target))
                {
                    Directory.Delete(target, true);
                }
            }
        }

        [TestMethod]
        public void Copy_ThrowsArgumentExceptionForTargetInsideSource()
        {
            string source = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string target = Path.Combine(source, "nested");

            try
            {
                Directory.CreateDirectory(source);

                Assert.ThrowsExactly<ArgumentException>(() => DirectoryUtilities.Copy(source, target));
            }
            finally
            {
                if (Directory.Exists(source))
                {
                    Directory.Delete(source, true);
                }
            }
        }

        #endregion
    }
}
