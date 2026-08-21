/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Linux.Native;
using BlackSharp.Core.Interop.Linux.Structures;
using BlackSharp.Core.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.HID.Backends;

/// <summary>
/// Provides the Linux HID transport implementation used by the hardware monitoring system.
/// </summary>
internal sealed class LinuxHIDTransport : IHIDTransport
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="LinuxHIDTransport"/> class.
    /// </summary>
    /// <param name="deviceDescriptor">The device descriptor involved in the operation.</param>
    /// <param name="cancellationDescriptor">The cancellation descriptor used by the operation.</param>
    /// <param name="reportsUseID">The reports use identifier.</param>
    private LinuxHIDTransport(int deviceDescriptor, int cancellationDescriptor, bool reportsUseID)
    {
        _deviceDescriptor = deviceDescriptor;
        _cancellationDescriptor = cancellationDescriptor;
        _reportsUseID = reportsUseID;
    }

    #endregion

    #region Fields

    /// <summary>
    /// Stores the cancellation descriptor state associated with this instance.
    /// </summary>
    private int _cancellationDescriptor;

    /// <summary>
    /// Stores the cancelled state associated with this instance.
    /// </summary>
    private int _cancelled;

    /// <summary>
    /// Stores the device descriptor state associated with this instance.
    /// </summary>
    private int _deviceDescriptor;

    /// <summary>
    /// Stores the disposed state associated with this instance.
    /// </summary>
    private int _disposed;

    /// <summary>
    /// Stores the reports use ID state associated with this instance.
    /// </summary>
    private readonly bool _reportsUseID;

    #endregion

    #region Public

    /// <summary>
    /// Processes open for the Linux HID transport component.
    /// </summary>
    /// <param name="device">The device involved in the operation.</param>
    /// <returns>The value produced by open.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the operation cannot be completed.</exception>
    public static LinuxHIDTransport Open(HIDDevice device)
    {
        // Open hidraw nonblocking with close-on-exec; later poll/read operations provide the caller's
        // timeout semantics without leaking descriptors to child processes.
        var deviceDescriptor = LibC.open(device.FileSystemPath, LibC.O_RDWR | LibC.O_NONBLOCK | LibC.O_CLOEXEC);

        if (deviceDescriptor < 0)
        {
            var error = Marshal.GetLastWin32Error();

            if (error == LibC.EACCES)
            {
                throw new UnauthorizedAccessException(
                    $"Access to HID interface '{device.FileSystemPath}' was denied.",
                    new Win32Exception(error));
            }

            throw CreateIOException($"Unable to open HID interface '{device.FileSystemPath}'.", error);
        }

        var cancellationDescriptor = LibC.eventfd(0, LibC.O_NONBLOCK | LibC.O_CLOEXEC);

        if (cancellationDescriptor >= 0)
        {
            return new LinuxHIDTransport(deviceDescriptor, cancellationDescriptor, device.ReportsUseID);
        }

        var cancellationError = Marshal.GetLastWin32Error();

        LibC.close(deviceDescriptor);

        throw CreateIOException("Unable to create the HID cancellation event.", cancellationError);
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        if (Interlocked.Exchange(ref _cancelled, 1) != 0)
        {
            return;
        }

        var descriptor = Volatile.Read(ref _cancellationDescriptor);

        if (descriptor < 0)
        {
            return;
        }

        ulong value = 1;

        LibC.write(descriptor, ref value, (UIntPtr)sizeof(ulong));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Cancel();

        var deviceDescriptor = Interlocked.Exchange(ref _deviceDescriptor, -1);

        if (deviceDescriptor >= 0)
        {
            LibC.close(deviceDescriptor);
        }

        var cancellationDescriptor = Interlocked.Exchange(ref _cancellationDescriptor, -1);

        if (cancellationDescriptor >= 0)
        {
            LibC.close(cancellationDescriptor);
        }
    }

    /// <inheritdoc/>
    public void GetFeature(byte[] buffer)
    {
        ThrowIfDisposedOrCancelled();

        if (buffer.Length < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }

        var nativeBuffer = new byte[buffer.Length - 1];

        nativeBuffer[0] = buffer[0];

        var request = HIDRaw.GetFeatureRequest(nativeBuffer.Length);

        var bytesRead = LibC.ioctl(_deviceDescriptor, request, nativeBuffer);

        if (bytesRead < 0)
        {
            throw CreateIOException("Unable to retrieve the HID feature report.", Marshal.GetLastWin32Error());
        }

        var copiedLength = Math.Min(bytesRead, nativeBuffer.Length);

        Buffer.BlockCopy(nativeBuffer, 0, buffer, 1, copiedLength);
        Array.Clear(buffer, 1 + copiedLength, buffer.Length - (1 + copiedLength));
    }

    /// <inheritdoc/>
    public int Read(byte[] buffer)
    {
        // Retry interrupted reads and translate native error codes only after distinguishing cancellation from a real transport failure.
        var nativeBuffer = _reportsUseID
            ? buffer
            : new byte[Math.Max(0, buffer.Length - 1)];

        while (true)
        {
            WaitForDevice(LibC.POLLIN, Timeout.Infinite);

            var result = LibC.read(_deviceDescriptor, nativeBuffer, (UIntPtr)nativeBuffer.Length).ToInt64();

            if (result >= 0)
            {
                var length = checked((int)result);

                if (_reportsUseID)
                {
                    return length;
                }

                buffer[0] = 0;

                if (length > 0)
                {
                    Buffer.BlockCopy(nativeBuffer, 0, buffer, 1, length);
                }

                return checked(length + 1);
            }

            var error = Marshal.GetLastWin32Error();

            if (error == LibC.EINTR || error == LibC.EAGAIN)
            {
                continue;
            }

            throw CreateIOException("Unable to read the HID input report.", error);
        }
    }

    /// <inheritdoc/>
    public void SetFeature(byte[] buffer)
    {
        ThrowIfDisposedOrCancelled();

        var request = HIDRaw.SetFeatureRequest(buffer.Length);

        if (LibC.ioctl(_deviceDescriptor, request, buffer) < 0)
        {
            throw CreateIOException("Unable to send the HID feature report.", Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc/>
    public int Write(byte[] buffer, int timeoutMilliseconds)
    {
        var started = Environment.TickCount;

        // Recompute the remaining budget on every retry so signals cannot extend the caller's timeout.
        while (true)
        {
            var remaining = TimeoutUtilities.GetRemainingMilliseconds(started, timeoutMilliseconds);

            if (remaining == 0)
            {
                throw new TimeoutException("The HID output report timed out.");
            }

            WaitForDevice(LibC.POLLOUT, remaining);

            var result = LibC.write(_deviceDescriptor, buffer, (UIntPtr)buffer.Length).ToInt64();

            if (result >= 0)
            {
                return checked((int)result);
            }

            var error = Marshal.GetLastWin32Error();

            // Signals and transient nonblocking back-pressure are retryable after polling again.
            if (error == LibC.EINTR || error == LibC.EAGAIN)
            {
                continue;
            }

            throw CreateIOException("Unable to write the HID output report.", error);
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// Creates I/O exception.
    /// </summary>
    /// <param name="message">The message used by the operation.</param>
    /// <param name="error">The error used by the operation.</param>
    /// <returns>A new I/O exception instance.</returns>
    private static IOException CreateIOException(string message, int error)
    {
        return new IOException($"{message} Linux error {error}: {new Win32Exception(error).Message}");
    }

    /// <summary>
    /// Processes throw if disposed or cancelled for the Linux HID transport component.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when the operation is attempted after the object has been disposed.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation cannot be completed.</exception>
    private void ThrowIfDisposedOrCancelled()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(LinuxHIDTransport));
        }

        if (Volatile.Read(ref _cancelled) != 0)
        {
            throw new OperationCanceledException();
        }
    }

    /// <summary>
    /// Waits for for device.
    /// </summary>
    /// <param name="requestedEvents">The requested events used by the operation.</param>
    /// <param name="timeoutMilliseconds">The timeout milliseconds used by the operation.</param>
    /// <exception cref="OperationCanceledException">Thrown when the operation cannot be completed.</exception>
    /// <exception cref="IOException">Thrown when the operation cannot be completed.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation cannot be completed.</exception>
    private void WaitForDevice(short requestedEvents, int timeoutMilliseconds)
    {
        ThrowIfDisposedOrCancelled();

        // Poll both the hidraw descriptor and the cancellation pipe so disposal interrupts a blocked read or write immediately.
        var descriptors = new[]
        {
            new PollFd
            {
                FileDescriptor = _deviceDescriptor,
                Events         = requestedEvents,
            },
            new PollFd
            {
                FileDescriptor = _cancellationDescriptor,
                Events         = LibC.POLLIN,
            },
        };

        while (true)
        {
            var result = LibC.poll(descriptors, (UIntPtr)descriptors.Length, timeoutMilliseconds);

            if (result > 0)
            {
                if ((descriptors[1].ReturnedEvents & LibC.POLLIN) != 0)
                {
                    throw new OperationCanceledException();
                }

                if ((descriptors[0].ReturnedEvents
                   & (LibC.POLLERR | LibC.POLLHUP | LibC.POLLNVAL)) != 0)
                {
                    throw new IOException("The HID interface was disconnected.");
                }

                if ((descriptors[0].ReturnedEvents & requestedEvents) != 0)
                {
                    return;
                }

                continue;
            }

            if (result == 0)
            {
                throw new TimeoutException("The HID operation timed out.");
            }

            var error = Marshal.GetLastWin32Error();

            if (error == LibC.EINTR)
            {
                continue;
            }

            throw CreateIOException("Unable to wait for the HID interface.", error);
        }
    }

    #endregion
}
