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
    /// Defines how a Windows file-opening operation handles existing and missing files.
    /// </summary>
    public enum FileCreationDisposition : uint
    {
        /// <summary>Creates a new file and fails if it already exists.</summary>
        CreateNew        = 1,
        /// <summary>Creates a file and overwrites an existing file.</summary>
        CreateAlways     = 2,
        /// <summary>Opens an existing file and fails if it does not exist.</summary>
        OpenExisting     = 3,
        /// <summary>Opens an existing file or creates a new one.</summary>
        OpenAlways       = 4,
        /// <summary>Opens and truncates an existing file.</summary>
        TruncateExisting = 5
    }
}
