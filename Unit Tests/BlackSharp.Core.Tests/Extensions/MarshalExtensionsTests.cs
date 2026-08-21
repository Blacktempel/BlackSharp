/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using BlackSharp.Core.Extensions;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class MarshalExtensionsTests
    {
        [TestMethod]
        public void GetBytes_FromBytes()
        {
            var expected = new TestStructure
            {
                First  = 0x1234,
                Second = 0x89ABCDEF
            };

            byte[] buffer = MarshalExtensions.GetBytes(expected);
            TestStructure actual = MarshalExtensions.FromBytes<TestStructure>(buffer);

            Assert.HasCount(Marshal.SizeOf<TestStructure>(), buffer);
            Assert.AreEqual(expected.First , actual.First );
            Assert.AreEqual(expected.Second, actual.Second);
        }

        [TestMethod]
        public void CopyToBytes_FromBytes_WithOffset()
        {
            var expected = new TestStructure
            {
                First = 0x1234,
                Second = 0x89ABCDEF
            };
            int offset = 3;
            var buffer = new byte[offset + Marshal.SizeOf<TestStructure>() + 2];

            MarshalExtensions.CopyToBytes(expected, buffer, offset);
            TestStructure actual =
                MarshalExtensions.FromBytes<TestStructure>(buffer, offset);

            Assert.AreEqual(expected.First, actual.First);
            Assert.AreEqual(expected.Second, actual.Second);
            Assert.AreEqual(0, buffer[0]);
            Assert.AreEqual(0, buffer[buffer.Length - 1]);
        }

        [TestMethod]
        public void CopyToBytes_FromBytes_InvalidBufferRange()
        {
            int size = Marshal.SizeOf<TestStructure>();
            var shortBuffer = new byte[size - 1];
            var exactBuffer = new byte[size];

            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalExtensions.CopyToBytes(default(TestStructure), null, 0));
            Assert.ThrowsExactly<ArgumentNullException>(
                () => MarshalExtensions.FromBytes<TestStructure>(null, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => MarshalExtensions.CopyToBytes(default(TestStructure), exactBuffer, -1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => MarshalExtensions.FromBytes<TestStructure>(exactBuffer, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => MarshalExtensions.CopyToBytes(default(TestStructure), shortBuffer, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => MarshalExtensions.FromBytes<TestStructure>(shortBuffer, 0));
        }

        [TestMethod]
        public void Copy()
        {
            var size = 512;

            var source      = Marshal.AllocHGlobal(size);
            var destination = Marshal.AllocHGlobal(size);

            Assert.ThrowsExactly<ArgumentNullException>(() => MarshalExtensions.Copy(IntPtr.Zero, IntPtr.Zero, 0, 0));
            Assert.ThrowsExactly<ArgumentNullException>(() => MarshalExtensions.Copy(source     , IntPtr.Zero, 0, 0));
            Assert.ThrowsExactly<ArgumentNullException>(() => MarshalExtensions.Copy(IntPtr.Zero, destination, 0, 0));

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => MarshalExtensions.Copy(source, destination, -1 , -1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => MarshalExtensions.Copy(source, destination, -1 ,  5));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => MarshalExtensions.Copy(source, destination,  5 , -1));

            int index = 0;

            TestCopy(source, destination, index++, 123);
            TestCopy(source, destination, index++, 456);
            TestCopy(source, destination, index++, 789);

            Marshal.FreeHGlobal(destination);
            Marshal.FreeHGlobal(source);
        }

        [TestMethod]
        public void ReadUInt16()
        {
            ushort value = 60000;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<ushort>());

            Assert.AreNotEqual(IntPtr.Zero, ptr);

            MarshalExtensions.WriteUInt16(ptr, value);

            Assert.AreEqual(value, MarshalExtensions.ReadUInt16(ptr));

            Marshal.FreeHGlobal(ptr);
        }

        [TestMethod]
        public void WriteUInt16()
        {
            ushort value = 50000;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<ushort>());

            Assert.AreNotEqual(IntPtr.Zero, ptr);

            MarshalExtensions.WriteUInt16(ptr, value);

            Assert.AreEqual(value, MarshalExtensions.ReadUInt16(ptr));

            Marshal.FreeHGlobal(ptr);
        }

        [TestMethod]
        public void ReadUInt64()
        {
            ulong value = ulong.MaxValue - 10;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<ulong>());

            Assert.AreNotEqual(IntPtr.Zero, ptr);

            MarshalExtensions.WriteUInt64(ptr, value);

            Assert.AreEqual(value, MarshalExtensions.ReadUInt64(ptr));

            Marshal.FreeHGlobal(ptr);
        }

        [TestMethod]
        public void WriteUInt64()
        {
            ulong value = ulong.MaxValue - 1000;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<ulong>());

            Assert.AreNotEqual(IntPtr.Zero, ptr);

            MarshalExtensions.WriteUInt64(ptr, value);

            Assert.AreEqual(value, MarshalExtensions.ReadUInt64(ptr));

            Marshal.FreeHGlobal(ptr);
        }

        void TestCopy(IntPtr source, IntPtr destination, int index, int value)
        {
            Marshal.WriteInt32(source, index * 4, value);
            MarshalExtensions.Copy(source, destination, index * 4, 4);
            Assert.AreEqual(value, Marshal.ReadInt32(destination));
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TestStructure
        {
            public ushort First;
            public uint Second;
        }
    }
}
