/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using System.Runtime.InteropServices;

namespace BlackSharp.UI.Avalonia.Platform.Windows.Interop
{
    internal static class User32
    {
        const string DLL_NAME = "user32.dll";

        [DllImport(DLL_NAME)]
        public static extern bool RegisterHotKey(IntPtr hwnd, int id, uint fsModifiers, uint vk);

        [DllImport(DLL_NAME)]
        public static extern bool UnregisterHotKey(IntPtr hwnd, int id);
    }
}
