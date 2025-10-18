/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

namespace BlackSharp.Core.Asynchronous
{
    /// <summary>
    /// Mutex guard for automatic lock and unlock via <see cref="IDisposable"/>.
    /// </summary>
    public class LockGuard : IDisposable
    {
        #region Constructor

        public LockGuard(object lockObject)
        {
            _LockObject = lockObject;

            Monitor.Enter(_LockObject);
        }

        #endregion

        #region Fields

        object _LockObject;

        #endregion

        #region Public

        public void Dispose()
        {
            Monitor.Exit(_LockObject);
        }

        #endregion
    }
}
