/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Interop.Windows
{
    /// <summary>
    /// Provides commonly used Win32 system error codes.
    /// </summary>
    public static class Win32ErrorCodes
    {
        #region Fields

        /// <summary>
        /// Indicates that an operation completed successfully.
        /// </summary>
        public const int Success               = 0;

        /// <summary>
        /// Indicates that the requested file could not be found.
        /// </summary>
        public const int FileNotFound          = 2;

        /// <summary>
        /// Indicates that access to a file or device was denied.
        /// </summary>
        public const int AccessDenied          = 5;

        /// <summary>
        /// Indicates that a file or device cannot be opened because of incompatible sharing settings.
        /// </summary>
        public const int SharingViolation      = 32;

        /// <summary>
        /// Indicates that the supplied buffer is too small.
        /// </summary>
        public const int InsufficientBuffer    = 122;

        /// <summary>
        /// Indicates that additional data is available.
        /// </summary>
        public const int MoreData              = 234;

        /// <summary>
        /// Indicates that an enumeration contains no additional items.
        /// </summary>
        public const int NoMoreItems           = 259;

        /// <summary>
        /// Indicates that the requested device does not exist.
        /// </summary>
        public const int NoSuchDevice          = 433;

        /// <summary>
        /// Indicates that an I/O operation was canceled.
        /// </summary>
        public const int OperationAborted      = 995;

        /// <summary>
        /// Indicates that an overlapped I/O operation has not completed.
        /// </summary>
        public const int IoIncomplete          = 996;

        /// <summary>
        /// Indicates that an overlapped I/O operation is pending.
        /// </summary>
        public const int IoPending             = 997;

        /// <summary>
        /// Indicates that a service is already running.
        /// </summary>
        public const int ServiceAlreadyRunning = 1056;

        /// <summary>
        /// Indicates that the requested service is not installed.
        /// </summary>
        public const int ServiceDoesNotExist   = 1060;

        /// <summary>
        /// Indicates that the requested service is not active.
        /// </summary>
        public const int ServiceNotActive      = 1062;

        /// <summary>
        /// Indicates that the requested item could not be found.
        /// </summary>
        public const int NotFound              = 1168;

        #endregion
    }
}
