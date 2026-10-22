// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;

namespace BlackSharp.Core.Tests.Extensions
{
    class CPU { }
    class Intel : CPU { }
    class AMD : CPU { }

    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void IsTypeOrSubclassOf()
        {
            Assert.IsTrue(TypeExtensions.IsTypeOrSubclassOf(typeof(Intel), typeof(CPU)));
            Assert.IsTrue(TypeExtensions.IsTypeOrSubclassOf(typeof(AMD  ), typeof(CPU)));
            Assert.IsTrue(TypeExtensions.IsTypeOrSubclassOf(typeof(CPU  ), typeof(CPU)));

            Assert.IsFalse(TypeExtensions.IsTypeOrSubclassOf(typeof(CPU), typeof(Intel)));
        }
    }
}
