/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Threading;

namespace BlackSharp.IO.HID;

/// <summary>
/// Provides worker-based, timeout-aware raw HID report I/O.
/// </summary>
public sealed class HIDStream : Stream
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="HIDStream"/> class.
    /// </summary>
    /// <param name="device">The device involved in the operation.</param>
    /// <param name="transport">The transport used by the operation.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="device"/> is <see langword="null"/>.
    /// </exception>
    internal HIDStream(HIDDevice device, IHIDTransport transport)
    {
        Device = device ?? throw new ArgumentNullException(nameof(device));
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));

        if (device.MaximumInputReportLength > 0)
        {
            _readThread = new Thread(ReadWorker)
            {
                IsBackground = true,
                Name = "BlackSharp HID Reader",
            };
            _readThread.Start();
        }

        if (device.MaximumOutputReportLength > 0)
        {
            _writeThread = new Thread(WriteWorker)
            {
                IsBackground = true,
                Name = "BlackSharp HID Writer",
            };
            _writeThread.Start();
        }
    }

    #endregion

    #region Fields

    /// <summary>
    /// Defines the fixed maximum queued input reports value used by this component.
    /// </summary>
    private const int MaximumQueuedInputReports = 128;

    /// <summary>
    /// Synchronizes access to state shared by concurrent operations.
    /// </summary>
    private readonly object _featureSync = new();

    /// <summary>
    /// Stores the input queue state associated with this instance.
    /// </summary>
    private readonly Queue<byte[]> _inputQueue = new();

    /// <summary>
    /// Synchronizes access to state shared by concurrent operations.
    /// </summary>
    private readonly object _inputSync = new();

    /// <summary>
    /// Stores the read thread state associated with this instance.
    /// </summary>
    private readonly Thread _readThread;

    /// <summary>
    /// Stores the transport state associated with this instance.
    /// </summary>
    private readonly IHIDTransport _transport;

    /// <summary>
    /// Stores the write queue state associated with this instance.
    /// </summary>
    private readonly Queue<WriteOperation> _writeQueue = new();

    /// <summary>
    /// Synchronizes access to state shared by concurrent operations.
    /// </summary>
    private readonly object _writeSync = new();

    /// <summary>
    /// Stores the write thread state associated with this instance.
    /// </summary>
    private readonly Thread _writeThread;

    /// <summary>
    /// Stores the dispose started state associated with this instance.
    /// </summary>
    private int _disposeStarted;

    /// <summary>
    /// Stores the input terminal exception state associated with this instance.
    /// </summary>
    private Exception _inputTerminalException;

    /// <summary>
    /// Stores the shutdown state associated with this instance.
    /// </summary>
    private bool _shutdown;

    /// <summary>
    /// Stores the write terminal exception state associated with this instance.
    /// </summary>
    private Exception _writeTerminalException;
    private int _readTimeout = 3000;
    private int _writeTimeout = 3000;

    #endregion

    #region Properties

    /// <inheritdoc />
    public override bool CanRead => !Volatile.Read(ref _shutdown)
                                 && Device.MaximumInputReportLength > 0;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanTimeout => true;

    /// <inheritdoc />
    public override bool CanWrite => !Volatile.Read(ref _shutdown)
                                  && Device.MaximumOutputReportLength > 0;

    /// <summary>
    /// Gets the HID device represented by this stream.
    /// </summary>
    public HIDDevice Device { get; }

    /// <inheritdoc />
    public override long Length => throw new NotSupportedException();

    /// <inheritdoc />
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override int ReadTimeout
    {
        get => Volatile.Read(ref _readTimeout);
        set
        {
            ValidateTimeout(value, nameof(value));
            Volatile.Write(ref _readTimeout, value);
        }
    }

    /// <inheritdoc />
    public override int WriteTimeout
    {
        get => Volatile.Read(ref _writeTimeout);
        set
        {
            ValidateTimeout(value, nameof(value));
            Volatile.Write(ref _writeTimeout, value);
        }
    }

    #endregion

    #region Public

    /// <summary>
    /// Retrieves a HID feature report.
    /// </summary>
    /// <param name="buffer">
    /// The report buffer. The first byte must contain the report ID and receives the returned report ID.
    /// </param>
    public void GetFeature(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        GetFeature(buffer, 0, buffer.Length);
    }

    /// <summary>
    /// Retrieves a HID feature report into a region of the supplied buffer.
    /// </summary>
    public void GetFeature(byte[] buffer, int offset, int count)
    {
        ValidateFeatureBuffer(buffer, offset, count);

        lock (_featureSync)
        {
            ThrowIfDisposed();

            if (offset == 0 && count == buffer.Length)
            {
                _transport.GetFeature(buffer);

                return;
            }

            var report = new byte[count];

            Buffer.BlockCopy(buffer, offset, report, 0, count);
            _transport.GetFeature(report);
            Buffer.BlockCopy(report, 0, buffer, offset, count);
        }
    }

    /// <inheritdoc />
    public override void Flush()
    {
        ThrowIfDisposed();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        // Read into a full input-report buffer and expose only the caller-requested payload bytes.
        ValidateBuffer(buffer, offset, count);

        if (count == 0)
        {
            return 0;
        }

        if (!CanRead)
        {
            ThrowIfDisposed();

            throw new NotSupportedException("The HID device does not expose input reports.");
        }

        var timeout = ReadTimeout;
        var started = Environment.TickCount;

        lock (_inputSync)
        {
            while (_inputQueue.Count == 0)
            {
                if (_inputTerminalException != null)
                {
                    ThrowStoredException(_inputTerminalException);
                }

                ThrowIfDisposed();

                var remaining = TimeoutUtilities.GetRemainingMilliseconds(started, timeout);

                if (remaining == 0 || !Monitor.Wait(_inputSync, remaining))
                {
                    throw new TimeoutException("The HID input report timed out.");
                }
            }

            var report = _inputQueue.Dequeue();
            var length = Math.Min(count, report.Length);

            Buffer.BlockCopy(report, 0, buffer, offset, length);

            return length;
        }
    }

    /// <summary>
    /// Reads one HID input report into the supplied buffer.
    /// </summary>
    /// <param name="buffer">The buffer that receives the report.</param>
    /// <returns>The number of bytes copied to the buffer.</returns>
    public int Read(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        return Read(buffer, 0, buffer.Length);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Sends a HID feature report.
    /// </summary>
    /// <param name="buffer">The report buffer whose first byte contains the report ID.</param>
    public void SetFeature(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        SetFeature(buffer, 0, buffer.Length);
    }

    /// <summary>
    /// Sends a HID feature report from a region of the supplied buffer.
    /// </summary>
    public void SetFeature(byte[] buffer, int offset, int count)
    {
        ValidateFeatureBuffer(buffer, offset, count);

        lock (_featureSync)
        {
            ThrowIfDisposed();

            if (offset == 0 && count == buffer.Length)
            {
                _transport.SetFeature(buffer);

                return;
            }

            var report = new byte[count];

            Buffer.BlockCopy(buffer, offset, report, 0, count);
            _transport.SetFeature(report);
        }
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        // Frame the caller data as a complete HID output report and keep partial native writes from appearing successful.
        ValidateBuffer(buffer, offset, count);

        if (count == 0)
        {
            return;
        }

        ThrowIfDisposed();

        if (!CanWrite)
        {
            throw new NotSupportedException("The HID device does not expose output reports.");
        }

        if (count > Device.MaximumOutputReportLength)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The output report is larger than the device maximum.");
        }

        var data = new byte[Device.MaximumOutputReportLength];

        Buffer.BlockCopy(buffer, offset, data, 0, count);

        var operation = new WriteOperation(data, WriteTimeout);

        lock (_writeSync)
        {
            if (_writeTerminalException != null)
            {
                ThrowStoredException(_writeTerminalException);
            }

            ThrowIfDisposed();

            _writeQueue.Enqueue(operation);
            Monitor.PulseAll(_writeSync);
        }

        operation.Wait();
    }

    /// <summary>
    /// Writes one complete HID output report.
    /// </summary>
    /// <param name="buffer">The report to write.</param>
    public void Write(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        Write(buffer, 0, buffer.Length);
    }

    #endregion

    #region Protected

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // Signal shutdown before releasing handles so blocked readers and writers observe a terminal state instead of racing disposed resources.
        if (Interlocked.Exchange(ref _disposeStarted, 1) != 0)
        {
            return;
        }

        Volatile.Write(ref _shutdown, true);

        _transport.Cancel();

        lock (_inputSync)
        {
            Monitor.PulseAll(_inputSync);
        }

        lock (_writeSync)
        {
            while (_writeQueue.Count > 0)
            {
                _writeQueue.Dequeue().Complete(new ObjectDisposedException(nameof(HIDStream)));
            }

            Monitor.PulseAll(_writeSync);
        }

        if (_readThread != null && Thread.CurrentThread != _readThread)
        {
            _readThread.Join();
        }

        if (_writeThread != null && Thread.CurrentThread != _writeThread)
        {
            _writeThread.Join();
        }

        _transport.Dispose();

        base.Dispose(disposing);
    }

    #endregion

    #region Private

    /// <summary>
    /// Reads worker from the underlying device or platform.
    /// </summary>
    /// <exception cref="IOException">Thrown when the operation cannot be completed.</exception>
    private void ReadWorker()
    {
        try
        {
            // A single transport reader owns the blocking endpoint and dispatches complete reports
            // to consumers, avoiding concurrent native reads on one HID handle.
            while (!Volatile.Read(ref _shutdown))
            {
                var buffer = new byte[Device.MaximumInputReportLength];
                var length = _transport.Read(buffer);

                if (length <= 0)
                {
                    throw new IOException("The HID interface was disconnected.");
                }

                if (length != buffer.Length)
                {
                    // Subscribers must only see bytes supplied by the transport, not buffer padding.
                    Array.Resize(ref buffer, length);
                }

                lock (_inputSync)
                {
                    if (_inputQueue.Count == MaximumQueuedInputReports)
                    {
                        _inputQueue.Dequeue();
                    }

                    _inputQueue.Enqueue(buffer);
                    Monitor.PulseAll(_inputSync);
                }
            }
        }
        catch (OperationCanceledException) when (Volatile.Read(ref _shutdown))
        {
        }
        catch (ObjectDisposedException) when (Volatile.Read(ref _shutdown))
        {
        }
        catch (Exception exception)
        {
            lock (_inputSync)
            {
                _inputTerminalException = NormalizeIOException(exception, "HID input failed.");
                Monitor.PulseAll(_inputSync);
            }
        }
    }

    /// <summary>
    /// Processes throw if disposed for the HID stream component.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when the operation is attempted after the object has been disposed.
    /// </exception>
    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _shutdown))
        {
            throw new ObjectDisposedException(nameof(HIDStream));
        }
    }

    /// <summary>
    /// Processes throw stored exception for the HID stream component.
    /// </summary>
    /// <param name="exception">The exception that describes the reported failure.</param>
    private static void ThrowStoredException(Exception exception)
    {
        throw exception;
    }

    /// <summary>
    /// Validates buffer.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    /// <param name="offset">The zero-based byte offset at which processing begins.</param>
    /// <param name="count">The number of entries to process.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the operation cannot be completed.</exception>
    private static void ValidateBuffer(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0 || count < 0 || offset > buffer.Length - count)
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Validates feature buffer.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    /// <param name="offset">The zero-based byte offset at which processing begins.</param>
    /// <param name="count">The number of entries to process.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested operation is not supported by the current platform or device.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is outside the supported range.
    /// </exception>
    private void ValidateFeatureBuffer(byte[] buffer, int offset, int count)
    {
        ValidateBuffer(buffer, offset, count);

        if (Device.MaximumFeatureReportLength == 0)
        {
            throw new NotSupportedException("The HID device does not expose feature reports.");
        }

        if (count == 0
         || count > Device.MaximumFeatureReportLength)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
    }

    /// <summary>
    /// Validates timeout.
    /// </summary>
    /// <param name="timeout">The timeout used by the operation.</param>
    /// <param name="parameterName">The parameter name used by the operation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the operation cannot be completed.</exception>
    private static void ValidateTimeout(int timeout, string parameterName)
    {
        if (timeout <= 0 && timeout != Timeout.Infinite)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    /// <summary>
    /// Writes worker to the underlying device or platform.
    /// </summary>
    private void WriteWorker()
    {
        WriteOperation operation = null;
        try
        {
            while (true)
            {
                lock (_writeSync)
                {
                    // The queue lock protects both dequeueing and the shutdown transition; waiting
                    // releases it so producers can enqueue and pulse the worker.
                    while (_writeQueue.Count == 0 && !Volatile.Read(ref _shutdown))
                    {
                        Monitor.Wait(_writeSync);
                    }

                    if (Volatile.Read(ref _shutdown))
                    {
                        return;
                    }

                    operation = _writeQueue.Dequeue();
                }

                var remaining = operation.GetRemainingTimeout();

                // Time spent waiting in the queue consumes the caller's original timeout budget.
                if (remaining == 0)
                {
                    operation.Complete(new TimeoutException("The HID output report timed out."));
                    operation = null;

                    continue;
                }
                try
                {
                    var length = _transport.Write(operation.Buffer, remaining);

                    operation.Complete(length == operation.Buffer.Length
                        ? null
                        : new IOException("The complete HID output report could not be written."));
                    operation = null;
                }
                catch (TimeoutException exception)
                {
                    operation.Complete(exception);
                    operation = null;
                }
            }
        }
        catch (OperationCanceledException) when (Volatile.Read(ref _shutdown))
        {
            operation?.Complete(new ObjectDisposedException(nameof(HIDStream)));
        }
        catch (ObjectDisposedException) when (Volatile.Read(ref _shutdown))
        {
            operation?.Complete(new ObjectDisposedException(nameof(HIDStream)));
        }
        catch (Exception exception)
        {
            // An unexpected transport failure is terminal for this worker. Complete every queued
            // operation with the same normalized error so no caller remains blocked.
            var terminalException = NormalizeIOException(exception, "HID output failed.");

            operation?.Complete(terminalException);

            lock (_writeSync)
            {
                _writeTerminalException = terminalException;

                while (_writeQueue.Count > 0)
                {
                    _writeQueue.Dequeue().Complete(terminalException);
                }
            }
        }
    }

    /// <summary>
    /// Normalizes I/O exception.
    /// </summary>
    /// <param name="exception">The exception that describes the reported failure.</param>
    /// <param name="message">The message used by the operation.</param>
    /// <returns>The normalized I/O exception value.</returns>
    private static Exception NormalizeIOException(Exception exception, string message)
    {
        if (exception is IOException
         || exception is TimeoutException
         || exception is UnauthorizedAccessException
         || exception is NotSupportedException)
        {
            return exception;
        }

        return new IOException(message, exception);
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Provides the write operation implementation used by the hardware monitoring system.
    /// </summary>
    private sealed class WriteOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOperation"/> class.
        /// </summary>
        /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
        /// <param name="timeout">The timeout used by the operation.</param>
        public WriteOperation(byte[] buffer, int timeout)
        {
            Buffer = buffer;
            Timeout = timeout;
            Started = Environment.TickCount;
        }

        /// <summary>
        /// Gets the buffer associated with this instance.
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Gets the started.
        /// </summary>
        public int Started { get; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        public int Timeout { get; }

        /// <summary>
        /// Synchronizes access to state shared by concurrent operations.
        /// </summary>
        private readonly object _sync = new();

        /// <summary>
        /// Stores the completed state associated with this instance.
        /// </summary>
        private bool _completed;

        /// <summary>
        /// Stores the exception state associated with this instance.
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// Processes complete for the write operation component.
        /// </summary>
        /// <param name="exception">The exception that describes the reported failure.</param>
        public void Complete(Exception exception)
        {
            lock (_sync)
            {
                if (_completed)
                {
                    return;
                }

                _exception = exception;
                _completed = true;

                Monitor.PulseAll(_sync);
            }
        }

        /// <summary>
        /// Retrieves remaining timeout.
        /// </summary>
        /// <returns>The requested remaining timeout.</returns>
        public int GetRemainingTimeout()
        {
            return TimeoutUtilities.GetRemainingMilliseconds(Started, Timeout);
        }

        /// <summary>
        /// Processes wait for the write operation component.
        /// </summary>
        public void Wait()
        {
            lock (_sync)
            {
                while (!_completed)
                {
                    Monitor.Wait(_sync);
                }
            }

            if (_exception != null)
            {
                HIDStream.ThrowStoredException(_exception);
            }
        }
    }

    #endregion
}
