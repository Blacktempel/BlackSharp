// SPDX-License-Identifier: MPL-2.0

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
