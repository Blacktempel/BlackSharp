/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

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
