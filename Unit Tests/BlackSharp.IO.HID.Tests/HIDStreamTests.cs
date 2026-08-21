/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Diagnostics;

namespace BlackSharp.IO.HID.Tests;

/// <summary>
/// Verifies the behavior of HID stream.
/// </summary>
[TestClass]
public sealed class HIDStreamTests
{
    #region Public

    /// <summary>
    /// Verifies that dispose immediately wakes blocked workers.
    /// </summary>
    [TestMethod]
    public void DisposeImmediatelyWakesBlockedWorkers()
    {
        // Arrange
        var transport = new TestTransport();
        var device = new HIDDevice("test", 64, 64);
        var stream = new HIDStream(device, transport);

        // Act and assert
        Assert.IsTrue(transport.ReadEntered.Wait(TimeSpan.FromSeconds(2)));

        var stopwatch = Stopwatch.StartNew();

        stream.Dispose();

        stopwatch.Stop();

        Assert.IsTrue(transport.CancelCalled);
        Assert.IsTrue(
            stopwatch.ElapsedMilliseconds < 200,
            $"Dispose took {stopwatch.ElapsedMilliseconds} ms.");
    }

    /// <summary>
    /// Verifies that concurrent writes are serialized on one worker.
    /// </summary>
    [TestMethod]
    public void ConcurrentWritesAreSerializedOnOneWorker()
    {
        // Arrange
        var transport = new TestTransport();
        var device = new HIDDevice("test", 64, 64);

        using var stream = new HIDStream(device, transport);

        var first = Task.Run(() => stream.Write(new byte[64]));

        // Act
        var second = Task.Run(() => stream.Write(new byte[64]));

        // Assert
        Assert.IsTrue(Task.WaitAll(new[] { first, second }, TimeSpan.FromSeconds(2)));
        Assert.AreEqual(2, transport.WriteCount);
        Assert.AreEqual(1, transport.MaximumConcurrentWrites);
        Assert.AreEqual(1, transport.WriteThreadIDs.Count);
    }

    /// <summary>
    /// Verifies that queued write is not sent after its timeout.
    /// </summary>
    [TestMethod]
    public void QueuedWriteIsNotSentAfterItsTimeout()
    {
        // Arrange
        var transport = new TestTransport
        {
            BlockFirstWrite = true,
        };
        var device = new HIDDevice("test", 64, 64);

        using var stream = new HIDStream(device, transport)
        {
            WriteTimeout = 1000,
        };

        // Act
        var first = Task.Run(() => stream.Write(new byte[64]));

        // Assert
        Assert.IsTrue(transport.FirstWriteEntered.Wait(TimeSpan.FromSeconds(2)));

        stream.WriteTimeout = 50;

        var second = Task.Run(() => stream.Write(new byte[64]));

        Thread.Sleep(100);
        transport.ReleaseFirstWrite.Set();

        Assert.IsTrue(first.Wait(TimeSpan.FromSeconds(2)));
        Assert.ThrowsExactly<TimeoutException>(() => second.GetAwaiter().GetResult());
        Assert.AreEqual(1, transport.WriteCount);
    }

    /// <summary>
    /// Verifies that output only device does not start A read worker and pads reports.
    /// </summary>
    [TestMethod]
    public void OutputOnlyDeviceDoesNotStartAReadWorkerAndPadsReports()
    {
        // Arrange
        var transport = new TestTransport();
        var device = new HIDDevice("test", 0, 8);

        using var stream = new HIDStream(device, transport);

        // Act
        stream.Write(new byte[] { 1, 2, 3 });

        // Assert
        Assert.IsFalse(transport.ReadEntered.IsSet);
        CollectionAssert.AreEqual(
            new byte[] { 1, 2, 3, 0, 0, 0, 0, 0 },
            transport.LastWrite);
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Provides the test transport implementation used by the hardware monitoring system.
    /// </summary>
    private sealed class TestTransport : IHIDTransport
    {
        #region Fields

        /// <summary>
        /// Stores the cancelled state associated with this instance.
        /// </summary>
        private readonly ManualResetEventSlim _cancelled = new(false);

        /// <summary>
        /// Synchronizes access to state shared by concurrent operations.
        /// </summary>
        private readonly object _writeSync = new();

        /// <summary>
        /// Stores the active writes state associated with this instance.
        /// </summary>
        private int _activeWrites;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the read entered.
        /// </summary>
        public ManualResetEventSlim ReadEntered { get; } = new(false);

        /// <summary>
        /// Gets the first write entered.
        /// </summary>
        public ManualResetEventSlim FirstWriteEntered { get; } = new(false);

        /// <summary>
        /// Gets the release first write.
        /// </summary>
        public ManualResetEventSlim ReleaseFirstWrite { get; } = new(false);

        /// <summary>
        /// Gets or sets the block first write.
        /// </summary>
        public bool BlockFirstWrite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cel called can be performed.
        /// </summary>
        public bool CancelCalled { get; private set; }

        /// <summary>
        /// Gets or sets the maximum concurrent writes.
        /// </summary>
        public int MaximumConcurrentWrites { get; private set; }

        /// <summary>
        /// Gets or sets the write count.
        /// </summary>
        public int WriteCount { get; private set; }

        /// <summary>
        /// Gets or sets the last write associated with this instance.
        /// </summary>
        public byte[] LastWrite { get; private set; }

        /// <summary>
        /// Gets the write thread IDs.
        /// </summary>
        public HashSet<int> WriteThreadIDs { get; } = new();

        #endregion

        #region Public

        /// <inheritdoc/>
        public void Cancel()
        {
            CancelCalled = true;
            _cancelled.Set();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _cancelled.Dispose();
            FirstWriteEntered.Dispose();
            ReadEntered.Dispose();
            ReleaseFirstWrite.Dispose();
        }

        /// <inheritdoc/>
        public void GetFeature(byte[] buffer)
        {
        }

        /// <inheritdoc/>
        public int Read(byte[] buffer)
        {
            ReadEntered.Set();
            _cancelled.Wait();

            throw new OperationCanceledException();
        }

        /// <inheritdoc/>
        public void SetFeature(byte[] buffer)
        {
        }

        /// <inheritdoc/>
        public int Write(byte[] buffer, int timeoutMilliseconds)
        {
            lock (_writeSync)
            {
                ++_activeWrites;
                ++WriteCount;

                MaximumConcurrentWrites = Math.Max(MaximumConcurrentWrites, _activeWrites);
                WriteThreadIDs.Add(Environment.CurrentManagedThreadId);
                LastWrite = (byte[])buffer.Clone();
            }

            if (WriteCount == 1 && BlockFirstWrite)
            {
                FirstWriteEntered.Set();
                ReleaseFirstWrite.Wait();
            }

            Thread.Sleep(10);

            lock (_writeSync)
            {
                --_activeWrites;
            }

            return buffer.Length;
        }

        #endregion
    }

    #endregion
}
