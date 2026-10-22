// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class ExceptionExtensionsTests
    {
        [TestMethod]
        public void FullExceptionString()
        {
            var exceptionStr = "Some exception message.";

            try
            {
                throw new Exception(exceptionStr);
            }
            catch (Exception e)
            {
                var str = ExceptionExtensions.FullExceptionString(e);

                Assert.Contains("Info:"     , str);
                Assert.Contains(exceptionStr, str);

                Assert.Contains("Details:"                      , str);
                Assert.Contains(nameof(ExceptionExtensionsTests), str);
                Assert.Contains(nameof(FullExceptionString)     , str);
            }
        }
    }
}
