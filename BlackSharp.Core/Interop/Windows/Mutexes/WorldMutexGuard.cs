// SPDX-License-Identifier: MPL-2.0

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
            _WorldMutex   = worldMutex ?? throw new ArgumentNullException(nameof(worldMutex));
            _LockAcquired = _WorldMutex.Lock();
        }

        #endregion

        #region Fields

        WorldMutex _WorldMutex;
        bool       _LockAcquired;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the mutex was acquired successfully.
        /// </summary>
        public bool IsLockAcquired => _LockAcquired;

        #endregion

        #region Public

        /// <summary>
        /// Unlocks mutex which has been passed in constructor.
        /// </summary>
        public void Dispose()
        {
            if (!_LockAcquired)
            {
                return;
            }

            _LockAcquired = false;

            _WorldMutex.Unlock();
        }

        #endregion
    }
}
