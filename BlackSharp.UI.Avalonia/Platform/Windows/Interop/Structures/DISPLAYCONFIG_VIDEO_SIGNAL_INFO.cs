/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.UI.Avalonia.Platform.Windows.Interop.Enums;
using System.Runtime.InteropServices;

namespace BlackSharp.UI.Avalonia.Platform.Windows.Interop.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
    {
        public ulong pixelRate;
        public DISPLAYCONFIG_RATIONAL hSyncFreq;
        public DISPLAYCONFIG_RATIONAL vSyncFreq;
        public DISPLAYCONFIG_2DREGION activeSize;
        public DISPLAYCONFIG_2DREGION totalSize;

        public uint videoStandard;

        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    }
}
