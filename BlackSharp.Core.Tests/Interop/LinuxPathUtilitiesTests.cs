/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

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
