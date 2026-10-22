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
    /// Contains Windows code-integrity options returned by NtQuerySystemInformation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemCodeIntegrityInformation
    {
        #region Fields

        /// <summary>
        /// Gets the structure length in bytes.
        /// </summary>
        public uint Length;

        /// <summary>
        /// Gets the code-integrity option flags.
        /// </summary>
        public uint Options;

        #endregion
    }
}
