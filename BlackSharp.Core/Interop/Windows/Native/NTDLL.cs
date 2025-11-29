/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;
using System.Security;

namespace BlackSharp.Core.Interop.Windows.Native
{
    internal static class NTDLL
    {
        const string DLLNAME = "ntdll.dll";

        [SecurityCritical]
        [DllImport(DLLNAME, CharSet = CharSet.Unicode)]
        public static extern uint RtlGetVersion(ref OSVERSIONINFOEX versionInfo);
    }
}
