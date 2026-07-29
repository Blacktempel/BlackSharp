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
    /// Supplies a security descriptor and handle-inheritance setting when creating a Windows object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityAttributes
    {
        /// <summary>
        /// The size of this structure in bytes.
        /// </summary>
        public uint Length;

        /// <summary>
        /// A pointer to the security descriptor to apply.
        /// </summary>
        public IntPtr SecurityDescriptor;

        /// <summary>
        /// Indicates whether child processes inherit the returned handle.
        /// </summary>
        public bool InheritHandle;
    }
}
