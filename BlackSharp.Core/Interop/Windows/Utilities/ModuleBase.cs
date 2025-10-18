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
    /// Base class to allow dynamic loading of a "module" which represents a native library.<br/>
    /// This is done via <see cref="Kernel32.LoadLibrary"/>, <see cref="Kernel32.GetProcAddress"/> and <see cref="Kernel32.FreeLibrary"/>.
    /// </summary>
    /// <seealso cref="Kernel32.LoadLibrary"/>.
    public abstract class ModuleBase : IDisposable
    {
        #region Constructor

        public ModuleBase()
        {
            var moduleFilename = GetModuleFilename();

            _Module = Kernel32.LoadLibrary(moduleFilename);

            if (SafeFileHandler.IsHandleValid(_Module) && LoadLibraryFunctions())
            {
                IsModuleLoaded = true;
            }
        }

        #endregion

        #region Fields

        bool _Disposed;

        IntPtr _Module;

        #endregion

        #region Properties

        public bool IsModuleLoaded { get; private set; }

        #endregion

        #region Public

        public virtual void Dispose()
        {
            if (!_Disposed)
            {
                if (_Module != IntPtr.Zero)
                {
                    Kernel32.FreeLibrary(_Module);
                }

                _Disposed = true;
            }
        }

        #endregion

        #region Abstract

        protected abstract string GetModuleFilename();

        protected abstract bool LoadLibraryFunctions();

        #endregion

        #region Protected

        protected T GetDelegate<T>(string procName)
            where T : Delegate
        {
            return DynamicLoader.GetDelegate<T>(_Module, procName);
        }

        #endregion
    }
}
