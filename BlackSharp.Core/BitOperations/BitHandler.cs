/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.BitOperations
{
    /// <summary>
    /// Helper class for bitwise operations.
    /// </summary>
    public static class BitHandler
    {
        #region Public

        /// <summary>
        /// Check if bit is set in given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="bitPos">Position of bit to check.</param>
        /// <returns>Returns whether specified bit is set in byte.</returns>
        public static bool IsBitSet(byte value, int bitPos)
        {
            return (value & (1 << bitPos)) != 0;
        }

        /// <summary>
        /// Check if bit is set in given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="bitPos">Position of bit to check.</param>
        /// <returns>Returns whether specified bit is set in byte.</returns>
        public static bool IsBitSet(ushort value, int bitPos)
        {
            return (value & (1 << bitPos)) != 0;
        }

        /// <summary>
        /// Manually swap bits in given byte.
        /// </summary>
        /// <param name="b">Byte to swap bits in.</param>
        /// <returns>Byte with swapped bits.</returns>
        public static byte SwapBits(byte b)
        {
            byte temp = 0;

            for (byte i = 0; i < 8; ++i)
            {
                temp |= (byte)((b & (1 << i)) != 0 ? (1 << (7 - i)) : 0);
            }

            return temp;
        }

        /// <summary>
        /// Unset bit at given index.
        /// </summary>
        /// <param name="value">Value to unset bit on.</param>
        /// <param name="bitIndexToUnset">Index of bit to unset.</param>
        /// <returns>Byte value with bit unset.</returns>
        public static byte UnsetBit(byte value, byte bitIndexToUnset)
        {
            return (byte)(value & ~(1 << bitIndexToUnset));
        }

        /// <summary>
        /// Get specified bit range as value.
        /// </summary>
        /// <typeparam name="T">Integer type.</typeparam>
        /// <param name="value">Value to get specific bits from.</param>
        /// <param name="fromBit">First bit in range to get value for.</param>
        /// <param name="toBit">Last bit in range to get value for.</param>
        /// <returns>Returns extracted value.</returns>
#if NET8_0_OR_GREATER
        public static T GetBits<T>(T value, int fromBit, int toBit)
            where T : System.Numerics.IBinaryInteger<T>
        {
            if (fromBit < 0 || toBit < fromBit)
            {
                throw new ArgumentOutOfRangeException();
            }

            int bitSize = int.CreateChecked(T.PopCount(~T.Zero));

            if (toBit >= bitSize)
            {
                throw new ArgumentOutOfRangeException();
            }

            int length = toBit - fromBit + 1;
            T mask = (T.One << length) - T.One;

            return (value >> fromBit) & mask;
        }
#else
        public static unsafe T GetBits<T>(T value, int fromBit, int toBit)
            where T : struct
        {
            if (fromBit < 0 || toBit < fromBit)
            {
                throw new ArgumentOutOfRangeException();
            }

            int bitSize = sizeof(T) * 8;

            if (toBit >= bitSize)
            {
                throw new ArgumentOutOfRangeException();
            }

            ulong raw = 0;

            byte* src = (byte*)&value;
            byte* dst = (byte*)&raw;

            for (int i = 0; i < sizeof(T); ++i)
            {
                dst[i] = src[i];
            }

            int length = toBit - fromBit + 1;
            ulong mask = (1UL << length) - 1UL;

            ulong result = (raw >> fromBit) & mask;

            T ret = default;
            byte* rsrc = (byte*)&result;
            byte* rdst = (byte*)&ret;

            for (int i = 0; i < sizeof(T); ++i)
            {
                rdst[i] = rsrc[i];
            }

            return ret;
        }
#endif

#if NET8_0_OR_GREATER
        public static T SetBits<T>(T value, T bits, int fromBit, int toBit)
            where T : System.Numerics.IBinaryInteger<T>
        {
            if (fromBit < 0 || toBit < fromBit)
                throw new ArgumentOutOfRangeException();

            int bitSize = int.CreateChecked(T.PopCount(~T.Zero));
            if (toBit >= bitSize)
                throw new ArgumentOutOfRangeException();

            int length = toBit - fromBit + 1;

            T mask = ((T.One << length) - T.One) << fromBit;

            T bitsMasked = (bits & ((T.One << length) - T.One)) << fromBit;

            return (value & ~mask) | bitsMasked;
        }
#else
        public static unsafe T SetBits<T>(T value, T bits, int fromBit, int toBit)
            where T : struct
        {
            if (fromBit < 0 || toBit < fromBit)
                throw new ArgumentOutOfRangeException();

            int bitSize = sizeof(T) * 8;
            if (toBit >= bitSize)
                throw new ArgumentOutOfRangeException();

            ulong rawValue = 0;
            ulong rawBits = 0;

            {
                byte* src = (byte*)&value;
                byte* dst = (byte*)&rawValue;
                for (int i = 0; i < sizeof(T); ++i)
                    dst[i] = src[i];
            }

            {
                byte* src = (byte*)&bits;
                byte* dst = (byte*)&rawBits;
                for (int i = 0; i < sizeof(T); ++i)
                    dst[i] = src[i];
            }

            int length = toBit - fromBit + 1;
            ulong mask = ((1UL << length) - 1UL) << fromBit;

            ulong bitsMasked = (rawBits & ((1UL << length) - 1UL)) << fromBit;
            ulong result = (rawValue & ~mask) | bitsMasked;

            T ret = default;
            {
                byte* src = (byte*)&result;
                byte* dst = (byte*)&ret;
                for (int i = 0; i < sizeof(T); ++i)
                    dst[i] = src[i];
            }

            return ret;
        }
#endif

#if NET8_0_OR_GREATER
        public static T SetBits<T>(T value, int fromBit, int toBit, bool set = true)
            where T : System.Numerics.IBinaryInteger<T>
        {
            if (fromBit < 0 || toBit < fromBit)
            {
                throw new ArgumentOutOfRangeException();
            }

            int bitSize = int.CreateChecked(T.PopCount(~T.Zero));

            if (toBit >= bitSize)
            {
                throw new ArgumentOutOfRangeException();
            }

            int length = toBit - fromBit + 1;
            T mask = ((T.One << length) - T.One) << fromBit;

            return set ? (value | mask) : (value & ~mask);
    }
#else
        public static unsafe T SetBits<T>(T value, int fromBit, int toBit, bool set = true)
            where T : struct
        {
            if (fromBit < 0 || toBit < fromBit)
            {
                throw new ArgumentOutOfRangeException();
            }

            int bitSize = sizeof(T) * 8;

            if (toBit >= bitSize)
            {
                throw new ArgumentOutOfRangeException();
            }

            ulong raw = 0;

            byte* src = (byte*)&value;
            byte* dst = (byte*)&raw;

            for (int i = 0; i < sizeof(T); ++i)
            {
                dst[i] = src[i];
            }

            int length = toBit - fromBit + 1;
            ulong mask = ((1UL << length) - 1UL) << fromBit;

            ulong result = set ? (raw | mask) : (raw & ~mask);

            T ret = default;
            byte* rsrc = (byte*)&result;
            byte* rdst = (byte*)&ret;

            for (int i = 0; i < sizeof(T); ++i)
            {
                rdst[i] = rsrc[i];
            }

            return ret;
        }
#endif

        public static unsafe T ExtractBits<T>(T value, params int[] bitPositions)
            where T : struct
        {
            ulong raw = 0;
            {
                ulong tmp = 0;

                byte* src = (byte*)&value;
                byte* dst = (byte*)&tmp;

                for (int i = 0; i < sizeof(T); ++i)
                {
                    dst[i] = src[i];
                }

                raw = tmp;
            }

            ulong result = 0;

            for (int i = 0; i < bitPositions.Length; ++i)
            {
                int bitPos = bitPositions[i];
                ulong bit = (raw >> bitPos) & 1UL;
                result |= (bit << i);
            }

            T ret = default;
            {
                byte* src = (byte*)&result;
                byte* dst = (byte*)&ret;

                for (int i = 0; i < sizeof(T); ++i)
                {
                    dst[i] = src[i];
                }
            }

            return ret;
        }

        #endregion
    }
}
