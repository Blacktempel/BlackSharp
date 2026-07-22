/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Text;

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for byte arrays.
    /// </summary>
    public static class ByteArrayExtensions
    {
        #region Public

        /// <summary>
        /// Reads a trimmed, null-terminated ASCII string from a bounded region of a byte array.
        /// </summary>
        /// <param name="data">Byte array containing the encoded string.</param>
        /// <param name="offset">Start offset of the encoded string.</param>
        /// <param name="length">Maximum number of bytes to read.</param>
        /// <returns>The decoded and trimmed string.</returns>
        public static string ReadNullTerminatedASCIIString(this byte[] data, int offset, int length)
        {
            ValidateStart(data, offset, length);

            var end = offset;
            var maximum = Math.Min(data.Length, offset + length);

            while (end < maximum && data[end] != 0)
            {
                ++end;
            }

            return Encoding.ASCII.GetString(data, offset, end - offset).Trim();
        }

        /// <summary>
        /// Reads a single-precision floating-point value in little-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded single-precision floating-point value.</returns>
        public static float ToSingleLittleEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 4);

            var bytes = new[]
            {
                data[offset],
                data[offset + 1],
                data[offset + 2],
                data[offset + 3],
            };

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reads an unsigned 16-bit integer in little-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded unsigned value.</returns>
        public static ushort ToUInt16LittleEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 2);

            return (ushort)(data[offset] | data[offset + 1] << 8);
        }

        /// <summary>
        /// Reads a signed 16-bit integer in little-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded signed value.</returns>
        public static short ToInt16LittleEndian(this byte[] data, int offset)
        {
            return unchecked((short)data.ToUInt16LittleEndian(offset));
        }

        /// <summary>
        /// Reads an unsigned 16-bit integer in big-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded unsigned value.</returns>
        public static ushort ToUInt16BigEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 2);

            return (ushort)(data[offset] << 8 | data[offset + 1]);
        }

        /// <summary>
        /// Reads a signed 32-bit integer in little-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded signed value.</returns>
        public static int ToInt32LittleEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 4);

            return data[offset]
                 | data[offset + 1] << 8
                 | data[offset + 2] << 16
                 | data[offset + 3] << 24;
        }

        /// <summary>
        /// Reads an unsigned 32-bit integer in little-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded unsigned value.</returns>
        public static uint ToUInt32LittleEndian(this byte[] data, int offset)
        {
            return unchecked((uint)data.ToInt32LittleEndian(offset));
        }

        /// <summary>
        /// Reads an unsigned 32-bit integer in big-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded unsigned value.</returns>
        public static uint ToUInt32BigEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 4);

            return ((uint)data[offset    ] << 24)
                 | ((uint)data[offset + 1] << 16)
                 | ((uint)data[offset + 2] <<  8)
                 | data[offset + 3];
        }

        /// <summary>
        /// Reads an unsigned 64-bit integer in big-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded unsigned value.</returns>
        public static ulong ToUInt64BigEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 8);

            ulong value = 0;

            for (var index = 0; index < 8; ++index)
            {
                value = (value << 8) | data[offset + index];
            }

            return value;
        }

        /// <summary>
        /// Reads an unsigned 48-bit integer in little-endian byte order.
        /// </summary>
        /// <param name="data">Byte array containing the value.</param>
        /// <param name="offset">Start offset of the value.</param>
        /// <returns>The decoded unsigned value.</returns>
        public static ulong ToUInt48LittleEndian(this byte[] data, int offset)
        {
            ValidateRange(data, offset, 6);

            ulong value = 0;

            for (var index = 0; index < 6; ++index)
            {
                value |= ((ulong)data[offset + index]) << (8 * index);
            }

            return value;
        }

        #endregion

        #region Private

        private static void ValidateRange(byte[] data, int offset, int length)
        {
            ValidateStart(data, offset, length);

            if (length > data.Length || offset > data.Length - length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
        }

        private static void ValidateStart(byte[] data, int offset, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (offset < 0 || offset > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
        }

        #endregion
    }
}
