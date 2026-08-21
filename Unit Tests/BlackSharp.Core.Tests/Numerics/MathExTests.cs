/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

#pragma warning disable MSTEST0032 // Review or remove the assertion as its condition is known to be always true

using BlackSharp.Core.Numerics;

namespace BlackSharp.Core.Tests.Numerics
{
    [TestClass]
    public class MathExTests
    {
        #region Public

        [TestMethod]
        public void Abs()
        {
            Assert.AreEqual(new IntPtr(42), MathEx.Abs(new IntPtr(-42)));
            Assert.AreEqual(IntPtr.Zero, MathEx.Abs(IntPtr.Zero));
        }

        [TestMethod]
        public void Acosh()
        {
            Assert.AreEqual(0.0, MathEx.Acosh(1.0));
            Assert.AreEqual(2.993222846126381, MathEx.Acosh(10.0), 0.000000000000001);
            Assert.IsTrue(double.IsNaN(MathEx.Acosh(0.5)));
            Assert.AreEqual(double.PositiveInfinity, MathEx.Acosh(double.PositiveInfinity));
        }

        [TestMethod]
        public void Asinh()
        {
            Assert.AreEqual(0.0, MathEx.Asinh(0.0));
            Assert.AreEqual(-2.99822295029797, MathEx.Asinh(-10.0), 0.000000000000001);
            Assert.AreEqual(double.PositiveInfinity, MathEx.Asinh(double.PositiveInfinity));
            Assert.IsTrue(double.IsNaN(MathEx.Asinh(double.NaN)));
        }

        [TestMethod]
        public void Atanh()
        {
            Assert.AreEqual(0.0, MathEx.Atanh(0.0));
            Assert.AreEqual(0.5493061443340548, MathEx.Atanh(0.5), 0.000000000000001);
            Assert.AreEqual(double.PositiveInfinity, MathEx.Atanh(1.0));
            Assert.AreEqual(double.NegativeInfinity, MathEx.Atanh(-1.0));
            Assert.IsTrue(double.IsNaN(MathEx.Atanh(2.0)));
        }

        [TestMethod]
        public void BigMul()
        {
            Assert.AreEqual(18446744065119617025UL, MathEx.BigMul(uint.MaxValue, uint.MaxValue));

            var highSigned = MathEx.BigMul(1234567890123456789L, -987654321098765432L, out var lowSigned);
            Assert.AreEqual(-66099811787816347L, highSigned);
            Assert.AreEqual(8792095257026148904L, lowSigned);

            var highUnsigned = MathEx.BigMul(0xFEDCBA9876543210UL, 0x123456789ABCDEF0UL, out var lowUnsigned);
            Assert.AreEqual(0x121FA00AD77D7422UL, highUnsigned);
            Assert.AreEqual(0x236D88FE5618CF00UL, lowUnsigned);

#if NET8_0_OR_GREATER
            Assert.AreEqual((Int128)long.MaxValue * (Int128)2, MathEx.BigMul(long.MaxValue, 2L));
            Assert.AreEqual((UInt128)ulong.MaxValue * (UInt128)2, MathEx.BigMul(ulong.MaxValue, 2UL));
#endif
        }

        [TestMethod]
        public void BitDecrement()
        {
            Assert.AreEqual(0.9999999999999999, MathEx.BitDecrement(1.0));
            Assert.AreEqual(-double.Epsilon, MathEx.BitDecrement(0.0));
            Assert.AreEqual(double.NegativeInfinity, MathEx.BitDecrement(double.NegativeInfinity));
            Assert.IsTrue(double.IsNaN(MathEx.BitDecrement(double.NaN)));
        }

        [TestMethod]
        public void BitIncrement()
        {
            Assert.AreEqual(1.0000000000000002, MathEx.BitIncrement(1.0));
            Assert.AreEqual(double.Epsilon, MathEx.BitIncrement(0.0));
            Assert.AreEqual(double.PositiveInfinity, MathEx.BitIncrement(double.PositiveInfinity));
            Assert.IsTrue(double.IsNaN(MathEx.BitIncrement(double.NaN)));
        }

        [TestMethod]
        public void Cbrt()
        {
            Assert.AreEqual(3.0, MathEx.Cbrt(27.0), 0.000000000000001);
            Assert.AreEqual(-3.0, MathEx.Cbrt(-27.0), 0.000000000000001);
            Assert.AreEqual(double.PositiveInfinity, MathEx.Cbrt(double.PositiveInfinity));
            Assert.IsTrue(double.IsNaN(MathEx.Cbrt(double.NaN)));
        }

        [TestMethod]
        public void Clamp()
        {
            Assert.AreEqual((byte)10, MathEx.Clamp((byte)20, (byte)0, (byte)10));
            Assert.AreEqual(5M, MathEx.Clamp(5M, 0M, 10M));
            Assert.AreEqual(0.0, MathEx.Clamp(-1.0, 0.0, 10.0));
            Assert.AreEqual((short)10, MathEx.Clamp((short)20, (short)0, (short)10));
            Assert.AreEqual(10, MathEx.Clamp(20, 0, 10));
            Assert.AreEqual(0L, MathEx.Clamp(-1L, 0L, 10L));
            Assert.AreEqual(new IntPtr(10), MathEx.Clamp(new IntPtr(20), IntPtr.Zero, new IntPtr(10)));
            Assert.AreEqual((sbyte)0, MathEx.Clamp((sbyte)-1, (sbyte)0, (sbyte)10));
            Assert.AreEqual(10F, MathEx.Clamp(20F, 0F, 10F));
            Assert.AreEqual((ushort)10, MathEx.Clamp((ushort)20, (ushort)0, (ushort)10));
            Assert.AreEqual(0U, MathEx.Clamp(0U, 0U, 10U));
            Assert.AreEqual(10UL, MathEx.Clamp(20UL, 0UL, 10UL));
            Assert.AreEqual(new UIntPtr(10U), MathEx.Clamp(new UIntPtr(20U), UIntPtr.Zero, new UIntPtr(10U)));

            Assert.ThrowsExactly<ArgumentException>(() => MathEx.Clamp(5, 10, 0));
        }

        [TestMethod]
        public void CopySign()
        {
            Assert.AreEqual(-4.0, MathEx.CopySign(4.0, -1.0));
            Assert.AreEqual(4.0, MathEx.CopySign(-4.0, 1.0));
            Assert.IsTrue(IsNegativeZero(MathEx.CopySign(0.0, -0.0)));
            Assert.IsFalse(IsNegativeZero(MathEx.CopySign(-0.0, 0.0)));
        }

        [TestMethod]
        public void DivRem()
        {
            var byteResult = MathEx.DivRem((byte)10, (byte)3);
            Assert.AreEqual((byte)3, byteResult.Quotient);
            Assert.AreEqual((byte)1, byteResult.Remainder);

            var shortResult = MathEx.DivRem((short)-10, (short)3);
            Assert.AreEqual((short)-3, shortResult.Quotient);
            Assert.AreEqual((short)-1, shortResult.Remainder);

            var intResult = MathEx.DivRem(-10, 3);
            Assert.AreEqual(-3, intResult.Quotient);
            Assert.AreEqual(-1, intResult.Remainder);

            var longResult = MathEx.DivRem(-10L, 3L);
            Assert.AreEqual(-3L, longResult.Quotient);
            Assert.AreEqual(-1L, longResult.Remainder);

            var intPtrResult = MathEx.DivRem(new IntPtr(-10), new IntPtr(3));
            Assert.AreEqual(new IntPtr(-3), intPtrResult.Quotient);
            Assert.AreEqual(new IntPtr(-1), intPtrResult.Remainder);

            var sbyteResult = MathEx.DivRem((sbyte)-10, (sbyte)3);
            Assert.AreEqual((sbyte)-3, sbyteResult.Quotient);
            Assert.AreEqual((sbyte)-1, sbyteResult.Remainder);

            var ushortResult = MathEx.DivRem((ushort)10, (ushort)3);
            Assert.AreEqual((ushort)3, ushortResult.Quotient);
            Assert.AreEqual((ushort)1, ushortResult.Remainder);

            var uintResult = MathEx.DivRem(10U, 3U);
            Assert.AreEqual(3U, uintResult.Quotient);
            Assert.AreEqual(1U, uintResult.Remainder);

            var ulongResult = MathEx.DivRem(10UL, 3UL);
            Assert.AreEqual(3UL, ulongResult.Quotient);
            Assert.AreEqual(1UL, ulongResult.Remainder);

            var uintPtrResult = MathEx.DivRem(new UIntPtr(10U), new UIntPtr(3U));
            Assert.AreEqual(new UIntPtr(3U), uintPtrResult.Quotient);
            Assert.AreEqual(new UIntPtr(1U), uintPtrResult.Remainder);
        }

        [TestMethod]
        public void FusedMultiplyAdd()
        {
            Assert.AreEqual(10.0, MathEx.FusedMultiplyAdd(2.0, 3.0, 4.0));
        }

        [TestMethod]
        public void ILogB()
        {
            Assert.AreEqual(4, MathEx.ILogB(16.0));
            Assert.AreEqual(int.MinValue, MathEx.ILogB(0.0));
            Assert.AreEqual(int.MaxValue, MathEx.ILogB(double.PositiveInfinity));
            Assert.AreEqual(int.MaxValue, MathEx.ILogB(double.NaN));
        }

        [TestMethod]
        public void Log2()
        {
            Assert.AreEqual(10.0, MathEx.Log2(1024.0));
            Assert.AreEqual(0.0, MathEx.Log2(1.0));
        }

        [TestMethod]
        public void Max()
        {
            Assert.AreEqual(new IntPtr(10), MathEx.Max(new IntPtr(-5), new IntPtr(10)));
            Assert.AreEqual(new UIntPtr(10U), MathEx.Max(new UIntPtr(5U), new UIntPtr(10U)));
            Assert.AreEqual(10.0, MathEx.Max(5.0, 10.0));
            Assert.AreEqual(5.0, MathEx.Max(5.0, null));

            Assert.IsNull(MathEx.Max(null, null));
        }

        [TestMethod]
        public void MaxMagnitude()
        {
            Assert.AreEqual(-10.0, MathEx.MaxMagnitude(-10.0, 5.0));
            Assert.AreEqual(10.0, MathEx.MaxMagnitude(-10.0, 10.0));
            Assert.IsTrue(double.IsNaN(MathEx.MaxMagnitude(double.NaN, 1.0)));
        }

        [TestMethod]
        public void Min()
        {
            Assert.AreEqual(new IntPtr(-5), MathEx.Min(new IntPtr(-5), new IntPtr(10)));
            Assert.AreEqual(new UIntPtr(5U), MathEx.Min(new UIntPtr(5U), new UIntPtr(10U)));
        }

        [TestMethod]
        public void MinMagnitude()
        {
            Assert.AreEqual(5.0, MathEx.MinMagnitude(-10.0, 5.0));
            Assert.AreEqual(-10.0, MathEx.MinMagnitude(10.0, -10.0));
            Assert.IsTrue(double.IsNaN(MathEx.MinMagnitude(double.NaN, 1.0)));
        }

        [TestMethod]
        public void ReciprocalEstimate()
        {
            var result = MathEx.ReciprocalEstimate(4.0);

            Assert.IsTrue(result > 0.24 && result < 0.26);
        }

        [TestMethod]
        public void ReciprocalSqrtEstimate()
        {
            var result = MathEx.ReciprocalSqrtEstimate(4.0);

            Assert.IsTrue(result > 0.49 && result < 0.51);
        }

        [TestMethod]
        public void ScaleB()
        {
            Assert.AreEqual(96.0, MathEx.ScaleB(3.0, 5));
            Assert.AreEqual(0.25, MathEx.ScaleB(1.0, -2));
            Assert.AreEqual(double.PositiveInfinity, MathEx.ScaleB(double.PositiveInfinity, 5));
        }

        [TestMethod]
        public void Sign()
        {
            Assert.AreEqual(-1, MathEx.Sign(new IntPtr(-5)));
            Assert.AreEqual(0, MathEx.Sign(IntPtr.Zero));
            Assert.AreEqual(1, MathEx.Sign(new IntPtr(5)));
        }

        [TestMethod]
        public void SinCos()
        {
            var result = MathEx.SinCos(System.Math.PI / 6.0);

            Assert.AreEqual(0.5, result.Sin, 0.000000000000001);
            Assert.AreEqual(0.8660254037844387, result.Cos, 0.000000000000001);
        }

        [TestMethod]
        public void Tau()
        {
            Assert.AreEqual(2.0 * System.Math.PI, MathEx.Tau, 0.000000000000001);
        }

        #endregion

        #region Private

        private static bool IsNegativeZero(double value)
        {
            return value == 0.0 && BitConverter.DoubleToInt64Bits(value) < 0;
        }

        #endregion
    }
}

#pragma warning restore MSTEST0032 // Review or remove the assertion as its condition is known to be always true
