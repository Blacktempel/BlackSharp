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
    /// Describes a device interface returned by the Windows Setup API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInterfaceData
    {
        /// <summary>
        /// Gets or sets the structure size.
        /// </summary>
        public int Size;

        /// <summary>
        /// Gets or sets the interface class identifier.
        /// </summary>
        public Guid InterfaceClassGuid;

        /// <summary>
        /// Gets or sets the interface flags.
        /// </summary>
        public int Flags;

        /// <summary>
        /// Gets or sets the reserved native value.
        /// </summary>
        public IntPtr Reserved;
    }
}
