/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows;
using BlackSharp.Core.Interop.Windows.Enums;
using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NativeHID = BlackSharp.Core.Interop.Windows.Native.HID;

namespace BlackSharp.IO.HID.Backends;

/// <summary>
/// Provides the Windows HID transport implementation used by the hardware monitoring system.
/// </summary>
internal sealed class WindowsHIDTransport : IHIDTransport
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsHIDTransport"/> class.
    /// </summary>
    /// <param name="handle">The native handle used by the operation.</param>
    /// <param name="cancellationEvent">The cancellation event used by the operation.</param>
    private WindowsHIDTransport(SafeFileHandle handle, IntPtr cancellationEvent)
    {
        _handle = handle;
        _cancellationEvent = cancellationEvent;
    }

    #endregion

    #region Fields

    /// <summary>
    /// Stores the cancellation event state associated with this instance.
    /// </summary>
    private readonly IntPtr _cancellationEvent;

    /// <summary>
    /// Stores the handle state associated with this instance.
    /// </summary>
    private readonly SafeFileHandle _handle;

    /// <summary>
    /// Stores the cancelled state associated with this instance.
    /// </summary>
    private int _cancelled;

    /// <summary>
    /// Stores the disposed state associated with this instance.
    /// </summary>
    private int _disposed;

    #endregion

    #region Public

    /// <summary>
    /// Processes open for the Windows HID transport component.
    /// </summary>
    /// <param name="device">The device involved in the operation.</param>
    /// <returns>The value produced by open.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the operation cannot be completed.</exception>
    public static WindowsHIDTransport Open(HIDDevice device)
    {
        // Request overlapped read/write access first. Some HID interfaces permit read-only access,
        // which is retried below for devices protected against output reports.
        var nativeHandle = Kernel32.CreateFile(
            device.FileSystemPath,
            DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
            FileShareMode.Read | FileShareMode.Write,
            IntPtr.Zero,
            FileCreationDisposition.OpenExisting,
            FileFlagsAndAttributes.Overlapped,
            IntPtr.Zero);

        if (nativeHandle == IntPtr.Zero || nativeHandle == Kernel32.InvalidHandle)
        {
            var error = Marshal.GetLastWin32Error();

            if (error == Win32ErrorCodes.AccessDenied
             || error == Win32ErrorCodes.SharingViolation)
            {
                throw new UnauthorizedAccessException(
                    $"Access to HID interface '{device.FileSystemPath}' was denied.",
                    new Win32Exception(error));
            }

            throw CreateIOException($"Unable to open HID interface '{device.FileSystemPath}'.", error);
        }

        var handle = new SafeFileHandle(nativeHandle, true);
        var cancellationEvent = Kernel32.CreateEvent(IntPtr.Zero, true, false, null);

        if (cancellationEvent != IntPtr.Zero
         && cancellationEvent != Kernel32.InvalidHandle)
        {
            return new WindowsHIDTransport(handle, cancellationEvent);
        }

        var cancellationError = Marshal.GetLastWin32Error();

        handle.Dispose();

        throw CreateIOException("Unable to create the HID cancellation event.", cancellationError);
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        if (Interlocked.Exchange(ref _cancelled, 1) != 0)
        {
            return;
        }

        Kernel32.SetEvent(_cancellationEvent);
        Kernel32.CancelIoEx(_handle.DangerousGetHandle(), IntPtr.Zero);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Cancel();

        _handle.Dispose();
        Kernel32.CloseHandle(_cancellationEvent);
    }

    /// <inheritdoc/>
    public void GetFeature(byte[] buffer)
    {
        ThrowIfDisposedOrCancelled();

        if (!NativeHID.HidD_GetFeature(_handle, buffer, buffer.Length))
        {
            throw CreateIOException("Unable to retrieve the HID feature report.", Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc/>
    public int Read(byte[] buffer)
    {
        return PerformIO(buffer, false, Timeout.Infinite);
    }

    /// <inheritdoc/>
    public void SetFeature(byte[] buffer)
    {
        ThrowIfDisposedOrCancelled();

        if (!NativeHID.HidD_SetFeature(_handle, buffer, buffer.Length))
        {
            throw CreateIOException("Unable to send the HID feature report.", Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc/>
    public int Write(byte[] buffer, int timeoutMilliseconds)
    {
        return PerformIO(buffer, true, timeoutMilliseconds);
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
        return new IOException($"{message} Windows error {error}: {new Win32Exception(error).Message}");
    }

    /// <summary>
    /// Processes perform I/O for the Windows HID transport component.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    /// <param name="write">The write used by the operation.</param>
    /// <param name="timeoutMilliseconds">The timeout milliseconds used by the operation.</param>
    /// <returns>The value produced by perform I/O.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation cannot be completed.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation cannot be completed.</exception>
    private int PerformIO(byte[] buffer, bool write, int timeoutMilliseconds)
    {
        ThrowIfDisposedOrCancelled();

        var operationEvent = Kernel32.CreateEvent(IntPtr.Zero, true, false, null);

        if (operationEvent == IntPtr.Zero || operationEvent == Kernel32.InvalidHandle)
        {
            throw CreateIOException("Unable to create the HID operation event.", Marshal.GetLastWin32Error());
        }

        var overlapped = new NativeOverlappedData
        {
            EventHandle = operationEvent,
        };

        // Both the user buffer and OVERLAPPED structure must remain at fixed addresses until Windows signals completion.
        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        try
        {
            uint immediateBytes;

            var completed = write
                ? Kernel32.WriteFile(
                    _handle,
                    pinnedBuffer.AddrOfPinnedObject(),
                    (uint)buffer.Length,
                    out immediateBytes,
                    ref overlapped)
                : Kernel32.ReadFile(
                    _handle,
                    pinnedBuffer.AddrOfPinnedObject(),
                    (uint)buffer.Length,
                    out immediateBytes,
                    ref overlapped);

            if (completed)
            {
                return checked((int)immediateBytes);
            }

            var error = Marshal.GetLastWin32Error();

            if (error != Win32ErrorCodes.IoPending)
            {
                if (error == Win32ErrorCodes.OperationAborted
                 && Volatile.Read(ref _cancelled) != 0)
                {
                    throw new OperationCanceledException();
                }

                throw CreateIOException(
                    write
                        ? "Unable to write the HID output report."
                        : "Unable to read the HID input report.",
                    error);
            }

            var waitHandles = new[]
            {
                operationEvent,
                _cancellationEvent,
            };
            var waitResult = Kernel32.WaitForMultipleObjects(
                (uint)waitHandles.Length,
                waitHandles,
                false,
                timeoutMilliseconds == Timeout.Infinite
                    ? Kernel32.Infinite
                    : checked((uint)timeoutMilliseconds));

            if (waitResult == Kernel32.WaitObject0)
            {
                if (!Kernel32.GetOverlappedResult(_handle, ref overlapped, out var transferred, false))
                {
                    throw CreateIOException(
                        write
                            ? "Unable to finish writing the HID output report."
                            : "Unable to finish reading the HID input report.",
                        Marshal.GetLastWin32Error());
                }

                return checked((int)transferred);
            }

            Kernel32.CancelIoEx(_handle, ref overlapped);
            Kernel32.WaitForSingleObject(operationEvent, Kernel32.Infinite);

            if (waitResult == Kernel32.WaitObject0 + 1)
            {
                throw new OperationCanceledException();
            }

            if (waitResult == Kernel32.WaitTimeout)
            {
                throw new TimeoutException(write ? "The HID output report timed out." : "The HID input report timed out.");
            }

            throw CreateIOException("Unable to wait for the HID operation.", Marshal.GetLastWin32Error());
        }
        finally
        {
            pinnedBuffer.Free();
            Kernel32.CloseHandle(operationEvent);
        }
    }

    /// <summary>
    /// Processes throw if disposed or cancelled for the Windows HID transport component.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when the operation is attempted after the object has been disposed.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation cannot be completed.</exception>
    private void ThrowIfDisposedOrCancelled()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(WindowsHIDTransport));
        }

        if (Volatile.Read(ref _cancelled) != 0)
        {
            throw new OperationCanceledException();
        }
    }

    #endregion
}
