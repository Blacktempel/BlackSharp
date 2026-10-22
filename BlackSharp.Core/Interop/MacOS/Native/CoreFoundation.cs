// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.MacOS.Native
{
    /// <summary>
    /// Provides selected native Core Foundation operations.
    /// </summary>
    public static class CoreFoundation
    {
        #region Fields

        const string LIBRARY_NAME =
            "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

        #endregion

        #region Imports

        /// <summary>
        /// Returns the number of values in a Core Foundation array.
        /// </summary>
        [DllImport(LIBRARY_NAME)]
        public static extern IntPtr CFArrayGetCount(IntPtr array);

        /// <summary>
        /// Returns the value at the specified Core Foundation array index.
        /// </summary>
        [DllImport(LIBRARY_NAME)]
        public static extern IntPtr CFArrayGetValueAtIndex(IntPtr array, IntPtr index);

        /// <summary>
        /// Releases a retained Core Foundation object.
        /// </summary>
        [DllImport(LIBRARY_NAME)]
        public static extern void CFRelease(IntPtr value);

        #endregion
    }
}
