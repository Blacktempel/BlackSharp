/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Interop.Windows.Mutexes
{
    /// <summary>
    /// Mutex guard for automatic lock and unlock of a <see cref="WorldMutex"/> via <see cref="IDisposable"/>.
    /// </summary>
    public sealed class WorldMutexGuard : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructs a new object and locks given mutex.
        /// </summary>
        /// <param name="worldMutex">Mutex to automatically lock and unlock.</param>
        public WorldMutexGuard(WorldMutex worldMutex)
        {
            _WorldMutex = worldMutex;

            _WorldMutex.Lock();
        }

        #endregion

        #region Fields

        WorldMutex _WorldMutex;

        #endregion

        #region Public

        /// <summary>
        /// Unlocks mutex which has been passed in constructor.
        /// </summary>
        public void Dispose()
        {
            _WorldMutex.Unlock();
        }

        #endregion
    }
}
