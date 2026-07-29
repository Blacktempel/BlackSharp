/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Interop.Windows.Enums
{
    /// <summary>
    /// Defines how subsequent handles may access an open file or device.
    /// </summary>
    [Flags]
    public enum FileShareMode : uint
    {
        /// <summary>Prevents sharing the file or device.</summary>
        None = 0,

        /// <summary>Allows subsequent read access.</summary>
        Read   = 0x00000001,
        /// <summary>Allows subsequent write access.</summary>
        Write  = 0x00000002,
        /// <summary>Allows subsequent delete access.</summary>
        Delete = 0x00000004,

        /// <summary>Allows subsequent read and write access.</summary>
        ReadWrite = Read | Write,

        /// <summary>Allows subsequent read, write, and delete access.</summary>
        All = Read | Write | Delete
    }
}
