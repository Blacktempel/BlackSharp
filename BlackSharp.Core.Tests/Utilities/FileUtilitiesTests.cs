/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.IO;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class FileUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void ReadAllTextOrEmpty()
        {
            string path = Path.GetTempFileName();

            try
            {
                File.WriteAllText(path, "  value  ");

                string content        = FileUtilities.ReadAllTextOrEmpty(path);
                string missingContent = FileUtilities.ReadAllTextOrEmpty(path + ".missing");

                Assert.AreEqual("value", content);
                Assert.AreEqual(string.Empty, missingContent);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void WriteAllTextAtomic()
        {
            string path = Path.GetTempFileName();

            try
            {
                FileUtilities.WriteAllTextAtomic(path, "first");
                FileUtilities.WriteAllTextAtomic(path, "second");

                Assert.AreEqual("second", File.ReadAllText(path));
                Assert.AreEqual(0, Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path) + ".*.tmp").Length);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void EnumerateOrEmpty()
        {
            string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            Assert.AreEqual(0, FileUtilities.GetFilesOrEmpty(directory).Length);
            Assert.AreEqual(0, FileUtilities.GetDirectoriesOrEmpty(directory).Length);
        }

        #endregion
    }
}
