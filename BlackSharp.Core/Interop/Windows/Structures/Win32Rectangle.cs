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
    /// Represents a native Windows rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Rectangle
    {
        #region Fields

        /// <summary>
        /// Contains the left coordinate.
        /// </summary>
        public int Left;

        /// <summary>
        /// Contains the top coordinate.
        /// </summary>
        public int Top;

        /// <summary>
        /// Contains the right coordinate.
        /// </summary>
        public int Right;

        /// <summary>
        /// Contains the bottom coordinate.
        /// </summary>
        public int Bottom;

        #endregion
    }
}
