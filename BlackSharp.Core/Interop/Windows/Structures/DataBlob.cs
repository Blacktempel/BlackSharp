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
    /// Describes an unmanaged byte buffer passed to Windows data-protection functions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DataBlob
    {
        #region Fields

        /// <summary>
        /// Length of the buffer in bytes.
        /// </summary>
        public int DataLength;

        /// <summary>
        /// Address of the buffer.
        /// </summary>
        public IntPtr Data;

        #endregion
    }
}
