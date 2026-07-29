/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides Windows trust-provider verification functions.
    /// </summary>
    public static class WinTrust
    {
        #region Fields

        const string DLL_NAME = "wintrust.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Evaluates trust for the supplied subject according to the selected policy.
        /// </summary>
        [DllImport(DLL_NAME, ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        public static extern uint WinVerifyTrust(IntPtr hwnd, ref Guid pgActionID, ref WinTrustData pWVTData);

        #endregion
    }
}
