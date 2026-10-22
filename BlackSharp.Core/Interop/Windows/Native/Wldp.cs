// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Imports Windows Lockdown Policy functions.
    /// </summary>
    public static class Wldp
    {
        #region Fields

        private const string DLL_NAME = "wldp.dll";

        #endregion

        #region Imports

        /// <summary>
        /// Retrieves the lockdown policy for a host.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int WldpGetLockdownPolicy(ref WldpHostInformation hostInformation, out uint lockdownState, uint lockdownFlags);

        #endregion
    }
}
