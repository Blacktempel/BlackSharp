/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Utilities;
using System.Text;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class StringBuilderUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void AppendLine()
        {
            var builder = new StringBuilder();

            StringBuilderUtilities.AppendLine(builder, 2, "Value");

            Assert.AreEqual("    Value" + Environment.NewLine, builder.ToString());
        }

        #endregion
    }
}
