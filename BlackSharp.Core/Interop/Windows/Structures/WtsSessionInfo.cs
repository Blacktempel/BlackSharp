// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes one Windows Terminal Services session.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WtsSessionInfo
    {
        #region Fields

        /// <summary>
        /// Identifies the session.
        /// </summary>
        public int SessionID;

        /// <summary>
        /// Points to the native station name.
        /// </summary>
        public IntPtr StationName;

        /// <summary>
        /// Identifies the connection state.
        /// </summary>
        public int State;

        #endregion
    }
}
