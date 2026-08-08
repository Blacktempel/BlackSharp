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
