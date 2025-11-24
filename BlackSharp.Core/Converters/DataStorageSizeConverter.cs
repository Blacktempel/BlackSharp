/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Converters
{
    /// <summary>
    /// This class provides methods for data size conversions.
    /// </summary>
    public static class DataStorageSizeConverter
    {
        #region Public

        /// <summary>
        /// Converts given byte value into megabytes.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns>Returns converted value.</returns>
        public static decimal ByteToMegabyte(ulong bytes)
        {
            return bytes == 0 ? 0M : bytes / 1000M / 1000M;
        }

        /// <summary>
        /// Converts given byte value into gigabytes.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns>Returns converted value.</returns>
        public static decimal ByteToGigabyte(ulong bytes)
        {
            return bytes == 0 ? 0M : bytes / 1000M / 1000M / 1000M;
        }

        #endregion
    }
}
