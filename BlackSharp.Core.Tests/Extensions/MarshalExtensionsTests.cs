/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class MarshalExtensionsTests
    {
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
    }
}
