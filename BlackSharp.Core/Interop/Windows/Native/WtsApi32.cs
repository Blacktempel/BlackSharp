// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Imports Windows Terminal Services session enumeration functions.
    /// </summary>
    public static class WtsApi32
    {
        #region Fields

        private const string DLL_NAME = "wtsapi32.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Enumerates sessions known to a Terminal Services server.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSEnumerateSessions(IntPtr server, int reserved, int version, out IntPtr sessions, out int count);

        /// <summary>
        /// Releases memory returned by a Terminal Services function.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern void WTSFreeMemory(IntPtr pointer);

        #endregion
    }
}
