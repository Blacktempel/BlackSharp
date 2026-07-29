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
    /// Describes ownership and access-control information for a Windows object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityDescriptor
    {
        /// <summary>
        /// The security-descriptor revision.
        /// </summary>
        public byte Revision;

        /// <summary>
        /// A value reserved for alignment.
        /// </summary>
        public byte Size;

        /// <summary>
        /// Control flags describing the descriptor.
        /// </summary>
        public short Control;

        /// <summary>
        /// A pointer to the owner's security identifier.
        /// </summary>
        public IntPtr Owner;

        /// <summary>
        /// A pointer to the primary group's security identifier.
        /// </summary>
        public IntPtr Group;

        /// <summary>
        /// A pointer to the system access-control list.
        /// </summary>
        public IntPtr Sacl;

        /// <summary>
        /// A pointer to the discretionary access-control list.
        /// </summary>
        public IntPtr Dacl;
    }
}
