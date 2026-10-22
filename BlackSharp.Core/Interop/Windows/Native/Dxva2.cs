// SPDX-License-Identifier: MPL-2.0

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

        /// <summary>
        /// Retrieves the current and maximum values of a monitor's VCP feature.
        /// </summary>
        /// <param name="physicalMonitor">The physical monitor handle.</param>
        /// <param name="vcpCode">The VCP feature code.</param>
        /// <param name="codeType">Optional pointer receiving the VCP code type; may be zero.</param>
        /// <param name="currentValue">Receives the current feature value.</param>
        /// <param name="maximumValue">Receives the maximum value for a continuous feature; undefined for a noncontinuous feature.</param>
        /// <returns><see langword="true"/> if the monitor returned the feature value.</returns>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetVCPFeatureAndVCPFeatureReply(IntPtr physicalMonitor, byte vcpCode, IntPtr codeType, out uint currentValue, out uint maximumValue);

        #endregion
    }
}
