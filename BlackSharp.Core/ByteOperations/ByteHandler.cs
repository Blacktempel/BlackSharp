// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.ByteOperations
{
    /// <summary>
    /// Helper class for binary operations.
    /// </summary>
    public static class BinaryHandler
    {
        /// <summary>
        /// Converts a given byte, which is encoded in binary coded decimal (BCD), to a standard byte value.
        /// </summary>
        /// <param name="bcd">Binary encoded decimal (BCD) value.</param>
        /// <returns>Decoded byte value.</returns>
        /// <example>Input is 0x14 (0001 0100); Output will be 14.</example>
        public static byte NormalizeBcd(byte bcd)
        {
            int tens = (bcd >> 4) * 10;
            int ones = bcd & 0x0F;

            return (byte)(tens + ones);
        }
    }
}
