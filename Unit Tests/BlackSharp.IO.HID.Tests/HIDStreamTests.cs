/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID.Tests;

/// <summary>
/// Verifies the behavior of HID stream.
/// </summary>
[TestClass]
public sealed class HIDStreamTests
{
    #region Public

    /// <summary>
    /// Verifies that disposal cancels a blocked read and joins the workers.
    /// </summary>
    [TestMethod]
    public void DisposeCancelsAndJoinsBlockedWorkers()
    {
        // Arrange
        var transport = new TestTransport();
        var device = new HIDDevice("test", 64, 64);
        var stream = new HIDStream(device, transport);

        Assert.IsTrue(transport.ReadEntered.Wait(TimeSpan.FromSeconds(2)));

        // Act
        var disposal = Task.Run(stream.Dispose);

        // Assert
        Assert.IsTrue(disposal.Wait(TimeSpan.FromSeconds(5)), "Dispose did not release the blocked worker.");
        Assert.IsTrue(transport.CancelCalled);
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
    /// Verifies that an output-only device does not start a read worker or alter the report length.
    /// </summary>
    [TestMethod]
    public void OutputOnlyDeviceDoesNotStartAReadWorkerAndPreservesReportLength()
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
            new byte[] { 1, 2, 3 },
            transport.LastWrite);
    }

    /// <summary>
    /// Verifies that offset-based protocol reports are not padded to a larger descriptor maximum.
    /// </summary>
    [TestMethod]
    public void WritePreservesProtocolReportLengthAndSelectedBufferRange()
    {
        // Arrange
        var transport = new TestTransport();

        var device = new HIDDevice("test", 0, 128);
        var buffer = new byte[70];

        buffer[ 0] = 0xFF;
        buffer[ 3] = 0xD0;
        buffer[ 4] = 0x40;
        buffer[66] = 0xAB;
        buffer[67] = 0xFF;

        using var stream = new HIDStream(device, transport);

        // Act
        stream.Write(buffer, 3, 64);

        // Assert
        Assert.HasCount(64, transport.LastWrite);

        Assert.AreEqual((byte)0xD0, transport.LastWrite[0]);
        Assert.AreEqual((byte)0x40, transport.LastWrite[1]);
        Assert.AreEqual((byte)0xAB, transport.LastWrite[63]);

        Assert.AreNotSame(buffer, transport.LastWrite);
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
