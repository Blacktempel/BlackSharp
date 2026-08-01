/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Interop.Linux
{
    /// <summary>
    /// Provides commonly used Linux system error codes.
    /// </summary>
    public static class LinuxErrorCodes
    {
        #region Fields

        /// <summary>
        /// Indicates a generic input/output failure.
        /// </summary>
        public const int InputOutput = 5;

        /// <summary>
        /// Indicates that the requested device or address does not exist.
        /// </summary>
        public const int NoSuchDeviceOrAddress = 6;

        /// <summary>
        /// Indicates that the requested device does not exist.
        /// </summary>
        public const int NoSuchDevice = 19;

        /// <summary>
        /// Indicates that an operation timed out.
        /// </summary>
        public const int TimedOut = 110;

        /// <summary>
        /// Indicates that a remote input/output operation failed.
        /// </summary>
        public const int RemoteInputOutput = 121;

        #endregion
    }
}
