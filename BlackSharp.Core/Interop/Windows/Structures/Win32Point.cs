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
    /// Represents a native Windows point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        #region Fields

        /// <summary>
        /// Contains the horizontal coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// Contains the vertical coordinate.
        /// </summary>
        public int Y;

        #endregion
    }
}
