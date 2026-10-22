// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Identifies a Windows Lockdown Policy host.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WldpHostInformation
    {
        #region Fields

        /// <summary>
        /// Gets the structure revision.
        /// </summary>
        public uint Revision;

        /// <summary>
        /// Gets the host identifier.
        /// </summary>
        public uint HostID;

        /// <summary>
        /// Gets the optional source pointer.
        /// </summary>
        public IntPtr Source;

        /// <summary>
        /// Gets the optional source handle.
        /// </summary>
        public IntPtr SourceHandle;

        #endregion
    }
}
