/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Stores the six-byte authority portion of a Windows security identifier.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SidIdentifierAuthority
    {
        /// <summary>
        /// The authority value in network byte order.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
        public byte[] Value;
    }
}
