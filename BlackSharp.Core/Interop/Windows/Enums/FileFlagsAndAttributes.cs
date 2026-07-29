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
    /// Defines Windows file attributes, open flags, and security quality-of-service options.
    /// </summary>
    [Flags]
    public enum FileFlagsAndAttributes : uint
    {
        /// <summary>No attributes or flags.</summary>
        None = 0,

        //File attributes
        /// <summary>Marks a file as read-only.</summary>
        Readonly            = 0x00000001,
        /// <summary>Marks a file as hidden.</summary>
        Hidden              = 0x00000002,
        /// <summary>Marks a file as used by the operating system.</summary>
        System              = 0x00000004,
        /// <summary>Identifies a directory.</summary>
        Directory           = 0x00000010,
        /// <summary>Marks a file for archival.</summary>
        Archive             = 0x00000020,
        /// <summary>Identifies a reserved device attribute.</summary>
        Device              = 0x00000040,
        /// <summary>Identifies a file without other attributes.</summary>
        Normal              = 0x00000080,
        /// <summary>Marks a file as temporary.</summary>
        Temporary           = 0x00000100,
        /// <summary>Identifies a sparse file.</summary>
        SparseFile          = 0x00000200,
        /// <summary>Identifies a reparse point.</summary>
        ReparsePoint        = 0x00000400,
        /// <summary>Identifies a compressed file or directory.</summary>
        Compressed          = 0x00000800,
        /// <summary>Marks file data as not immediately available.</summary>
        Offline             = 0x00001000,
        /// <summary>Excludes a file from content indexing.</summary>
        NotContentIndexed   = 0x00002000,
        /// <summary>Identifies an encrypted file or directory.</summary>
        Encrypted           = 0x00004000,
        /// <summary>Enables integrity support for a file or directory.</summary>
        IntegrityStream     = 0x00008000,
        /// <summary>Identifies a virtual file attribute.</summary>
        Virtual             = 0x00010000,
        /// <summary>Excludes the data stream from integrity scrubbing.</summary>
        NoScrubData         = 0x00020000,

        //File flags
        /// <summary>Writes data through intermediate caches.</summary>
        WriteThrough        = 0x80000000,
        /// <summary>Enables asynchronous file operations.</summary>
        Overlapped          = 0x40000000,
        /// <summary>Disables system caching for file data.</summary>
        NoBuffering         = 0x20000000,
        /// <summary>Hints that file access will be random.</summary>
        RandomAccess        = 0x10000000,
        /// <summary>Hints that file access will be sequential.</summary>
        SequentialScan      = 0x08000000,
        /// <summary>Deletes the file when its final handle closes.</summary>
        DeleteOnClose       = 0x04000000,
        /// <summary>Allows backup-oriented access to directories and protected files.</summary>
        BackupSemantics     = 0x02000000,
        /// <summary>Uses POSIX-compatible access semantics where supported.</summary>
        PosixSemantics      = 0x01000000,
        /// <summary>Restricts a file handle to the creating session.</summary>
        SessionAware        = 0x00800000,
        /// <summary>Opens a reparse point instead of its target.</summary>
        OpenReparsePoint    = 0x00200000,
        /// <summary>Prevents recall of offline file data.</summary>
        OpenNoRecall        = 0x00100000,
        /// <summary>Requires creation of the first instance of a named pipe.</summary>
        FirstPipeInstance   = 0x00080000,

        //Security QoS flags - also passed via dwFlagsAndAttributes
        /// <summary>Uses anonymous impersonation.</summary>
        SecurityAnonymous       = 0x00000000,
        /// <summary>Allows identification of the caller without impersonation.</summary>
        SecurityIdentification  = 0x00010000,
        /// <summary>Allows impersonation of the caller on the local system.</summary>
        SecurityImpersonation   = 0x00020000,
        /// <summary>Allows delegated impersonation where permitted.</summary>
        SecurityDelegation      = 0x00030000,
        /// <summary>Tracks the client's security context dynamically.</summary>
        SecurityContextTracking = 0x00040000,
        /// <summary>Uses only enabled privileges from the client context.</summary>
        SecurityEffectiveOnly   = 0x00080000,
        /// <summary>Indicates that security quality-of-service flags are present.</summary>
        SecuritySqosPresent     = 0x00100000,
    }
}
