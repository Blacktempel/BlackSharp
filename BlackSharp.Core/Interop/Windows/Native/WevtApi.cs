// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Imports Windows Event Log query and rendering functions.
    /// </summary>
    public static class WevtApi
    {
        #region Fields

        private const string DLL_NAME = "wevtapi.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Closes an Event Log handle.
        /// </summary>
        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EvtClose(IntPtr handle);

        /// <summary>
        /// Reads events from a query result.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EvtNext(IntPtr resultSet, int eventArraySize, [Out] IntPtr[] events, int timeout, int flags, out int returned);

        /// <summary>
        /// Creates a query against a channel or event log file.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr EvtQuery(IntPtr session, string path, string query, int flags);

        /// <summary>
        /// Renders an event fragment into a supplied buffer.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EvtRender(IntPtr context, IntPtr fragment, int flags, int bufferSize, IntPtr buffer, out int bufferUsed, out int propertyCount);

        #endregion
    }
}
