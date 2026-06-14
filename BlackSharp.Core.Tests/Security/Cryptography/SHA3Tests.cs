/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Security.Cryptography;
using BlackSharp.Core.Utilities;
using System.Text;

namespace BlackSharp.Core.Tests.Security.Cryptography
{
    [TestClass]
    public class SHA3Tests
    {
        #region Constants

        const string Text = "The quick brown fox jumps over the lazy dog.";

        const string SHA3_224_Hash = "2d0708903833afabdd232a20201176e8b58c5be8a6fe74265ac54db0";
        const string SHA3_256_Hash = "a80f839cd4f83f6c3dafc87feae470045e4eb0d366397d5c6ce34ba1739f734d";
        const string SHA3_384_Hash = "1a34d81695b622df178bc74df7124fe12fac0f64ba5250b78b99c1273d4b080168e10652894ecad5f1f4d5b965437fb9";
        const string SHA3_512_Hash = "18f4f4bd419603f95538837003d9d254c26c23765565162247483f65c50303597bc9ce4d289f21d1c2f1f458828e33dc442100331b35e7eb031b5d38ba6460f8";

        #endregion

        #region Public

        [TestMethod]
        public void SHA3_Hashes()
        {
            var hash224 = GetHash<SHA3_224>();
            var hash256 = GetHash<SHA3_256>();
            var hash384 = GetHash<SHA3_384>();
            var hash512 = GetHash<SHA3_512>();

            Assert.AreEqual(SHA3_224_Hash, hash224, StringComparer.OrdinalIgnoreCase);
            Assert.AreEqual(SHA3_256_Hash, hash256, StringComparer.OrdinalIgnoreCase);
            Assert.AreEqual(SHA3_384_Hash, hash384, StringComparer.OrdinalIgnoreCase);
            Assert.AreEqual(SHA3_512_Hash, hash512, StringComparer.OrdinalIgnoreCase);
        }

        [TestMethod]
        public void SHA3_Create_Default()
        {
            Assert.AreSame(typeof(SHA3_512), SHA3.Create()?.GetType());
        }

        [TestMethod]
        public void SHA3_Create_All()
        {
            Assert.AreSame(typeof(SHA3_224), SHA3.Create(nameof(SHA3_224))?.GetType());
            Assert.AreSame(typeof(SHA3_256), SHA3.Create(nameof(SHA3_256))?.GetType());
            Assert.AreSame(typeof(SHA3_384), SHA3.Create(nameof(SHA3_384))?.GetType());
            Assert.AreSame(typeof(SHA3_512), SHA3.Create(nameof(SHA3_512))?.GetType());
        }

        #endregion

        #region Private

        string GetHash<T>()
            where T : SHA3, new()
        {
            var raw = Encoding.UTF8.GetBytes(Text);
            var sha = new T();

            sha.ComputeHash(raw);

            return StringUtilities.ToHexString(sha.Hash);
        }

        #endregion
    }
}
