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
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Backends;

internal sealed class WindowsSerialPortBackend : ISerialPortBackend
{
    #region Fields

    private IntPtr _handle = WindowsNativeMethods.InvalidHandleValue;
    private SerialPortSettings _settings;

    #endregion

    #region Properties

    public bool IsOpen => _handle != WindowsNativeMethods.InvalidHandleValue;

    public int BytesToRead
    {
        get
        {
            EnsureOpen();

            if (!WindowsNativeMethods.ClearCommError(_handle, out _, out var stat))
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
            FileFlagsAndAttributes.Normal,
            IntPtr.Zero);

        if (handle == WindowsNativeMethods.InvalidHandleValue)
        {
            throw CreateIOException($"Could not open serial port '{settings.PortName}'.");
        }

        _handle = handle;
        _settings = settings.Clone();

        try
        {
            WindowsNativeMethods.SetupComm(_handle, 4096, 4096);
            Configure(settings);
            ApplyTimeouts(settings);
            SetDtr(settings.DtrEnable);
            SetRts(settings.RtsEnable);
        }
        catch
        {
            CloseCore();
            throw;
        }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        EnsureOpen();

        if (count == 0)
        {
            return 0;
        }

        var temp = new byte[count];

        if (!WindowsNativeMethods.ReadFile(_handle, temp, count, out int bytesRead, IntPtr.Zero))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.ReadFile)} failed.");
        }

        if (bytesRead == 0)
        {
            throw new TimeoutException("The serial port read operation timed out.");
        }

        Buffer.BlockCopy(temp, 0, buffer, offset, bytesRead);
        return bytesRead;
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        EnsureOpen();

        if (count == 0)
        {
            return;
        }

        var temp = new byte[count];
        Buffer.BlockCopy(buffer, offset, temp, 0, count);

        if (!WindowsNativeMethods.WriteFile(_handle, temp, count, out int bytesWritten, IntPtr.Zero))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.WriteFile)} failed.");
        }

        if (bytesWritten != count)
        {
            throw new TimeoutException($"The serial port write operation timed out after writing {bytesWritten} of {count} bytes.");
        }
    }

    public void DiscardInBuffer()
    {
        EnsureOpen();

        if (!WindowsNativeMethods.PurgeComm(_handle, WindowsNativeMethods.PurgeRxAbort | WindowsNativeMethods.PurgeRxClear))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.PurgeComm)}(RX) failed.");
        }
    }

    public void DiscardOutBuffer()
    {
        EnsureOpen();

        if (!WindowsNativeMethods.PurgeComm(_handle, WindowsNativeMethods.PurgeTxAbort | WindowsNativeMethods.PurgeTxClear))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.PurgeComm)}(TX) failed.");
        }
    }

    public void SetDtr(bool enabled)
    {
        if (!IsOpen)
        {
            return;
        }

        if (!WindowsNativeMethods.EscapeCommFunction(_handle, enabled ? WindowsNativeMethods.Setdtr : WindowsNativeMethods.Clrdtr))
        {
            throw CreateIOException(enabled
                ? $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Setdtr)}) failed."
                : $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Clrdtr)}) failed.");
        }
    }

    public void SetRts(bool enabled)
    {
        if (!IsOpen)
        {
            return;
        }

        if (_settings?.Handshake == Handshake.RequestToSend || _settings?.Handshake == Handshake.RequestToSendXOnXOff)
        {
            return;
        }

        if (!WindowsNativeMethods.EscapeCommFunction(_handle, enabled ? WindowsNativeMethods.Setrts : WindowsNativeMethods.Clrrts))
        {
            throw CreateIOException(enabled
                ? $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Setrts)}) failed."
                : $"{nameof(WindowsNativeMethods.EscapeCommFunction)}({nameof(WindowsNativeMethods.Clrrts)}) failed.");
        }
    }

    public void RequestAbort()
    {
        if (!IsOpen)
        {
            return;
        }

        WindowsNativeMethods.SetCommMask(_handle, 0);

        try
        {
            WindowsNativeMethods.CancelIoEx(_handle, IntPtr.Zero);
        }
        catch (EntryPointNotFoundException)
        {
            // CancelIoEx is Vista+. If unavailable, PurgeComm below is still attempted.
        }

        WindowsNativeMethods.PurgeComm(
            _handle,
            WindowsNativeMethods.PurgeTxAbort |
            WindowsNativeMethods.PurgeRxAbort |
            WindowsNativeMethods.PurgeTxClear |
            WindowsNativeMethods.PurgeRxClear);
    }

    public void CloseCore()
    {
        IntPtr handle = _handle;
        _handle = WindowsNativeMethods.InvalidHandleValue;

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

    private void Configure(SerialPortSettings settings)
    {
        var dcb = new Dcb
        {
            DCBlength = (uint)Marshal.SizeOf(typeof(Dcb))
        };

        if (!WindowsNativeMethods.GetCommState(_handle, ref dcb))
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

        if (!WindowsNativeMethods.SetCommState(_handle, ref dcb))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.SetCommState)} failed.");
        }
    }

    private void ApplyTimeouts(SerialPortSettings settings)
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

        if (!WindowsNativeMethods.SetCommTimeouts(_handle, ref timeouts))
        {
            throw CreateIOException($"{nameof(WindowsNativeMethods.SetCommTimeouts)} failed.");
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

    private void EnsureOpen()
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("The serial port backend is not open.");
        }
    }

    private static IOException CreateIOException(string message)
    {
        var inner = WindowsNativeMethods.LastWin32Exception();
        return new IOException(message + " " + inner.Message, inner);
    }

    #endregion
}
