/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Enums;
using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.IO.Ports.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Windows;

/// <summary>
/// Provides timeout-aware overlapped I/O for Windows device and serial handles.
/// </summary>
public sealed class WindowsDeviceStream : IDisposable
{
    #region Constructor

    private WindowsDeviceStream(SafeFileHandle handle)
    {
        _Handle = handle;
    }

    #endregion

    #region Fields

    private readonly SafeFileHandle _Handle;

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the underlying Windows handle can no longer be used.
    /// </summary>
    public bool IsInvalid => _Handle == null || _Handle.IsInvalid || _Handle.IsClosed;

    #endregion

    #region Public

    /// <summary>
    /// Opens a Windows device path for overlapped read and write access.
    /// </summary>
    /// <param name="path">The Windows device path to open.</param>
    /// <returns>The opened stream, or <see langword="null"/> if the device could not be opened.</returns>
    public static WindowsDeviceStream Open(string path)
    {
        IntPtr handle = Kernel32.CreateFile(
            path,
            DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
            FileShareMode.Read | FileShareMode.Write,
            IntPtr.Zero,
            FileCreationDisposition.OpenExisting,
            FileFlagsAndAttributes.Overlapped,
            IntPtr.Zero);

        if (handle == IntPtr.Zero || handle == WindowsNativeMethods.InvalidHandleValue)
        {
            return null;
        }

        return new WindowsDeviceStream(new SafeFileHandle(handle, true));
    }

    /// <summary>
    /// Releases the underlying Windows device handle.
    /// </summary>
    public void Dispose()
    {
        _Handle.Dispose();
    }

    /// <summary>
    /// Reads data from the device within the specified timeout.
    /// </summary>
    /// <param name="buffer">The buffer that receives the data.</param>
    /// <param name="timeoutMilliseconds">The maximum duration of the operation in milliseconds.</param>
    /// <param name="bytesRead">Receives the number of bytes read.</param>
    /// <returns>
    /// <see langword="true"/> when the operation completed successfully; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Read(byte[] buffer, int timeoutMilliseconds, out int bytesRead)
    {
        return PerformIO(buffer, false, timeoutMilliseconds, out bytesRead);
    }

    /// <summary>
    /// Writes data to the device within the specified timeout.
    /// </summary>
    /// <param name="buffer">The data to write.</param>
    /// <param name="timeoutMilliseconds">The maximum duration of the operation in milliseconds.</param>
    /// <param name="bytesWritten">Receives the number of bytes written.</param>
    /// <returns>
    /// <see langword="true"/> when the operation completed successfully; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Write(byte[] buffer, int timeoutMilliseconds, out int bytesWritten)
    {
        return PerformIO(buffer, true, timeoutMilliseconds, out bytesWritten);
    }

    /// <summary>
    /// Sends a control code to the device within the specified timeout.
    /// </summary>
    /// <param name="controlCode">The device-specific control code.</param>
    /// <param name="inputBuffer">The optional input buffer.</param>
    /// <param name="outputBuffer">The optional output buffer.</param>
    /// <param name="timeoutMilliseconds">The maximum duration of the operation in milliseconds.</param>
    /// <param name="bytesReturned">Receives the number of bytes returned.</param>
    /// <returns>
    /// <see langword="true"/> when the operation completed successfully; otherwise, <see langword="false"/>.
    /// </returns>
    public bool DeviceControl(
        uint controlCode,
        byte[] inputBuffer,
        byte[] outputBuffer,
        int timeoutMilliseconds,
        out int bytesReturned)
    {
        bytesReturned = 0;
        if (IsInvalid || timeoutMilliseconds <= 0)
        {
            return false;
        }

        var eventHandle = WindowsNativeMethods.CreateEventW(IntPtr.Zero, true, false, null);
        if (eventHandle == IntPtr.Zero || eventHandle == WindowsNativeMethods.InvalidHandleValue)
        {
            return false;
        }

        var overlapped = new NativeOverlappedData
        {
            hEvent = eventHandle,
        };

        GCHandle inputHandle = default;
        GCHandle outputHandle = default;

        try
        {
            var inputPointer = IntPtr.Zero;
            uint inputLength = 0;

            if (inputBuffer != null && inputBuffer.Length > 0)
            {
                inputHandle = GCHandle.Alloc(inputBuffer, GCHandleType.Pinned);
                inputPointer = inputHandle.AddrOfPinnedObject();
                inputLength = checked((uint)inputBuffer.Length);
            }

            var outputPointer = IntPtr.Zero;
            uint outputLength = 0;

            if (outputBuffer != null && outputBuffer.Length > 0)
            {
                outputHandle = GCHandle.Alloc(outputBuffer, GCHandleType.Pinned);
                outputPointer = outputHandle.AddrOfPinnedObject();
                outputLength = checked((uint)outputBuffer.Length);
            }

            bool completed = WindowsNativeMethods.DeviceIoControl(
                _Handle,
                controlCode,
                inputPointer,
                inputLength,
                outputPointer,
                outputLength,
                out uint immediateBytes,
                ref overlapped);

            if (!CompleteOverlappedIO(completed, eventHandle, timeoutMilliseconds, ref overlapped, ref immediateBytes))
            {
                return false;
            }

            bytesReturned = checked((int)immediateBytes);

            return true;
        }
        finally
        {
            if (inputHandle.IsAllocated)
            {
                inputHandle.Free();
            }

            if (outputHandle.IsAllocated)
            {
                outputHandle.Free();
            }

            Kernel32.CloseHandle(eventHandle);
        }
    }

    /// <summary>
    /// Configures the device handle for 8-N-1 serial communication.
    /// </summary>
    /// <param name="baudRate">The serial baud rate.</param>
    /// <param name="timeoutMilliseconds">The read and write timeout in milliseconds.</param>
    /// <param name="enableRTS">Whether request-to-send is enabled.</param>
    /// <param name="readIntervalTimeout">The maximum delay between received bytes in milliseconds.</param>
    /// <param name="purgeAfterConfigure">
    /// Whether pending input and output should be purged after configuration.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the device was configured successfully; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ConfigureSerial(
        uint baudRate,
        int timeoutMilliseconds,
        bool enableRTS,
        uint readIntervalTimeout,
        bool purgeAfterConfigure = true)
    {
        var dcb = new Dcb
        {
            DCBlength = (uint)Marshal.SizeOf(typeof(Dcb)),
        };

        if (!WindowsNativeMethods.GetCommState(_Handle, ref dcb))
        {
            return false;
        }

        dcb.BaudRate = baudRate;
        dcb.ByteSize = 8;
        dcb.Parity   = 0;
        dcb.StopBits = 0;
        dcb.Flags    = dcb.Flags & 0xFFFFCFCCu | 0x00000001u;

        if (enableRTS)
        {
            dcb.Flags |= 0x00001000u;
        }

        if (!WindowsNativeMethods.SetCommState(_Handle, ref dcb))
        {
            return false;
        }

        var timeouts = new CommTimeouts
        {
            ReadIntervalTimeout         = readIntervalTimeout,
            ReadTotalTimeoutMultiplier  = 0,
            ReadTotalTimeoutConstant    = (uint)timeoutMilliseconds,
            WriteTotalTimeoutMultiplier = 0,
            WriteTotalTimeoutConstant   = (uint)timeoutMilliseconds,
        };

        if (!WindowsNativeMethods.SetCommTimeouts(_Handle, ref timeouts))
        {
            return false;
        }

        return !purgeAfterConfigure || Purge();
    }

    /// <summary>
    /// Cancels pending serial operations and clears the input and output buffers.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the buffers were purged successfully; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Purge()
    {
        return !IsInvalid && WindowsNativeMethods.PurgeComm(
            _Handle,
            WindowsNativeMethods.PurgeRxAbort
          | WindowsNativeMethods.PurgeRxClear
          | WindowsNativeMethods.PurgeTxAbort
          | WindowsNativeMethods.PurgeTxClear);
    }

    #endregion

    #region Private

    private bool PerformIO(byte[] buffer, bool write, int timeoutMilliseconds, out int bytesTransferred)
    {
        bytesTransferred = 0;
        if (IsInvalid || buffer == null || buffer.Length == 0 || timeoutMilliseconds <= 0)
        {
            return false;
        }

        var eventHandle = WindowsNativeMethods.CreateEventW(IntPtr.Zero, true, false, null);
        if (eventHandle == IntPtr.Zero || eventHandle == WindowsNativeMethods.InvalidHandleValue)
        {
            return false;
        }

        var overlapped = new NativeOverlappedData
        {
            hEvent = eventHandle,
        };

        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            uint immediateBytes;

            bool completed = write
                ? WindowsNativeMethods.WriteFile(
                    _Handle,
                    pinnedBuffer.AddrOfPinnedObject(),
                    (uint)buffer.Length,
                    out immediateBytes,
                    ref overlapped)
                : WindowsNativeMethods.ReadFile(
                    _Handle,
                    pinnedBuffer.AddrOfPinnedObject(),
                    (uint)buffer.Length,
                    out immediateBytes,
                    ref overlapped);

            if (!CompleteOverlappedIO(completed, eventHandle, timeoutMilliseconds, ref overlapped, ref immediateBytes))
            {
                return false;
            }

            bytesTransferred = checked((int)immediateBytes);

            return true;
        }
        finally
        {
            pinnedBuffer.Free();

            Kernel32.CloseHandle(eventHandle);
        }
    }

    private bool CompleteOverlappedIO(
        bool completed,
        IntPtr eventHandle,
        int timeoutMilliseconds,
        ref NativeOverlappedData overlapped,
        ref uint bytesTransferred)
    {
        if (completed)
        {
            return true;
        }

        int error = Marshal.GetLastWin32Error();
        if (error != WindowsNativeMethods.ErrorIoPending)
        {
            return false;
        }

        uint waitResult = WindowsNativeMethods.WaitForSingleObject(eventHandle, (uint)timeoutMilliseconds);
        if (waitResult != WindowsNativeMethods.WaitObject0)
        {
            WindowsNativeMethods.CancelIoEx(_Handle, ref overlapped);
            WindowsNativeMethods.WaitForSingleObject(eventHandle, 100);

            return false;
        }

        return WindowsNativeMethods.GetOverlappedResult(_Handle, ref overlapped, out bytesTransferred, false);
    }

    #endregion
}
