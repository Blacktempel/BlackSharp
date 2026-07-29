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
    /// Provides selected Windows Performance Data Helper functions and status values.
    /// </summary>
    public static class Pdh
    {
        #region Fields

        const string DLL_NAME = "pdh.dll";

        /// <summary>
        /// Indicates that the supplied PDH buffer is too small.
        /// </summary>
        public const uint PdhMoreData = 0x800007D2u;

        /// <summary>
        /// Requests counter values formatted as double-precision numbers.
        /// </summary>
        public const uint PdhFormatDouble = 0x00000200u;

        #endregion

        #region Imports

        /// <summary>
        /// Adds an English-language counter path to a query.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern uint PdhAddEnglishCounter(IntPtr query, string fullCounterPath, IntPtr userData, out IntPtr counter);

        /// <summary>
        /// Closes a PDH query and releases its counters.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint PdhCloseQuery(IntPtr query);

        /// <summary>
        /// Collects the current data for every counter in a query.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint PdhCollectQueryData(IntPtr query);

        /// <summary>
        /// Retrieves the formatted values of a counter instance array.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern uint PdhGetFormattedCounterArray(IntPtr counter, uint format, ref uint bufferSize, ref uint itemCount, IntPtr itemBuffer);

        /// <summary>
        /// Creates a new PDH query.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern uint PdhOpenQuery(string dataSource, IntPtr userData, out IntPtr query);

        #endregion
    }
}
