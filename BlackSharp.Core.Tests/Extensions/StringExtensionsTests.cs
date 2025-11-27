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
    public class StringExtensionsTests
    {
        [TestMethod]
        public void Contains()
        {
            const string TestString = "This is a CoMpLeX test string.";

            Assert.IsTrue(StringExtensions.Contains(TestString, "complex", StringComparison.OrdinalIgnoreCase));

            Assert.IsFalse(StringExtensions.Contains(TestString, "complex", StringComparison.Ordinal));

            Assert.IsFalse(StringExtensions.Contains(null      , "complex", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(StringExtensions.Contains(TestString, null     , StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(StringExtensions.Contains(null      , null     , StringComparison.OrdinalIgnoreCase));

            Assert.IsFalse(StringExtensions.Contains(string.Empty, null        , StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(StringExtensions.Contains(null        , string.Empty, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue (StringExtensions.Contains(string.Empty, string.Empty, StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void Join()
        {
            const int Elem0 =   123;
            const int Elem1 =   789;
            const int Elem2 = 42950;

            string Separator = ";";

            var list = new List<int> { Elem0, Elem1, Elem2 };

            var result0 = StringExtensions.Join(Separator, list);
            var result1 = StringExtensions.Join(Separator, Elem0, Elem1, Elem2);

            Assert.AreEqual($"{Elem0}{Separator}{Elem1}{Separator}{Elem2}", result0);
            Assert.AreEqual($"{Elem0}{Separator}{Elem1}{Separator}{Elem2}", result1);
        }
    }
}
