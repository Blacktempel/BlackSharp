/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using BlackSharp.Core.Interop.Windows.Enums;
using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.IO.Ports.Interop.Windows;
using BlackSharp.IO.Ports.Interop.Windows.Structures;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Backends;

internal sealed class WindowsSerialPortBackend : ISerialPortBackend
{
    #region Fields

    private const int CancelCompletionWaitMilliseconds = 250;

    private readonly object _stateLock = new object();
    private IntPtr _handle = WindowsNativeMethods.InvalidHandleValue;
    private SerialPortSettings _settings;
    private bool _closing;

    #endregion

    #region Properties

    public bool IsOpen
    {
        get
        {
            lock (_stateLock)
                return _handle != WindowsNativeMethods.InvalidHandleValue && !_closing;
        }
    }

    public int BytesToRead
    {
        get
        {
            var context = GetOpenOperationContext();

            if (!WindowsNativeMethods.ClearCommError(context.Handle, out _, out var stat))
            {
                throw CreateIOException($"{nameof(WindowsNativeMethods.ClearCommError)} failed.");
            }

            return stat.cbInQue > int.MaxValue ? int.MaxValue : (int)stat.cbInQue;
        }
    }

    #endregion

    #region Public

    public void Open(SerialPortSettings settings)
    {
        if (IsOpen)
        {
            throw new InvalidOperationException("The serial port backend is already open.");
        }

        string deviceName = NormalizePortName(settings.PortName);

        var handle = Kernel32.CreateFile(
            deviceName,
            DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
            FileShareMode.None,
            IntPtr.Zero,
            FileCreationDisposition.OpenExisting,
            FileFlagsAndAttributes.Normal | FileFlagsAndAttributes.Overlapped,
            IntPtr.Zero);

        if (handle == WindowsNativeMethods.InvalidHandleValue)
        {
            throw CreateIOException($"Could not open serial port '{settings.PortName}'.");
        }

        lock (_stateLock)
        {
            _handle = handle;
            _settings = settings.Clone();
            _closing = false;
        }

        try
        {
            WindowsNativeMethods.SetupComm(handle, 4096, 4096);

            Configure(handle, settings);
            ApplyTimeouts(handle, settings);

            SetDtr(settings.DtrEnable);
            SetRts(settings.RtsEnable);
        }
        catch
        {
            SerialPortCloseWorker.Close(this, TimeSpan.Zero, out _);
            throw;
        }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        var context = GetOpenOperationContext();

        if (count == 0)
        {
            return 0;
        }

        using var operation = new OverlappedIoOperation(count);

        bool completedSynchronously = WindowsNativeMethods.ReadFileOverlapped(
            context.Handle,
            operation.Buffer,
            count,
            IntPtr.Zero,
            operation.Overlapped);

        int error = Marshal.GetLastWin32Error();
        uint bytesTransferred = CompleteOverlappedOperation(
            context.Handle,
            operation,
            completedSynchronously,
            error,
            context.Settings.ReadTimeout,
            "read");

        if (bytesTransferred == 0)
        {
            throw new TimeoutException("The serial port read operation timed out.");
        }

        int bytesRead = checked((int)bytesTransferred);
        Marshal.Copy(operation.Buffer, buffer, offset, bytesRead);
        return bytesRead;
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        var context = GetOpenOperationContext();

        if (count == 0)
        {
            return;
        }

        using var operation = new OverlappedIoOperation(count);
        Marshal.Copy(buffer, offset, operation.Buffer, count);

        bool completedSynchronously = WindowsNativeMethods.WriteFileOverlapped(
            context.Handle,
            operation.Buffer,
            count,
            IntPtr.Zero,
            operation.Overlapped);

        int error = Marshal.GetLastWin32Error();
        uint bytesTransferred = CompleteOverlappedOperation(
            context.Handle,
            operation,
            completedSynchronously,
            error,
            context.Settings.WriteTimeout,
            "write");

        if (bytesTransferred != count)
        {
            throw new TimeoutException($"The serial port write operation timed out after writing {bytesTransferred} of {count} bytes.");
        }
    }

    public void DiscardInBuffer()
    {
        var context = GetOpenOperationContext();

        if (!WindowsNativeMethods.PurgeComm(context.Handle, WindowsNativeMethods.PurgeRxAbort | WindowsNativeMethods.PurgeRxClear))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.PurgeComm)}(RX) failed.");
        }
    }

    public void DiscardOutBuffer()
    {
        var context = GetOpenOperationContext();

        if (!WindowsNativeMethods.PurgeComm(context.Handle, WindowsNativeMethods.PurgeTxAbort | WindowsNativeMethods.PurgeTxClear))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.PurgeComm)}(TX) failed.");
        }
    }

    public void SetDtr(bool enabled)
    {
        var context = TryGetOperationContext();
        if (!context.HasValue)
        {
            return;
        }

        if (!WindowsNativeMethods.EscapeCommFunction(context.Value.Handle, enabled ? WindowsNativeMethods.Setdtr : WindowsNativeMethods.Clrdtr))
        {
            throw CreateIOException(enabled
                ? $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Setdtr)}) failed."
                : $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Clrdtr)}) failed.");
        }
    }

    public void SetRts(bool enabled)
    {
        var context = TryGetOperationContext();
        if (!context.HasValue)
        {
            return;
        }

        if (context.Value.Settings.Handshake == Handshake.RequestToSend || context.Value.Settings.Handshake == Handshake.RequestToSendXOnXOff)
        {
            return;
        }

        if (!WindowsNativeMethods.EscapeCommFunction(context.Value.Handle, enabled ? WindowsNativeMethods.Setrts : WindowsNativeMethods.Clrrts))
        {
            throw CreateIOException(enabled
                ? $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Setrts)}) failed."
                : $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Clrrts)}) failed.");
        }
    }

    public void RequestAbort()
    {
        IntPtr handle;

        lock (_stateLock)
        {
            if (_handle == WindowsNativeMethods.InvalidHandleValue)
            {
                return;
            }

            _closing = true;
            handle = _handle;
        }

        try
        {
            WindowsNativeMethods.CancelIoEx(handle, IntPtr.Zero);
        }
        catch (EntryPointNotFoundException)
        {
            // CancelIoEx is Vista+. No fallback is used here because PurgeComm can itself hang on broken USB/VCP drivers.
        }
    }

    public void CloseCore()
    {
        IntPtr handle;

        lock (_stateLock)
        {
            handle = _handle;
            _handle = WindowsNativeMethods.InvalidHandleValue;
            _closing = true;
        }

        if (handle == WindowsNativeMethods.InvalidHandleValue)
        {
            return;
        }

        if (!Kernel32.CloseHandle(handle))
        {
            throw CreateIOException($"{nameof(Kernel32.CloseHandle)} failed.");
        }
    }

    public static List<string> GetPortNames()
    {
        var buffer = new char[64 * 1024];
        uint length = WindowsNativeMethods.QueryDosDeviceW(null, buffer, (uint)buffer.Length);

        if (length == 0)
        {
            return [];
        }

        var result = new List<string>();
        int start = 0;

        for (int i = 0; i < length; i++)
        {
            if (buffer[i] != '\0')
            {
                continue;
            }

            if (i > start)
            {
                string name = new string(buffer, start, i - start);
                if (IsComName(name))
                {
                    result.Add(name);
                }
            }

            start = i + 1;
        }

        result.Sort(ComparePortNames);

        return result;
    }

    #endregion

    #region Private

    private uint CompleteOverlappedOperation(
        IntPtr handle,
        OverlappedIoOperation operation,
        bool completedSynchronously,
        int initialError,
        int timeout,
        string operationName)
    {
        if (completedSynchronously)
        {
            if (TryGetOverlappedResult(handle, operation.Overlapped, operationName, out uint completedBytes))
            {
                return completedBytes;
            }
        }
        else if (initialError != WindowsNativeMethods.ErrorIoPending)
        {
            throw CreateIOException($"{operationName} operation failed.", initialError);
        }

        uint waitResult = WindowsNativeMethods.WaitForSingleObject(operation.EventHandle, ToWaitMilliseconds(timeout));

        if (waitResult == WindowsNativeMethods.WaitObject0)
        {
            return GetCompletedOverlappedResult(handle, operation.Overlapped, operationName);
        }

        if (waitResult == WindowsNativeMethods.WaitTimeout)
        {
            CancelOverlappedOperation(handle, operation);
            throw new TimeoutException($"The serial port {operationName} operation timed out.");
        }

        if (waitResult == WindowsNativeMethods.WaitFailed)
        {
            throw CreateIOException($"Waiting for the serial port {operationName} operation failed.");
        }

        throw new IOException($"Waiting for the serial port {operationName} operation returned unexpected result 0x{waitResult:X8}.");
    }

    private static bool TryGetOverlappedResult(IntPtr handle, IntPtr overlapped, string operationName, out uint bytesTransferred)
    {
        if (WindowsNativeMethods.GetOverlappedResult(handle, overlapped, out bytesTransferred, false))
        {
            return true;
        }

        int error = Marshal.GetLastWin32Error();
        if (error == WindowsNativeMethods.ErrorIoIncomplete)
        {
            return false;
        }

        if (error == WindowsNativeMethods.ErrorOperationAborted)
        {
            throw new IOException($"The serial port {operationName} operation was aborted.");
        }

        throw CreateIOException($"Completing the serial port {operationName} operation failed.", error);
    }

    private static uint GetCompletedOverlappedResult(IntPtr handle, IntPtr overlapped, string operationName)
    {
        if (WindowsNativeMethods.GetOverlappedResult(handle, overlapped, out uint bytesTransferred, false))
        {
            return bytesTransferred;
        }

        int error = Marshal.GetLastWin32Error();
        if (error == WindowsNativeMethods.ErrorOperationAborted)
        {
            throw new IOException($"The serial port {operationName} operation was aborted.");
        }

        throw CreateIOException($"Completing the serial port {operationName} operation failed.", error);
    }

    private static void CancelOverlappedOperation(IntPtr handle, OverlappedIoOperation operation)
    {
        try
        {
            if (!WindowsNativeMethods.CancelIoEx(handle, operation.Overlapped))
            {
                int cancelError = Marshal.GetLastWin32Error();
                if (cancelError != WindowsNativeMethods.ErrorNotFound && cancelError != WindowsNativeMethods.ErrorOperationAborted)
                {
                    operation.AbandonResources();
                    return;
                }
            }
        }
        catch (EntryPointNotFoundException)
        {
            // Vista+ API. If it does not exist, the bounded wait below still prevents the caller from blocking forever.
        }

        uint waitResult = WindowsNativeMethods.WaitForSingleObject(operation.EventHandle, CancelCompletionWaitMilliseconds);
        if (waitResult != WindowsNativeMethods.WaitObject0)
        {
            operation.AbandonResources();
        }
    }

    private static uint ToWaitMilliseconds(int timeout)
    {
        if (timeout == SerialPort.InfiniteTimeout)
        {
            return WindowsNativeMethods.Infinite;
        }

        return checked((uint)timeout);
    }

    private void Configure(IntPtr handle, SerialPortSettings settings)
    {
        var dcb = new Dcb
        {
            DCBlength = (uint)Marshal.SizeOf(typeof(Dcb))
        };

        if (!WindowsNativeMethods.GetCommState(handle, ref dcb))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.GetCommState)} failed.");
        }

        dcb.DCBlength = (uint)Marshal.SizeOf(typeof(Dcb));
        dcb.BaudRate = checked((uint)settings.BaudRate);
        dcb.ByteSize = checked((byte)settings.DataBits);
        dcb.Parity = ToWindowsParity(settings.Parity);
        dcb.StopBits = ToWindowsStopBits(settings.StopBits);

        uint flags = dcb.Flags;
        flags = SetFlag(flags, 0, true); // fBinary
        flags = SetFlag(flags, 1, settings.Parity != Parity.None); // fParity
        flags = SetFlag(flags, 2, settings.Handshake == Handshake.RequestToSend || settings.Handshake == Handshake.RequestToSendXOnXOff); // fOutxCtsFlow
        flags = SetFlag(flags, 3, false); // fOutxDsrFlow
        flags = SetBits(flags, 4, 2, settings.DtrEnable ? 1u : 0u); // fDtrControl
        flags = SetFlag(flags, 6, false); // fDsrSensitivity
        flags = SetFlag(flags, 7, false); // fTXContinueOnXoff
        flags = SetFlag(flags, 8, settings.Handshake == Handshake.XOnXOff || settings.Handshake == Handshake.RequestToSendXOnXOff); // fOutX
        flags = SetFlag(flags, 9, settings.Handshake == Handshake.XOnXOff || settings.Handshake == Handshake.RequestToSendXOnXOff); // fInX
        flags = SetFlag(flags, 10, false); // fErrorChar
        flags = SetFlag(flags, 11, false); // fNull
        flags = SetBits(flags, 12, 2, settings.Handshake == Handshake.RequestToSend || settings.Handshake == Handshake.RequestToSendXOnXOff ? 2u : (settings.RtsEnable ? 1u : 0u)); // fRtsControl
        flags = SetFlag(flags, 14, false); // fAbortOnError
        dcb.Flags = flags;

        if (!WindowsNativeMethods.SetCommState(handle, ref dcb))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.SetCommState)} failed.");
        }
    }

    private void ApplyTimeouts(IntPtr handle, SerialPortSettings settings)
    {
        var timeouts = new CommTimeouts();

        if (settings.ReadTimeout == SerialPort.InfiniteTimeout)
        {
            timeouts.ReadIntervalTimeout = 0;
            timeouts.ReadTotalTimeoutMultiplier = 0;
            timeouts.ReadTotalTimeoutConstant = 0;
        }
        else
        {
            // Windows special mode: return immediately with buffered bytes, otherwise wait up to constant for one byte.
            timeouts.ReadIntervalTimeout = uint.MaxValue;
            timeouts.ReadTotalTimeoutMultiplier = uint.MaxValue;
            timeouts.ReadTotalTimeoutConstant = checked((uint)settings.ReadTimeout);
        }

        timeouts.WriteTotalTimeoutMultiplier = 0;
        timeouts.WriteTotalTimeoutConstant = settings.WriteTimeout == SerialPort.InfiniteTimeout ? 0 : checked((uint)settings.WriteTimeout);

        if (!WindowsNativeMethods.SetCommTimeouts(handle, ref timeouts))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.SetCommTimeouts)} failed.");
        }
    }

    private OperationContext GetOpenOperationContext()
    {
        var context = TryGetOperationContext();
        if (!context.HasValue)
        {
            throw new InvalidOperationException("The serial port backend is not open.");
        }

        return context.Value;
    }

    private OperationContext? TryGetOperationContext()
    {
        lock (_stateLock)
        {
            if (_handle == WindowsNativeMethods.InvalidHandleValue || _closing || _settings == null)
            {
                return null;
            }

            return new OperationContext(_handle, _settings.Clone());
        }
    }

    private static string NormalizePortName(string portName)
    {
        if (portName.StartsWith(@"\\.\", StringComparison.Ordinal))
        {
            return portName;
        }

        if (IsComName(portName))
        {
            return @"\\.\" + portName;
        }

        return portName;
    }

    private static bool IsComName(string name)
    {
        if (!name.StartsWith("COM", StringComparison.OrdinalIgnoreCase) || name.Length <= 3)
        {
            return false;
        }

        for (int i = 3; i < name.Length; i++)
        {
            if (!char.IsDigit(name[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static int ComparePortNames(string x, string y)
    {
        int nx = ParseComNumber(x);
        int ny = ParseComNumber(y);

        return nx.CompareTo(ny);
    }

    private static int ParseComNumber(string name)
    {
        return int.TryParse(name.Substring(3), out int value) ? value : int.MaxValue;
    }

    private static byte ToWindowsParity(Parity parity)
    {
        return parity switch
        {
            Parity.None  => WindowsNativeMethods.Noparity   ,
            Parity.Odd   => WindowsNativeMethods.Oddparity  ,
            Parity.Even  => WindowsNativeMethods.Evenparity ,
            Parity.Mark  => WindowsNativeMethods.Markparity ,
            Parity.Space => WindowsNativeMethods.Spaceparity,
            _ => throw new ArgumentOutOfRangeException(nameof(parity))
        };
    }

    private static byte ToWindowsStopBits(StopBits stopBits)
    {
        return stopBits switch
        {
            StopBits.One          => WindowsNativeMethods.Onestopbit  ,
            StopBits.OnePointFive => WindowsNativeMethods.One5stopbits,
            StopBits.Two          => WindowsNativeMethods.Twostopbits ,
            _ => throw new ArgumentOutOfRangeException(nameof(stopBits))
        };
    }

    private static uint SetFlag(uint flags, int bit, bool value)
    {
        uint mask = 1u << bit;
        return value ? flags | mask : flags & ~mask;
    }

    private static uint SetBits(uint flags, int offset, int width, uint value)
    {
        uint mask = ((1u << width) - 1u) << offset;
        return (flags & ~mask) | ((value << offset) & mask);
    }

    private static IOException CreateIOException(string message)
    {
        var inner = WindowsNativeMethods.LastWin32Exception();
        return new IOException(message + " " + inner.Message, inner);
    }

    private static IOException CreateIOException(string message, int error)
    {
        var inner = new Win32Exception(error);
        return new IOException(message + " " + inner.Message, inner);
    }

    #endregion

    #region Nested types

    private readonly struct OperationContext
    {
        public OperationContext(IntPtr handle, SerialPortSettings settings)
        {
            Handle = handle;
            Settings = settings;
        }

        public IntPtr Handle { get; }

        public SerialPortSettings Settings { get; }
    }

    private sealed class OverlappedIoOperation : IDisposable
    {
        #region Constructor

        public OverlappedIoOperation(int bufferSize)
        {
            int effectiveBufferSize = Math.Max(1, bufferSize);
            Buffer = Marshal.AllocHGlobal(effectiveBufferSize);

            try
            {
                EventHandle = WindowsNativeMethods.CreateEventW(IntPtr.Zero, true, false, null);
                if (EventHandle == IntPtr.Zero)
                {
                    throw CreateIOException($"{nameof(WindowsNativeMethods.CreateEventW)} failed.");
                }

                Overlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlappedData)));

                var overlapped = new NativeOverlappedData { hEvent = EventHandle };

                Marshal.StructureToPtr(overlapped, Overlapped, false);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        #endregion

        #region Fields

        private bool _ownsResources = true;

        #endregion

        #region Properties

        public IntPtr Buffer { get; private set; }

        public IntPtr Overlapped { get; private set; }

        public IntPtr EventHandle { get; private set; }

        #endregion

        #region Public

        public void AbandonResources()
        {
            _ownsResources = false;
        }

        public void Dispose()
        {
            if (!_ownsResources)
            {
                return;
            }

            if (Overlapped != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Overlapped);
                Overlapped = IntPtr.Zero;
            }

            if (EventHandle != IntPtr.Zero)
            {
                Kernel32.CloseHandle(EventHandle);
                EventHandle = IntPtr.Zero;
            }

            if (Buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Buffer);
                Buffer = IntPtr.Zero;
            }
        }

        #endregion
    }

    #endregion
}
