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
