/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Interop.Windows.Native;

namespace BlackSharp.Core.Interop.Windows.Utilities
{
    /// <summary>
    /// Abstracted low level file operations via <see cref="Kernel32"/>.
    /// </summary>
    public static class SafeFileHandler
    {
        #region Public

        public static IntPtr OpenHandle(string physicalPath)
        {
            return Kernel32.CreateFile(physicalPath, Kernel32.GENERIC_READ | Kernel32.GENERIC_WRITE, Kernel32.FILE_SHARE_READ | Kernel32.FILE_SHARE_WRITE, IntPtr.Zero, Kernel32.OPEN_EXISTING, 0, IntPtr.Zero);
        }

        public static void CloseHandle(IntPtr handle)
        {
            Kernel32.CloseHandle(handle);
        }

        public static bool IsHandleValid(IntPtr handle)
        {
            return handle != IntPtr.Zero
                && handle != Kernel32.InvalidHandle;
        }

        #endregion
    }
}
