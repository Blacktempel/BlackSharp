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
    /// Contains extended information about a Windows display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MonitorInfoEx
    {
        #region Fields

        /// <summary>
        /// Contains the structure size.
        /// </summary>
        public int Size;

        /// <summary>
        /// Contains the monitor rectangle.
        /// </summary>
        public Win32Rectangle Monitor;

        /// <summary>
        /// Contains the work-area rectangle.
        /// </summary>
        public Win32Rectangle Work;

        /// <summary>
        /// Contains monitor flags.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Contains the monitor device name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        #endregion
    }
}
