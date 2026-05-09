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
    public enum FileCreationDisposition : uint
    {
        CreateNew        = 1,
        CreateAlways     = 2,
        OpenExisting     = 3,
        OpenAlways       = 4,
        TruncateExisting = 5
    }
}
