/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Tests.Interop
{
    [TestClass]
    public class MarshalUtilitiesTests
    {
        #region Public

        [TestMethod]
        public void InvokeStructure()
        {
            var value = new TestValue { Value = 1 };

            var result = MarshalUtilities.Invoke(ref value, pointer =>
            {
                Marshal.WriteInt32(pointer, 42);

                return 7;
            });

            Assert.AreEqual(7, result);
            Assert.AreEqual(42, value.Value);
        }

        [TestMethod]
        public void InvokeStructureArray()
        {
            var values = new[]
            {
                new TestValue { Value = 1 },
                new TestValue { Value = 2 },
            };

            var result = MarshalUtilities.Invoke(values, pointer =>
            {
                Marshal.WriteInt32(pointer, 0, 10);
                Marshal.WriteInt32(pointer, Marshal.SizeOf<TestValue>(), 20);

                return 30;
            });

            Assert.AreEqual(30, result);
            Assert.AreEqual(10, values[0].Value);
            Assert.AreEqual(20, values[1].Value);
        }

        [TestMethod]
        public void InvokePinned()
        {
            var first  = new byte[] { 1 };
            var second = new byte[] { 2 };

            var result = MarshalUtilities.InvokePinned(first, second, (firstPointer, secondPointer) =>
            {
                Marshal.WriteByte(firstPointer, 3);
                Marshal.WriteByte(secondPointer, 4);

                return 5;
            });

            Assert.AreEqual(5, result);
            Assert.AreEqual(3, first[0]);
            Assert.AreEqual(4, second[0]);
        }

        [TestMethod]
        public void Invoke_ThrowsArgumentNullException()
        {
            var value = new TestValue();

            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalUtilities.Invoke<TestValue, int>(ref value, null));
            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalUtilities.Invoke<TestValue, int>(null, _ => 0));
            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalUtilities.Invoke<TestValue, int>(new[] { value }, null));
            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalUtilities.InvokePinned<byte, byte, int>(null, Array.Empty<byte>(), (_, _) => 0));
            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalUtilities.InvokePinned<byte, byte, int>(Array.Empty<byte>(), null, (_, _) => 0));
            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalUtilities.InvokePinned<byte, byte, int>(Array.Empty<byte>(), Array.Empty<byte>(), null));
        }

        #endregion

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        struct TestValue
        {
            public int Value;
        }

        #endregion
    }
}
