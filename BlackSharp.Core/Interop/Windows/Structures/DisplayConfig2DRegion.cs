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
    /// Represents a two-dimensional display region.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfig2DRegion
    {
        #region Fields

        /// <summary>
        /// Contains the region width.
        /// </summary>
        public uint Width;

        /// <summary>
        /// Contains the region height.
        /// </summary>
        public uint Height;

        #endregion
    }
}
