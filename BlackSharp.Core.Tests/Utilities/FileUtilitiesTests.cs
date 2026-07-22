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

        #endregion
    }
}
