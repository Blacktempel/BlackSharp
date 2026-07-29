/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected Windows physical-monitor configuration functions.
    /// </summary>
    public static class Dxva2
    {
        #region Fields

        const string DLL_NAME = "dxva2.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Releases an array of physical-monitor handles.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint monitorCount, IntPtr physicalMonitors);

        /// <summary>
        /// Retrieves the technology type reported by a physical monitor.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorTechnologyType(IntPtr physicalMonitor, out int technologyType);

        /// <summary>
        /// Retrieves the number of physical monitors associated with a logical monitor.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr monitor, out uint monitorCount);

        /// <summary>
        /// Retrieves physical-monitor descriptors for a logical monitor.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr monitor, uint monitorCount, IntPtr physicalMonitors);

        /// <summary>
        /// Retrieves the current timing report for a physical monitor.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTimingReport(IntPtr physicalMonitor, IntPtr timingReport);

        #endregion
    }
}
