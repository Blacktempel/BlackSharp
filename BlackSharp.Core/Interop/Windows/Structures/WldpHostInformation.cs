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
