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
    /// Represents a Windows locally unique identifier used by the display configuration API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigLuid
    {
        #region Fields

        /// <summary>
        /// Contains the low-order part of the identifier.
        /// </summary>
        public uint LowPart;

        /// <summary>
        /// Contains the high-order part of the identifier.
        /// </summary>
        public int HighPart;

        #endregion
    }
}
