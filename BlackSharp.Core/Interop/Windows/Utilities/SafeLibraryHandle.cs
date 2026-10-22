// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Windows.Native;
using Microsoft.Win32.SafeHandles;

namespace BlackSharp.Core.Interop.Windows.Utilities
{
    /// <summary>
    /// Provides a safe handle for a native Windows library.
    /// </summary>
    public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Constructor

        /// <summary>
        /// Initializes an invalid library handle that owns its native resource.
        /// </summary>
        public SafeLibraryHandle()
            : base(true)
        {
        }

        /// <summary>
        /// Initializes a library handle from an existing native handle.
        /// </summary>
        /// <param name="existingHandle">The existing native library handle.</param>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to release the native library when this instance is disposed.
        /// </param>
        public SafeLibraryHandle(IntPtr existingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            return Kernel32.FreeLibrary(handle);
        }

        #endregion
    }
}
