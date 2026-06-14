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
    public enum FileFlagsAndAttributes : uint
    {
        None = 0,

        //File attributes
        Readonly            = 0x00000001,
        Hidden              = 0x00000002,
        System              = 0x00000004,
        Directory           = 0x00000010,
        Archive             = 0x00000020,
        Device              = 0x00000040,
        Normal              = 0x00000080,
        Temporary           = 0x00000100,
        SparseFile          = 0x00000200,
        ReparsePoint        = 0x00000400,
        Compressed          = 0x00000800,
        Offline             = 0x00001000,
        NotContentIndexed   = 0x00002000,
        Encrypted           = 0x00004000,
        IntegrityStream     = 0x00008000,
        Virtual             = 0x00010000,
        NoScrubData         = 0x00020000,

        //File flags
        WriteThrough        = 0x80000000,
        Overlapped          = 0x40000000,
        NoBuffering         = 0x20000000,
        RandomAccess        = 0x10000000,
        SequentialScan      = 0x08000000,
        DeleteOnClose       = 0x04000000,
        BackupSemantics     = 0x02000000,
        PosixSemantics      = 0x01000000,
        SessionAware        = 0x00800000,
        OpenReparsePoint    = 0x00200000,
        OpenNoRecall        = 0x00100000,
        FirstPipeInstance   = 0x00080000,

        //Security QoS flags - also passed via dwFlagsAndAttributes
        SecurityAnonymous       = 0x00000000,
        SecurityIdentification  = 0x00010000,
        SecurityImpersonation   = 0x00020000,
        SecurityDelegation      = 0x00030000,
        SecurityContextTracking = 0x00040000,
        SecurityEffectiveOnly   = 0x00080000,
        SecuritySqosPresent     = 0x00100000,
    }
}
