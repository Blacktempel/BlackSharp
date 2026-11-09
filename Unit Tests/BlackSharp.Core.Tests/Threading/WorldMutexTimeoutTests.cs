// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Windows.Mutexes;
using System.Diagnostics;

namespace BlackSharp.Core.Tests.Threading
{
    /// <summary>
    /// Verifies bounded mutex waits and release of ownership transferred by an abandoned mutex.
    /// </summary>
    [TestClass]
    public sealed class WorldMutexTimeoutTests
    {
        #region Public

        /// <summary>
        /// Verifies that contention returns within the timeout and acquisition succeeds after the owner releases the mutex.
        /// </summary>
        [TestMethod]
        public void BusyMutexTimesOutAndCanBeAcquiredAfterRelease()
        {
            // Arrange
            using var mutex   = new WorldMutex("BlackSharp_Test_" + Guid.NewGuid().ToString("N"));
            using var locked  = new ManualResetEventSlim();
            using var release = new ManualResetEventSlim();

            var owner = new Thread(() =>
            {
                if (!mutex.Lock())
                {
                    return;
                }

                locked.Set();
                release.Wait(TimeSpan.FromSeconds(10));
                mutex.Unlock();
            });

            owner.Start();

            try
            {
                Assert.IsTrue(locked.Wait(TimeSpan.FromSeconds(5)));

                // Act
                var stopwatch = Stopwatch.StartNew();
                var acquired  = mutex.Lock(50);
                var elapsed   = stopwatch.Elapsed;

                // Assert
                Assert.IsFalse(acquired);
                Assert.IsTrue(elapsed < TimeSpan.FromSeconds(2));
            }
            finally
            {
                // Cleanup
                release.Set();

                Assert.IsTrue(owner.Join(TimeSpan.FromSeconds(5)));
            }

            // Act
            var acquiredAfterRelease = mutex.Lock(100);

            // Assert
            Assert.IsTrue(acquiredAfterRelease);

            // Cleanup
            mutex.Unlock();
        }

        /// <summary>
        /// Verifies that a guard owns and releases an abandoned mutex so another thread can acquire it afterwards.
        /// </summary>
        [TestMethod]
        public void AbandonedMutexOwnershipIsReleasedByGuard()
        {
            // Arrange
            using var mutex = new WorldMutex("BlackSharp_Abandoned_" + Guid.NewGuid().ToString("N"));

            var owner = new Thread(() => Assert.IsTrue(mutex.Lock()));

            owner.Start();

            Assert.IsTrue(owner.Join(TimeSpan.FromSeconds(5)));

            // Act and Assert
            using (var guard = new WorldMutexGuard(mutex))
            {
                Assert.IsTrue(guard.IsLockAcquired);
            }

            // Arrange
            bool acquired = false;

            var next = new Thread(() =>
            {
                acquired = mutex.Lock(100);

                if (acquired)
                {
                    mutex.Unlock();
                }
            });

            // Act
            next.Start();

            // Assert
            Assert.IsTrue(next.Join(TimeSpan.FromSeconds(5)));
            Assert.IsTrue(acquired);
        }

        #endregion
    }
}
