/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Contains the current state of physical and virtual memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatusEx
    {
        /// <summary>
        /// The size of this structure in bytes.
        /// </summary>
        public uint Length;

        /// <summary>
        /// The approximate percentage of physical memory currently in use.
        /// </summary>
        public uint MemoryLoad;

        /// <summary>
        /// The total amount of physical memory in bytes.
        /// </summary>
        public ulong TotalPhysical;

        /// <summary>
        /// The amount of physical memory currently available in bytes.
        /// </summary>
        public ulong AvailablePhysical;

        /// <summary>
        /// The current committed-memory limit in bytes.
        /// </summary>
        public ulong TotalPageFile;

        /// <summary>
        /// The amount of memory available within the commit limit in bytes.
        /// </summary>
        public ulong AvailablePageFile;

        /// <summary>
        /// The total user-mode virtual address space in bytes.
        /// </summary>
        public ulong TotalVirtual;

        /// <summary>
        /// The currently unused user-mode virtual address space in bytes.
        /// </summary>
        public ulong AvailableVirtual;

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public ulong AvailableExtendedVirtual;
    }
}
