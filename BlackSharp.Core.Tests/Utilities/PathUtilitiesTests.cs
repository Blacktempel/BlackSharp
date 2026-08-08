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
