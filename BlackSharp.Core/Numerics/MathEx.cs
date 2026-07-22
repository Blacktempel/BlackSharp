/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Numerics;

namespace BlackSharp.Core.Numerics
{
    /// <summary>
    /// Compatibility helpers for <see cref="System.Math"/> members that are not available on .NET Framework.
    /// </summary>
    public static class MathEx
    {
        #region Fields

        private const long DoubleSignMask = unchecked((long)0x8000000000000000);
        private static readonly BigInteger UInt64Mask = new BigInteger(ulong.MaxValue);

        /// <summary>
        /// Represents the number of radians in one turn, specified by the constant τ.
        /// </summary>
        public const double Tau = 6.28318530717958647692528676655900576839433879875021;

        #endregion

        #region Public

        /// <summary>
        /// Returns the absolute value of a native signed integer.
        /// </summary>
        /// <param name="value">Native signed integer value.</param>
        /// <returns>Absolute value.</returns>
        public static IntPtr Abs(IntPtr value)
        {
            var raw = value.ToInt64();

            if (raw == long.MinValue)
            {
                throw new OverflowException();
            }

            return new IntPtr(System.Math.Abs(raw));
        }

        /// <summary>
        /// Returns the angle whose hyperbolic cosine is the specified number.
        /// </summary>
        /// <param name="d">Hyperbolic cosine value.</param>
        /// <returns>Angle in radians.</returns>
        public static double Acosh(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.Acosh(d);
#else
            if (double.IsNaN(d) || d < 1.0)
            {
                return double.NaN;
            }

            if (double.IsPositiveInfinity(d))
            {
                return double.PositiveInfinity;
            }

            return d > 1.0e154
                ? System.Math.Log(d) + System.Math.Log(2.0)
                : System.Math.Log(d + System.Math.Sqrt((d - 1.0) * (d + 1.0)));
#endif
        }

        /// <summary>
        /// Returns the angle whose hyperbolic sine is the specified number.
        /// </summary>
        /// <param name="d">Hyperbolic sine value.</param>
        /// <returns>Angle in radians.</returns>
        public static double Asinh(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.Asinh(d);
#else
            if (double.IsNaN(d) || double.IsInfinity(d) || d == 0.0)
            {
                return d;
            }

            var magnitude = System.Math.Abs(d);
            var result = magnitude > 1.0e154
                ? System.Math.Log(magnitude) + System.Math.Log(2.0)
                : System.Math.Log(magnitude + System.Math.Sqrt(magnitude * magnitude + 1.0));

            return CopySign(result, d);
#endif
        }

        /// <summary>
        /// Returns the angle whose hyperbolic tangent is the specified number.
        /// </summary>
        /// <param name="d">Hyperbolic tangent value.</param>
        /// <returns>Angle in radians.</returns>
        public static double Atanh(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.Atanh(d);
#else
            if (double.IsNaN(d))
            {
                return double.NaN;
            }

            if (d > 1.0 || d < -1.0)
            {
                return double.NaN;
            }

            if (d == 1.0)
            {
                return double.PositiveInfinity;
            }

            if (d == -1.0)
            {
                return double.NegativeInfinity;
            }

            if (d == 0.0)
            {
                return d;
            }

            return 0.5 * System.Math.Log((1.0 + d) / (1.0 - d));
#endif
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Produces the full product of two 64-bit numbers.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Full product.</returns>
        public static Int128 BigMul(long a, long b)
        {
            return (Int128)a * b;
        }

        /// <summary>
        /// Produces the full product of two unsigned 64-bit numbers.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Full product.</returns>
        public static UInt128 BigMul(ulong a, ulong b)
        {
            return (UInt128)a * b;
        }
#endif

        /// <summary>
        /// Produces the full product of two unsigned 32-bit numbers.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Full product.</returns>
        public static ulong BigMul(uint a, uint b)
        {
            return (ulong)a * b;
        }

        /// <summary>
        /// Produces the full product of two 64-bit numbers.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="low">Low 64 bits of the product.</param>
        /// <returns>High 64 bits of the product.</returns>
        public static long BigMul(long a, long b, out long low)
        {
#if NET8_0_OR_GREATER
            return System.Math.BigMul(a, b, out low);
#else
            var product = (BigInteger)a * b;
            var lower = (ulong)(product & UInt64Mask);

            low = unchecked((long)lower);
            return (long)(product >> 64);
#endif
        }

        /// <summary>
        /// Produces the full product of two unsigned 64-bit numbers.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="low">Low 64 bits of the product.</param>
        /// <returns>High 64 bits of the product.</returns>
        public static ulong BigMul(ulong a, ulong b, out ulong low)
        {
#if NET8_0_OR_GREATER
            return System.Math.BigMul(a, b, out low);
#else
            var product = (BigInteger)a * b;

            low = (ulong)(product & UInt64Mask);
            return (ulong)(product >> 64);
#endif
        }

        /// <summary>
        /// Returns the largest value that compares less than a specified value.
        /// </summary>
        /// <param name="x">Value to decrement.</param>
        /// <returns>Largest value that compares less than <paramref name="x"/>.</returns>
        public static double BitDecrement(double x)
        {
#if NET8_0_OR_GREATER
            return System.Math.BitDecrement(x);
#else
            if (double.IsNaN(x) || x == double.NegativeInfinity)
            {
                return x;
            }

            if (x == 0.0)
            {
                return -double.Epsilon;
            }

            var bits = DoubleToInt64Bits(x);
            bits = bits < 0 ? bits + 1 : bits - 1;

            return Int64BitsToDouble(bits);
#endif
        }

        /// <summary>
        /// Returns the smallest value that compares greater than a specified value.
        /// </summary>
        /// <param name="x">Value to increment.</param>
        /// <returns>Smallest value that compares greater than <paramref name="x"/>.</returns>
        public static double BitIncrement(double x)
        {
#if NET8_0_OR_GREATER
            return System.Math.BitIncrement(x);
#else
            if (double.IsNaN(x) || x == double.PositiveInfinity)
            {
                return x;
            }

            if (x == 0.0)
            {
                return double.Epsilon;
            }

            var bits = DoubleToInt64Bits(x);
            bits = bits < 0 ? bits - 1 : bits + 1;

            return Int64BitsToDouble(bits);
#endif
        }

        /// <summary>
        /// Returns the cube root of a specified number.
        /// </summary>
        /// <param name="d">Value.</param>
        /// <returns>Cube root.</returns>
        public static double Cbrt(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.Cbrt(d);
#else
            if (double.IsNaN(d) || double.IsInfinity(d) || d == 0.0)
            {
                return d;
            }

            var result = System.Math.Pow(System.Math.Abs(d), 1.0 / 3.0);
            return CopySign(result, d);
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static byte Clamp(byte value, byte min, byte max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static decimal Clamp(decimal value, decimal min, decimal max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static double Clamp(double value, double min, double max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static short Clamp(short value, short min, short max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static int Clamp(int value, int min, int max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static long Clamp(long value, long min, long max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static IntPtr Clamp(IntPtr value, IntPtr min, IntPtr max)
        {
            var rawValue = value.ToInt64();
            var rawMin = min.ToInt64();
            var rawMax = max.ToInt64();

            if (rawMax < rawMin)
            {
                ThrowMinMaxException();
            }

            if (rawValue < rawMin)
            {
                return min;
            }

            return rawValue > rawMax ? max : value;
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static ushort Clamp(ushort value, ushort min, ushort max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static uint Clamp(uint value, uint min, uint max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static ulong Clamp(ulong value, ulong min, ulong max)
        {
#if NET8_0_OR_GREATER
            return System.Math.Clamp(value, min, max);
#else
            if (max < min)
            {
                ThrowMinMaxException();
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
#endif
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static UIntPtr Clamp(UIntPtr value, UIntPtr min, UIntPtr max)
        {
            var rawValue = value.ToUInt64();
            var rawMin = min.ToUInt64();
            var rawMax = max.ToUInt64();

            if (rawMax < rawMin)
            {
                ThrowMinMaxException();
            }

            if (rawValue < rawMin)
            {
                return min;
            }

            return rawValue > rawMax ? max : value;
        }

        /// <summary>
        /// Returns a value with the magnitude of <paramref name="x"/> and the sign of <paramref name="y"/>.
        /// </summary>
        /// <param name="x">Value that provides the magnitude.</param>
        /// <param name="y">Value that provides the sign.</param>
        /// <returns>Value with copied sign.</returns>
        public static double CopySign(double x, double y)
        {
#if NET8_0_OR_GREATER
            return System.Math.CopySign(x, y);
#else
            var xBits = DoubleToInt64Bits(x);
            var yBits = DoubleToInt64Bits(y);

            return Int64BitsToDouble((xBits & ~DoubleSignMask) | (yBits & DoubleSignMask));
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two unsigned 8-bit numbers.
        /// </summary>
        public static (byte Quotient, byte Remainder) DivRem(byte left, byte right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return ((byte)(left / right), (byte)(left % right));
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two signed 16-bit numbers.
        /// </summary>
        public static (short Quotient, short Remainder) DivRem(short left, short right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return ((short)(left / right), (short)(left % right));
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two signed 32-bit numbers.
        /// </summary>
        public static (int Quotient, int Remainder) DivRem(int left, int right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return (System.Math.DivRem(left, right, out var remainder), remainder);
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two signed 64-bit numbers.
        /// </summary>
        public static (long Quotient, long Remainder) DivRem(long left, long right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return (System.Math.DivRem(left, right, out var remainder), remainder);
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two signed native-size numbers.
        /// </summary>
        public static (IntPtr Quotient, IntPtr Remainder) DivRem(IntPtr left, IntPtr right)
        {
            var quotient = System.Math.DivRem(left.ToInt64(), right.ToInt64(), out var remainder);
            return (new IntPtr(quotient), new IntPtr(remainder));
        }

        /// <summary>
        /// Produces the quotient and the remainder of two signed 8-bit numbers.
        /// </summary>
        public static (sbyte Quotient, sbyte Remainder) DivRem(sbyte left, sbyte right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return ((sbyte)(left / right), (sbyte)(left % right));
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two unsigned 16-bit numbers.
        /// </summary>
        public static (ushort Quotient, ushort Remainder) DivRem(ushort left, ushort right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return ((ushort)(left / right), (ushort)(left % right));
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two unsigned 32-bit numbers.
        /// </summary>
        public static (uint Quotient, uint Remainder) DivRem(uint left, uint right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return (left / right, left % right);
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two unsigned 64-bit numbers.
        /// </summary>
        public static (ulong Quotient, ulong Remainder) DivRem(ulong left, ulong right)
        {
#if NET8_0_OR_GREATER
            return System.Math.DivRem(left, right);
#else
            return (left / right, left % right);
#endif
        }

        /// <summary>
        /// Produces the quotient and the remainder of two unsigned native-size numbers.
        /// </summary>
        public static (UIntPtr Quotient, UIntPtr Remainder) DivRem(UIntPtr left, UIntPtr right)
        {
            var rawLeft = left.ToUInt64();
            var rawRight = right.ToUInt64();

            return (new UIntPtr(rawLeft / rawRight), new UIntPtr(rawLeft % rawRight));
        }

        /// <summary>
        /// Returns (<paramref name="x"/> * <paramref name="y"/>) + <paramref name="z"/>.
        /// </summary>
        /// <param name="x">First multiplication value.</param>
        /// <param name="y">Second multiplication value.</param>
        /// <param name="z">Additive value.</param>
        /// <returns>Result of the multiplication and addition.</returns>
        public static double FusedMultiplyAdd(double x, double y, double z)
        {
#if NET8_0_OR_GREATER
            return System.Math.FusedMultiplyAdd(x, y, z);
#else
            return (x * y) + z;
#endif
        }

        /// <summary>
        /// Returns the base 2 integer logarithm of a specified number.
        /// </summary>
        /// <param name="d">Value.</param>
        /// <returns>Base 2 integer logarithm.</returns>
        public static int ILogB(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.ILogB(d);
#else
            if (d == 0.0)
            {
                return int.MinValue;
            }

            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return int.MaxValue;
            }

            var bits = DoubleToInt64Bits(System.Math.Abs(d));
            var exponent = (int)((bits >> 52) & 0x7FFL);

            if (exponent != 0)
            {
                return exponent - 1023;
            }

            var mantissa = bits & 0xFFFFFFFFFFFFFL;
            exponent = -1023;

            while ((mantissa & 0x8000000000000L) == 0)
            {
                mantissa <<= 1;
                exponent--;
            }

            return exponent;
#endif
        }

        /// <summary>
        /// Returns the base 2 logarithm of a specified number.
        /// </summary>
        /// <param name="d">Value.</param>
        /// <returns>Base 2 logarithm.</returns>
        public static double Log2(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.Log2(d);
#else
            return System.Math.Log(d, 2.0);
#endif
        }

        /// <summary>
        /// Returns the larger of two native signed integers.
        /// </summary>
        public static IntPtr Max(IntPtr val1, IntPtr val2)
        {
            return val1.ToInt64() >= val2.ToInt64() ? val1 : val2;
        }

        /// <summary>
        /// Returns the larger of two native unsigned integers.
        /// </summary>
        public static UIntPtr Max(UIntPtr val1, UIntPtr val2)
        {
            return val1.ToUInt64() >= val2.ToUInt64() ? val1 : val2;
        }

        /// <summary>
        /// Returns the larger of two nullable double-precision floating-point numbers.
        /// </summary>
        /// <param name="val1">First value.</param>
        /// <param name="val2">Second value.</param>
        /// <returns>The larger value, the present value, or <see langword="null"/> when neither is present.</returns>
        public static double? Max(double? val1, double? val2)
        {
            if (!val2.HasValue)
            {
                return val1;
            }

            return val1.HasValue
                ? Math.Max(val1.Value, val2.Value)
                : val2;
        }

        /// <summary>
        /// Returns the larger magnitude of two double-precision floating-point numbers.
        /// </summary>
        /// <param name="x">First value.</param>
        /// <param name="y">Second value.</param>
        /// <returns>Larger magnitude.</returns>
        public static double MaxMagnitude(double x, double y)
        {
#if NET8_0_OR_GREATER
            return System.Math.MaxMagnitude(x, y);
#else
            if (double.IsNaN(x))
            {
                return x;
            }

            if (double.IsNaN(y))
            {
                return y;
            }

            var xMagnitude = System.Math.Abs(x);
            var yMagnitude = System.Math.Abs(y);

            if (xMagnitude > yMagnitude)
            {
                return x;
            }

            if (xMagnitude < yMagnitude)
            {
                return y;
            }

            if (x == y)
            {
                return IsNegative(x) ? y : x;
            }

            return x > y ? x : y;
#endif
        }

        /// <summary>
        /// Returns the smaller of two native signed integers.
        /// </summary>
        public static IntPtr Min(IntPtr val1, IntPtr val2)
        {
            return val1.ToInt64() <= val2.ToInt64() ? val1 : val2;
        }

        /// <summary>
        /// Returns the smaller of two native unsigned integers.
        /// </summary>
        public static UIntPtr Min(UIntPtr val1, UIntPtr val2)
        {
            return val1.ToUInt64() <= val2.ToUInt64() ? val1 : val2;
        }

        /// <summary>
        /// Returns the smaller magnitude of two double-precision floating-point numbers.
        /// </summary>
        /// <param name="x">First value.</param>
        /// <param name="y">Second value.</param>
        /// <returns>Smaller magnitude.</returns>
        public static double MinMagnitude(double x, double y)
        {
#if NET8_0_OR_GREATER
            return System.Math.MinMagnitude(x, y);
#else
            if (double.IsNaN(x))
            {
                return x;
            }

            if (double.IsNaN(y))
            {
                return y;
            }

            var xMagnitude = System.Math.Abs(x);
            var yMagnitude = System.Math.Abs(y);

            if (xMagnitude < yMagnitude)
            {
                return x;
            }

            if (xMagnitude > yMagnitude)
            {
                return y;
            }

            if (x == y)
            {
                return IsNegative(x) ? x : y;
            }

            return x < y ? x : y;
#endif
        }

        /// <summary>
        /// Returns an estimate of the reciprocal of a specified number.
        /// </summary>
        /// <param name="d">Value.</param>
        /// <returns>Reciprocal estimate.</returns>
        public static double ReciprocalEstimate(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.ReciprocalEstimate(d);
#else
            return 1.0 / d;
#endif
        }

        /// <summary>
        /// Returns an estimate of the reciprocal square root of a specified number.
        /// </summary>
        /// <param name="d">Value.</param>
        /// <returns>Reciprocal square root estimate.</returns>
        public static double ReciprocalSqrtEstimate(double d)
        {
#if NET8_0_OR_GREATER
            return System.Math.ReciprocalSqrtEstimate(d);
#else
            return 1.0 / System.Math.Sqrt(d);
#endif
        }

        /// <summary>
        /// Returns <paramref name="x"/> * 2^<paramref name="n"/> computed efficiently.
        /// </summary>
        /// <param name="x">Base value.</param>
        /// <param name="n">Power of two.</param>
        /// <returns>Scaled value.</returns>
        public static double ScaleB(double x, int n)
        {
#if NET8_0_OR_GREATER
            return System.Math.ScaleB(x, n);
#else
            if (double.IsNaN(x) || double.IsInfinity(x) || x == 0.0)
            {
                return x;
            }

            return x * System.Math.Pow(2.0, n);
#endif
        }

        /// <summary>
        /// Returns an integer that indicates the sign of a native-sized signed integer.
        /// </summary>
        public static int Sign(IntPtr value)
        {
            return System.Math.Sign(value.ToInt64());
        }

        /// <summary>
        /// Returns the sine and cosine of the specified angle.
        /// </summary>
        /// <param name="x">Angle in radians.</param>
        /// <returns>Sine and cosine.</returns>
        public static (double Sin, double Cos) SinCos(double x)
        {
#if NET8_0_OR_GREATER
            return System.Math.SinCos(x);
#else
            return (System.Math.Sin(x), System.Math.Cos(x));
#endif
        }

        #endregion

        #region Private

        static long DoubleToInt64Bits(double value)
        {
#if NET8_0_OR_GREATER
            return BitConverter.DoubleToInt64Bits(value);
#else
            return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
#endif
        }

        static double Int64BitsToDouble(long value)
        {
#if NET8_0_OR_GREATER
            return BitConverter.Int64BitsToDouble(value);
#else
            return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
#endif
        }

        static bool IsNegative(double value)
        {
            return (DoubleToInt64Bits(value) & DoubleSignMask) != 0;
        }

        static void ThrowMinMaxException()
        {
            throw new ArgumentException("Maximum value must be greater than or equal to minimum value.");
        }

        #endregion
    }
}
