/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

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

                Assert.IsTrue(str.Contains("Info:"));
                Assert.IsTrue(str.Contains(exceptionStr));

                Assert.IsTrue(str.Contains("Details:"));
                Assert.IsTrue(str.Contains(nameof(ExceptionExtensionsTests)));
                Assert.IsTrue(str.Contains(nameof(FullExceptionString)));
            }
        }
    }
}
