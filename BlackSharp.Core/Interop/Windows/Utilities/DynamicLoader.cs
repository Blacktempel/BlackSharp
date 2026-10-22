// SPDX-License-Identifier: MPL-2.0

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

        /// <inheritdoc cref="GetDelegate(IntPtr, string, Type)"/>
        public static T GetDelegate<T>(IntPtr module, string procName)
            where T : Delegate
        {
            return GetDelegate(module, procName, typeof(T)) as T;
        }

        /// <summary>
        /// Loads function from given module and casts it to specified delegate.
        /// </summary>
        /// <param name="module">Module to load function from.</param>
        /// <param name="procName">Name of function to load.</param>
        /// <param name="delegateType">Type of delegate to cast to.</param>
        /// <returns>Returns loaded and casted delegate, ready for use.<br/>
        /// May return null if signature of delegate did not match function to load.</returns>
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
