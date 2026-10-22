// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Contains Windows code-integrity options returned by NtQuerySystemInformation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemCodeIntegrityInformation
    {
        #region Fields

        /// <summary>
        /// Gets the structure length in bytes.
        /// </summary>
        public uint Length;

        /// <summary>
        /// Gets the code-integrity option flags.
        /// </summary>
        public uint Options;

        #endregion
    }
}
