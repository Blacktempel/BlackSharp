/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

namespace BlackSharp.Core.Interop.Windows.Enums
{
    [Flags]
    public enum FileShareMode : uint
    {
        None = 0,

        Read   = 0x00000001,
        Write  = 0x00000002,
        Delete = 0x00000004
    }
}
