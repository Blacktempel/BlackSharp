// SPDX-License-Identifier: MPL-2.0

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
