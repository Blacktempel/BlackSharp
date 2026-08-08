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
    /// Describes a source in a Windows display path.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigPathSourceInfo
    {
        #region Fields

        /// <summary>
        /// Identifies the source adapter.
        /// </summary>
        public DisplayConfigLuid AdapterID;

        /// <summary>
        /// Identifies the source on the adapter.
        /// </summary>
        public uint ID;

        /// <summary>
        /// Contains the related mode information index.
        /// </summary>
        public uint ModeInfoIndex;

        /// <summary>
        /// Contains source status flags.
        /// </summary>
        public uint StatusFlags;

        #endregion
    }
}
