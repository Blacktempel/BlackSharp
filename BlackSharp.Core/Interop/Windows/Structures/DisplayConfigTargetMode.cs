// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes the signal mode of a Windows display target.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigTargetMode
    {
        #region Fields

        /// <summary>
        /// Contains the target video signal information.
        /// </summary>
        public DisplayConfigVideoSignalInfo TargetVideoSignalInfo;

        #endregion
    }
}
