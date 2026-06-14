/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Interop.Windows.Enums
{
    [Flags]
    public enum DesiredAccess : uint
    {
        None = 0,

        // Generic access rights
        GenericRead    = 0x80000000,
        GenericWrite   = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll     = 0x10000000,

        // Standard access rights
        Delete      = 0x00010000,
        ReadControl = 0x00020000,
        WriteDac    = 0x00040000,
        WriteOwner  = 0x00080000,
        Synchronize = 0x00100000,

        // File-specific access rights
        FileReadData        = 0x00000001,
        FileListDirectory   = 0x00000001,

        FileWriteData       = 0x00000002,
        FileAddFile         = 0x00000002,

        FileAppendData      = 0x00000004,
        FileAddSubdirectory = 0x00000004,

        FileReadEa          = 0x00000008,
        FileWriteEa         = 0x00000010,

        FileExecute         = 0x00000020,
        FileTraverse        = 0x00000020,

        FileDeleteChild     = 0x00000040,
        FileReadAttributes  = 0x00000080,
        FileWriteAttributes = 0x00000100,

        FileAllAccess = 0x001F01FF
    }
}
