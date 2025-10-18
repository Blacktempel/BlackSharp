/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Interop.Windows.Native;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Utilities
{
    /// <summary>
    /// Helper class for dynamic loading of native functions via <see cref="Kernel32.GetProcAddress"/>.
    /// </summary>
    public static class DynamicLoader
    {
        #region Public

        public static T GetDelegate<T>(IntPtr module, string procName)
            where T : Delegate
        {
            return GetDelegate(module, procName, typeof(T)) as T;
        }

        public static Delegate GetDelegate(IntPtr module, string procName, Type delegateType)
        {
            IntPtr ptr = Kernel32.GetProcAddress(module, procName);
            if (ptr != IntPtr.Zero)
            {
                Delegate d = Marshal.GetDelegateForFunctionPointer(ptr, delegateType);
                return d;
            }

            int result = Marshal.GetHRForLastWin32Error();
            throw Marshal.GetExceptionForHR(result);
        }

        #endregion
    }
}
