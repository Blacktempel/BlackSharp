// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes a Windows display path.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigPathInfo
    {
        #region Fields

        /// <summary>
        /// Describes the display source.
        /// </summary>
        public DisplayConfigPathSourceInfo SourceInfo;

        /// <summary>
        /// Describes the display target.
        /// </summary>
        public DisplayConfigPathTargetInfo TargetInfo;

        /// <summary>
        /// Contains path flags.
        /// </summary>
        public uint Flags;

        #endregion
    }
}
