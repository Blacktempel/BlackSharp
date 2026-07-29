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
    /// Defines generic, standard, and file-specific Windows access rights.
    /// </summary>
    [Flags]
    public enum DesiredAccess : uint
    {
        /// <summary>No access rights.</summary>
        None = 0,

        // Generic access rights
        /// <summary>Generic read access.</summary>
        GenericRead    = 0x80000000,
        /// <summary>Generic write access.</summary>
        GenericWrite   = 0x40000000,
        /// <summary>Generic execute access.</summary>
        GenericExecute = 0x20000000,
        /// <summary>All generic access rights.</summary>
        GenericAll     = 0x10000000,

        // Standard access rights
        /// <summary>Permission to delete the object.</summary>
        Delete      = 0x00010000,
        /// <summary>Permission to read the object's security descriptor.</summary>
        ReadControl = 0x00020000,
        /// <summary>Permission to modify the object's discretionary access-control list.</summary>
        WriteDac    = 0x00040000,
        /// <summary>Permission to change the object's owner.</summary>
        WriteOwner  = 0x00080000,
        /// <summary>Permission to use the object for synchronization.</summary>
        Synchronize = 0x00100000,

        // File-specific access rights
        /// <summary>Permission to read file data.</summary>
        FileReadData        = 0x00000001,
        /// <summary>Permission to list directory entries.</summary>
        FileListDirectory   = 0x00000001,

        /// <summary>Permission to write file data.</summary>
        FileWriteData       = 0x00000002,
        /// <summary>Permission to add a file to a directory.</summary>
        FileAddFile         = 0x00000002,

        /// <summary>Permission to append file data.</summary>
        FileAppendData      = 0x00000004,
        /// <summary>Permission to add a subdirectory.</summary>
        FileAddSubdirectory = 0x00000004,

        /// <summary>Permission to read extended attributes.</summary>
        FileReadEa          = 0x00000008,
        /// <summary>Permission to write extended attributes.</summary>
        FileWriteEa         = 0x00000010,

        /// <summary>Permission to execute a file.</summary>
        FileExecute         = 0x00000020,
        /// <summary>Permission to traverse a directory.</summary>
        FileTraverse        = 0x00000020,

        /// <summary>Permission to delete child objects.</summary>
        FileDeleteChild     = 0x00000040,
        /// <summary>Permission to read file attributes.</summary>
        FileReadAttributes  = 0x00000080,
        /// <summary>Permission to write file attributes.</summary>
        FileWriteAttributes = 0x00000100,

        /// <summary>All file access rights.</summary>
        FileAllAccess = 0x001F01FF
    }
}
