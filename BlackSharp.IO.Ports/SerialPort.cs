/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using BlackSharp.IO.Ports.Backends;
using System.Text;
using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.IO.Ports;

/// <summary>
/// Cross-platform serial port implementation for Windows and Linux.<br/>
/// This class intentionally does not wrap System.IO.Ports.SerialPort.<br/>
/// It talks to the OS directly so <see cref="Close"/> can be implemented without blocking the caller
/// forever when a USB/VCP driver hangs during close.
/// </summary>
/// <remarks>
/// Supported core API: Open, Close, Read, Write, DTR/RTS, read/write timeouts, input/output purge.<br/>
/// Not implemented: DataReceived event, BaseStream, pin-changed events, Encoding-heavy text helpers.
/// </remarks>
public class SerialPort : IDisposable
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of <see cref="SerialPort"/> using the default port name for the current platform.
    /// </summary>
    public SerialPort()
        : this(GetDefaultPortName())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SerialPort"/> for the specified port name.
    /// </summary>
    /// <param name="portName">The port name to use (e.g. <c>COM1</c> on Windows or <c>/dev/ttyS0</c> on Linux).</param>
    public SerialPort(string portName)
    {
        PortName = portName;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SerialPort"/> with the specified port name and baud rate.
    /// </summary>
    /// <param name="portName">The port name to use (e.g. <c>COM1</c> on Windows or <c>/dev/ttyS0</c> on Linux).</param>
    /// <param name="baudRate">The baud rate.</param>
    public SerialPort(string portName, int baudRate)
        : this(portName)
    {
        BaudRate = baudRate;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SerialPort"/> with the specified port name, baud rate, parity, data bits and stop bits.
    /// </summary>
    /// <param name="portName">The port name to use (e.g. <c>COM1</c> on Windows or <c>/dev/ttyS0</c> on Linux).</param>
    /// <param name="baudRate">The baud rate.</param>
    /// <param name="parity">The parity-checking protocol.</param>
    /// <param name="dataBits">The number of data bits per byte.</param>
    /// <param name="stopBits">The number of stop bits.</param>
    public SerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        : this(portName, baudRate)
    {
        Parity = parity;
        DataBits = dataBits;
        StopBits = stopBits;
    }

    #endregion

    #region Fields

    /// <summary>
    /// Represents an infinite timeout. Pass this value to <see cref="ReadTimeout"/> or <see cref="WriteTimeout"/> to disable the respective timeout.
    /// </summary>
    public const int InfiniteTimeout = -1;

    private readonly object _sync = new object();
    private ISerialPortBackend _backend;
    private bool _opening;
    private bool _closeRequestedDuringOpen;
    private bool _disposed;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of the serial port (e.g. <c>COM1</c> on Windows or <c>/dev/ttyS0</c> on Linux).
    /// </summary>
    public string PortName { get; set; }

    /// <summary>
    /// Gets or sets the baud rate. Default is <c>9600</c>.
    /// </summary>
    public int BaudRate { get; set; } = 9600;

    /// <summary>
    /// Gets or sets the parity-checking protocol. Default is <see cref="Parity.None"/>.
    /// </summary>
    public Parity Parity { get; set; } = Parity.None;

    /// <summary>
    /// Gets or sets the number of data bits per byte. Default is <c>8</c>.
    /// </summary>
    public int DataBits { get; set; } = 8;

    /// <summary>
    /// Gets or sets the number of stop bits. Default is <see cref="StopBits.One"/>.
    /// </summary>
    public StopBits StopBits { get; set; } = StopBits.One;

    /// <summary>
    /// Gets or sets the handshaking protocol for serial data transmission. Default is <see cref="Handshake.None"/>.
    /// </summary>
    public Handshake Handshake { get; set; } = Handshake.None;

    /// <summary>
    /// Gets or sets the read timeout in milliseconds. Use <see cref="InfiniteTimeout"/> to disable. Default is <c>1000</c>.
    /// </summary>
    public int ReadTimeout { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the write timeout in milliseconds. Use <see cref="InfiniteTimeout"/> to disable. Default is <c>1000</c>.
    /// </summary>
    public int WriteTimeout { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum time to wait for the native close operation to complete before delegating it to a background thread.
    /// Default is <c>1</c> second.
    /// </summary>
    public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the character encoding used for text-based read and write operations. Default is <see cref="Encoding.ASCII"/>.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.ASCII;

    /// <summary>
    /// Gets a value indicating whether the serial port is currently open.
    /// </summary>
    public bool IsOpen
    {
        get
        {
            lock (_sync)
                return _backend?.IsOpen == true;
        }
    }

    /// <summary>
    /// Gets the number of bytes available in the receive buffer.
    /// </summary>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public int BytesToRead
    {
        get
        {
            return GetOpenBackendSnapshot().BytesToRead;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Data Terminal Ready (DTR) signal is enabled.<br/>
    /// When the port is open the signal is applied immediately.
    /// </summary>
    public bool DtrEnable
    {
        get => _dtrEnable;
        set
        {
            ISerialPortBackend backend;

            lock (_sync)
            {
                _dtrEnable = value;
                backend = _backend;
            }

            backend?.SetDtr(value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Request to Send (RTS) signal is enabled.<br/>
    /// When the port is open the signal is applied immediately.
    /// </summary>
    public bool RtsEnable
    {
        get => _rtsEnable;
        set
        {
            ISerialPortBackend backend;

            lock (_sync)
            {
                _rtsEnable = value;
                backend = _backend;
            }

            backend?.SetRts(value);
        }
    }

    private bool _dtrEnable;
    private bool _rtsEnable;

    #endregion

    #region Public

    /// <summary>
    /// Opens the serial port connection using the current settings.
    /// </summary>
    /// <exception cref="InvalidOperationException">The port is already open.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void Open()
    {
        ThrowIfDisposed();
        ValidateSettings();

        SerialPortSettings settings;

        lock (_sync)
        {
            if (_backend?.IsOpen == true || _opening)
            {
                throw new InvalidOperationException("The serial port is already open.");
            }

            settings = CreateSettings();
            _closeRequestedDuringOpen = false;
            _opening = true;
        }

        ISerialPortBackend backend = CreateBackend();
        bool assigned = false;

        try
        {
            backend.Open(settings);

            lock (_sync)
            {
                _opening = false;

                if (_disposed || _closeRequestedDuringOpen)
                {
                    _closeRequestedDuringOpen = false;
                    assigned = false;
                }
                else
                {
                    _backend = backend;
                    assigned = true;
                }
            }

            if (!assigned)
            {
                CloseDetached(backend, CloseTimeout);
            }
        }
        catch
        {
            lock (_sync)
            {
                _opening = false;
                _closeRequestedDuringOpen = false;
            }

            throw;
        }
    }

    /// <summary>
    /// Uses <see cref="CloseTimeout"/> and throws <see cref="TimeoutException"/> if the native close path
    /// does not complete within that timeout.<br/>
    /// Use <see cref="TryClose"/> if you prefer a bool result.
    /// </summary>
    public void Close()
    {
        if (!TryClose(CloseTimeout))
        {
            throw new TimeoutException($"Closing serial port '{PortName}' did not complete within {CloseTimeout.TotalMilliseconds:0} ms.");
        }
    }

    /// <summary>
    /// Attempts to close the port without allowing the caller to block forever.<br/>
    /// Returns true if the native close completed within the timeout; false if close continues on
    /// a background thread.<br/>
    /// After a false result, discard this SerialPort instance.
    /// </summary>
    public bool TryClose(TimeSpan timeout)
    {
        ValidateTimeout(timeout, nameof(timeout), allowInfinite: true);

        ISerialPortBackend backend;

        lock (_sync)
        {
            if (_opening && _backend == null)
            {
                _closeRequestedDuringOpen = true;
            }

            backend = _backend;
            _backend = null;
        }

        if (backend == null)
        {
            return true;
        }

        return CloseDetached(backend, timeout);
    }

    /// <summary>
    /// Reads up to <paramref name="count"/> bytes from the port into <paramref name="buffer"/> starting at <paramref name="offset"/>.
    /// </summary>
    /// <param name="buffer">The buffer to read into.</param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing data.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>The number of bytes actually read.</returns>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public int Read(byte[] buffer, int offset, int count)
    {
        ValidateBuffer(buffer, offset, count);

        var backend = GetOpenBackendSnapshot();
        return backend.Read(buffer, offset, count);
    }

    /// <summary>
    /// Reads a single byte from the port.
    /// </summary>
    /// <returns>The byte read as an <see cref="int"/>, or <c>-1</c> if no data is available.</returns>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public int ReadByte()
    {
        var buffer = new byte[1];
        int read = Read(buffer, 0, 1);

        return read == 0 ? -1 : buffer[0];
    }

    /// <summary>
    /// Reads all bytes currently available in the receive buffer and returns them decoded as a string using <see cref="Encoding"/>.<br/>
    /// Returns <see cref="string.Empty"/> if no data is available.
    /// </summary>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public string ReadExisting()
    {
        int available = BytesToRead;
        if (available <= 0)
        {
            return string.Empty;
        }

        var buffer = new byte[available];
        int read = Read(buffer, 0, buffer.Length);

        return Encoding.GetString(buffer, 0, read);
    }

    /// <summary>
    /// Writes <paramref name="count"/> bytes from <paramref name="buffer"/> starting at <paramref name="offset"/> to the port.
    /// </summary>
    /// <param name="buffer">The buffer containing the data to write.</param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin reading data.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public void Write(byte[] buffer, int offset, int count)
    {
        ValidateBuffer(buffer, offset, count);

        var backend = GetOpenBackendSnapshot();
        backend.Write(buffer, offset, count);
    }

    /// <summary>
    /// Writes the entire byte array to the port.
    /// </summary>
    /// <param name="buffer">The buffer containing the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public void Write(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        Write(buffer, 0, buffer.Length);
    }

    /// <summary>
    /// Encodes <paramref name="text"/> using <see cref="Encoding"/> and writes it to the port.
    /// </summary>
    /// <param name="text">The string to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public void Write(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        byte[] buffer = Encoding.GetBytes(text);
        Write(buffer, 0, buffer.Length);
    }

    /// <summary>
    /// Discards all data from the receive buffer.
    /// </summary>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public void DiscardInBuffer()
    {
        var backend = GetOpenBackendSnapshot();
        backend.DiscardInBuffer();
    }

    /// <summary>
    /// Discards all data from the transmit buffer.
    /// </summary>
    /// <exception cref="InvalidOperationException">The port is not open.</exception>
    public void DiscardOutBuffer()
    {
        var backend = GetOpenBackendSnapshot();
        backend.DiscardOutBuffer();
    }

    /// <summary>
    /// Releases all resources used by this <see cref="SerialPort"/> and closes the port if it is open.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            TryClose(CloseTimeout);
        }
        catch
        {
            // IDisposable should not take down callers during cleanup.
        }
    }

    /// <summary>
    /// Returns a list of available serial port names on the current platform.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of port name strings, or an empty list if the platform is not supported.</returns>
    public static List<string> GetPortNames()
    {
        if (OS.IsWindows())
        {
            return WindowsSerialPortBackend.GetPortNames();
        }

        if (OS.IsLinux())
        {
            return LinuxSerialPortBackend.GetPortNames();
        }

        return new List<string>();
    }

    #endregion

    #region Private

    private SerialPortSettings CreateSettings()
    {
        return new SerialPortSettings
        {
            PortName     = this.PortName,
            BaudRate     = this.BaudRate,
            Parity       = this.Parity,
            DataBits     = this.DataBits,
            StopBits     = this.StopBits,
            Handshake    = this.Handshake,
            DtrEnable    = _dtrEnable,
            RtsEnable    = _rtsEnable,
            ReadTimeout  = this.ReadTimeout,
            WriteTimeout = this.WriteTimeout
        };
    }

    private bool CloseDetached(ISerialPortBackend backend, TimeSpan timeout)
    {
        bool completed = SerialPortCloseWorker.Close(backend, timeout, out var exception);

        if (!completed)
        {
            return false;
        }

        if (exception != null)
        {
            throw new IOException("The serial port close operation failed.", exception);
        }

        return true;
    }

    private static ISerialPortBackend CreateBackend()
    {
        if (OS.IsWindows())
        {
            return new WindowsSerialPortBackend();
        }

        if (OS.IsLinux())
        {
            return new LinuxSerialPortBackend();
        }

        throw new PlatformNotSupportedException($"This {nameof(SerialPort)} implementation currently supports Windows and Linux.");
    }

    private ISerialPortBackend GetOpenBackendSnapshot()
    {
        ThrowIfDisposed();

        lock (_sync)
        {
            if (_backend == null || !_backend.IsOpen)
            {
                throw new InvalidOperationException("The serial port is not open.");
            }

            return _backend;
        }
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(PortName))
        {
            throw new ArgumentException("Must not be empty.", nameof(PortName));
        }

        if (BaudRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(BaudRate));
        }

        if (DataBits < 5 || DataBits > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(DataBits), "Must be between 5 and 8.");
        }

        if (StopBits == StopBits.None)
        {
            throw new ArgumentOutOfRangeException(nameof(StopBits), $"{nameof(StopBits)}.{StopBits.None} is not valid for opening a serial port.");
        }

        ValidateMillisecondsTimeout(ReadTimeout , nameof(ReadTimeout ));
        ValidateMillisecondsTimeout(WriteTimeout, nameof(WriteTimeout));

        ValidateTimeout(CloseTimeout, nameof(CloseTimeout), allowInfinite: true);
    }

    private static void ValidateMillisecondsTimeout(int timeout, string name)
    {
        if (timeout < 0 && timeout != InfiniteTimeout)
        {
            throw new ArgumentOutOfRangeException(name, $"Timeout must be non-negative or {nameof(SerialPort)}.{nameof(InfiniteTimeout)}.");
        }
    }

    private static void ValidateTimeout(TimeSpan timeout, string name, bool allowInfinite)
    {
        if (allowInfinite && timeout == Timeout.InfiniteTimeSpan)
        {
            return;
        }

        if (timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }

    private static void ValidateBuffer(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0 || offset > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0 || count > buffer.Length - offset)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SerialPort));
        }
    }

    private static string GetDefaultPortName()
    {
        if (OS.IsWindows())
        {
            return "COM1";
        }

        if (OS.IsLinux())
        {
            return "/dev/ttyS0";
        }

        return string.Empty;
    }

    #endregion
}
