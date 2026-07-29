/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Structures
{
    /// <summary>
    /// Describes a file descriptor monitored by the POSIX poll function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PollFd
    {
        /// <summary>
        /// The file descriptor to monitor.
        /// </summary>
        public int FileDescriptor;

        /// <summary>
        /// The requested event mask.
        /// </summary>
        public short Events;

        /// <summary>
        /// The event mask reported by the operating system.
        /// </summary>
        public short ReturnedEvents;
    }
}
