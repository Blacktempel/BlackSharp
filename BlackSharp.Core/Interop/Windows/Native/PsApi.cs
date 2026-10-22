// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Imports Windows Process Status API functions.
    /// </summary>
    public static class PsApi
    {
        #region Fields

        private const string DLL_NAME = "psapi.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Retrieves system-wide performance and memory counters.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo(ref PerformanceInformation performanceInformation, uint size);

        #endregion
    }
}
