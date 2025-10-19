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
