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
