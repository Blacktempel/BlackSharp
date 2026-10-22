// SPDX-License-Identifier: MPL-2.0

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
