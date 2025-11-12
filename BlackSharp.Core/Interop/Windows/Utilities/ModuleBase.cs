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

        /// <summary>
        /// Constructs a new object, loads module based on <see cref="GetModuleFilename"/> and loads functions via <see cref="LoadLibraryFunctions"/>.
        /// </summary>
        /// <remarks>If successful, <see cref="IsModuleLoaded"/> is set to true.</remarks>
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

        /// <summary>
        /// Identifies whether the module was loaded properly.
        /// </summary>
        public bool IsModuleLoaded { get; private set; }

        #endregion

        #region Public

        /// <summary>
        /// Unloads the module.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_Disposed)
            {
                if (_Module != IntPtr.Zero)
                {
                    Kernel32.FreeLibrary(_Module);
                    _Module = IntPtr.Zero;
                }

                _Disposed = true;
            }
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Gets filename of module to load via <see cref="Kernel32.LoadLibrary"/>.
        /// </summary>
        /// <returns>Filename of module</returns>
        protected abstract string GetModuleFilename();

        /// <summary>
        /// Loads functions of loaded module.
        /// </summary>
        /// <returns>Returns boolean value to determine whether all necessary functions were loaded correctly.</returns>
        /// <remarks>Use <see cref="GetDelegate"/> for simple loading.</remarks>
        protected abstract bool LoadLibraryFunctions();

        #endregion

        #region Protected

        /// <summary>
        /// Load function via <see cref="DynamicLoader"/> and return delegate type, ready for use.
        /// </summary>
        /// <typeparam name="T">Delegate type for given function.</typeparam>
        /// <param name="procName">Function to load.</param>
        /// <returns><inheritdoc cref="DynamicLoader.GetDelegate"/></returns>
        protected T GetDelegate<T>(string procName)
            where T : Delegate
        {
            return DynamicLoader.GetDelegate<T>(_Module, procName);
        }

        #endregion
    }
}
