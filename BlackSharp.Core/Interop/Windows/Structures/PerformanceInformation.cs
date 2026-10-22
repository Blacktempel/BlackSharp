// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Contains system-wide memory, handle, process and thread counters returned by PSAPI.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PerformanceInformation
    {
        #region Fields

        /// <summary>
        /// Gets the structure size in bytes.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Gets total committed pages.
        /// </summary>
        public UIntPtr CommitTotal;

        /// <summary>
        /// Gets the commit limit in pages.
        /// </summary>
        public UIntPtr CommitLimit;

        /// <summary>
        /// Gets peak committed pages.
        /// </summary>
        public UIntPtr CommitPeak;

        /// <summary>
        /// Gets total physical pages.
        /// </summary>
        public UIntPtr PhysicalTotal;

        /// <summary>
        /// Gets available physical pages.
        /// </summary>
        public UIntPtr PhysicalAvailable;

        /// <summary>
        /// Gets cached system pages.
        /// </summary>
        public UIntPtr SystemCache;

        /// <summary>
        /// Gets total kernel pages.
        /// </summary>
        public UIntPtr KernelTotal;

        /// <summary>
        /// Gets paged kernel pages.
        /// </summary>
        public UIntPtr KernelPaged;

        /// <summary>
        /// Gets nonpaged kernel pages.
        /// </summary>
        public UIntPtr KernelNonpaged;

        /// <summary>
        /// Gets the page size in bytes.
        /// </summary>
        public UIntPtr PageSize;

        /// <summary>
        /// Gets the number of open handles.
        /// </summary>
        public uint HandleCount;

        /// <summary>
        /// Gets the number of processes.
        /// </summary>
        public uint ProcessCount;

        /// <summary>
        /// Gets the number of threads.
        /// </summary>
        public uint ThreadCount;

        #endregion
    }
}
