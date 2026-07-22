/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.IO.Ports.Interop.Linux;
using BlackSharp.IO.Ports.Interop.Linux.Structures;
using BlackSharp.IO.Ports.Models;
using System.Diagnostics;

namespace BlackSharp.IO.Ports.Backends;

internal sealed class LinuxSerialPortBackend : ISerialPortBackend
{
    #region Fields

    private int _fd = -1;
    private SerialPortSettings _settings;

    #endregion

    #region Properties

    public bool IsOpen => _fd >= 0;

    public int BytesToRead
    {
        get
        {
            EnsureOpen();
            int value = 0;

            if (LinuxNativeMethods.ioctl(_fd, LinuxNativeMethods.FIONREAD, ref value) != 0)
            {
                throw CreateIOException($"{nameof(LinuxNativeMethods.ioctl)}({nameof(LinuxNativeMethods.FIONREAD)}) failed.");
            }

            return Math.Max(0, value);
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

        string path = NormalizePortName(settings.PortName);
        int fd = LinuxNativeMethods.open(path, LinuxNativeMethods.O_RDWR | LinuxNativeMethods.O_NOCTTY | LinuxNativeMethods.O_NONBLOCK | LinuxNativeMethods.O_CLOEXEC);

        if (fd < 0)
        {
            throw CreateIOException($"Could not open serial port '{settings.PortName}'.");
        }

        _fd = fd;
        _settings = settings.Clone();

        try
        {
            Configure(settings);
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

        int total = 0;
        int timeout = _settings?.ReadTimeout ?? SerialPort.InfiniteTimeout;
        var stopwatch = timeout == SerialPort.InfiniteTimeout ? null : Stopwatch.StartNew();

        while (total < count)
        {
            int pollTimeout = total > 0 ? 0 : GetRemainingTimeout(timeout, stopwatch);

            short events = Poll(LinuxNativeMethods.POLLIN, pollTimeout);

            if ((events & (LinuxNativeMethods.POLLERR | LinuxNativeMethods.POLLHUP | LinuxNativeMethods.POLLNVAL)) != 0 && (events & LinuxNativeMethods.POLLIN) == 0)
            {
                throw new IOException("The serial port reported an error or hangup while reading.");
            }

            if ((events & LinuxNativeMethods.POLLIN) == 0)
            {
                if (total > 0)
                {
                    return total;
                }

                throw new TimeoutException("The serial port read operation timed out.");
            }

            int remaining = count - total;
            var temp = new byte[remaining];

            IntPtr result = LinuxNativeMethods.read(_fd, temp, (UIntPtr)temp.Length);

            long n = result.ToInt64();

            if (n > 0)
            {
                Buffer.BlockCopy(temp, 0, buffer, offset + total, checked((int)n));

                total += checked((int)n);
                continue;
            }

            if (n == 0)
            {
                if (total > 0)
                {
                    return total;
                }

                throw new IOException("The serial port reached end-of-file while reading.");
            }

            int errno = LinuxNativeMethods.Errno;
            if (errno == LinuxNativeMethods.EINTR)
            {
                continue;
            }

            if (errno == LinuxNativeMethods.EAGAIN)
            {
                if (total > 0)
                {
                    return total;
                }

                continue;
            }

            throw CreateIOException($"{nameof(LinuxNativeMethods.read)} failed.");
        }

        return total;
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        EnsureOpen();

        if (count == 0)
        {
            return;
        }

        int total = 0;
        int timeout = _settings?.WriteTimeout ?? SerialPort.InfiniteTimeout;
        var stopwatch = timeout == SerialPort.InfiniteTimeout ? null : Stopwatch.StartNew();

        while (total < count)
        {
            int pollTimeout = GetRemainingTimeout(timeout, stopwatch);
            short events = Poll(LinuxNativeMethods.POLLOUT, pollTimeout);

            if ((events & (LinuxNativeMethods.POLLERR | LinuxNativeMethods.POLLHUP | LinuxNativeMethods.POLLNVAL)) != 0 && (events & LinuxNativeMethods.POLLOUT) == 0)
            {
                throw new IOException("The serial port reported an error or hangup while writing.");
            }

            if ((events & LinuxNativeMethods.POLLOUT) == 0)
            {
                throw new TimeoutException($"The serial port write operation timed out after writing {total} of {count} bytes.");
            }

            int remaining = count - total;
            var temp = new byte[remaining];

            Buffer.BlockCopy(buffer, offset + total, temp, 0, remaining);

            IntPtr result = LinuxNativeMethods.write(_fd, temp, (UIntPtr)temp.Length);
            long n = result.ToInt64();

            if (n > 0)
            {
                total += checked((int)n);
                continue;
            }

            if (n == 0)
            {
                throw new TimeoutException($"The serial port write operation wrote 0 bytes after writing {total} of {count} bytes.");
            }

            int errno = LinuxNativeMethods.Errno;
            if (errno == LinuxNativeMethods.EINTR || errno == LinuxNativeMethods.EAGAIN)
            {
                continue;
            }

            throw CreateIOException($"{nameof(LinuxNativeMethods.write)} failed.");
        }
    }

    public void DiscardInBuffer()
    {
        EnsureOpen();

        if (LinuxNativeMethods.tcflush(_fd, LinuxNativeMethods.TCIFLUSH) != 0)
        {
            throw CreateIOException($"{nameof(LinuxNativeMethods.tcflush)}({nameof(LinuxNativeMethods.TCIFLUSH)}) failed.");
        }
    }

    public void DiscardOutBuffer()
    {
        EnsureOpen();

        if (LinuxNativeMethods.tcflush(_fd, LinuxNativeMethods.TCOFLUSH) != 0)
        {
            throw CreateIOException($"{nameof(LinuxNativeMethods.tcflush)}({nameof(LinuxNativeMethods.TCOFLUSH)}) failed.");
        }
    }

    public void SetDtr(bool enabled)
    {
        if (!IsOpen)
        {
            return;
        }

        int bits = LinuxNativeMethods.TIOCM_DTR;
        int request = enabled ? LinuxNativeMethods.TIOCMBIS : LinuxNativeMethods.TIOCMBIC;

        if (LinuxNativeMethods.ioctl(_fd, request, ref bits) != 0)
        {
            throw CreateIOException(enabled
                ? $"{nameof(LinuxNativeMethods.ioctl)}({nameof(LinuxNativeMethods.TIOCMBIS)}, DTR) failed."
                : $"{nameof(LinuxNativeMethods.ioctl)}({nameof(LinuxNativeMethods.TIOCMBIC)}, DTR) failed.");
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

        int bits = LinuxNativeMethods.TIOCM_RTS;
        int request = enabled ? LinuxNativeMethods.TIOCMBIS : LinuxNativeMethods.TIOCMBIC;

        if (LinuxNativeMethods.ioctl(_fd, request, ref bits) != 0)
        {
            throw CreateIOException(enabled
                ? $"{nameof(LinuxNativeMethods.ioctl)}({nameof(LinuxNativeMethods.TIOCMBIS)}, RTS) failed."
                : $"{nameof(LinuxNativeMethods.ioctl)}({nameof(LinuxNativeMethods.TIOCMBIC)}, RTS) failed.");
        }
    }

    public void RequestAbort()
    {
        if (!IsOpen)
        {
            return;
        }

        LinuxNativeMethods.tcflush(_fd, LinuxNativeMethods.TCIOFLUSH);
    }

    public void CloseCore()
    {
        int fd = _fd;
        _fd = -1;

        if (fd < 0)
        {
            return;
        }

        LinuxNativeMethods.tcflush(fd, LinuxNativeMethods.TCIOFLUSH);
        LinuxNativeMethods.close(fd);
    }

    public static List<string> GetPortNames()
    {
        var ports = new SortedSet<string>(StringComparer.Ordinal);

        AddMatches(ports, "/dev", "ttyS*");
        AddMatches(ports, "/dev", "ttyUSB*");
        AddMatches(ports, "/dev", "ttyACM*");
        AddMatches(ports, "/dev", "ttyAMA*");
        AddMatches(ports, "/dev", "rfcomm*");
        AddMatches(ports, "/dev/serial/by-id", "*");

        return ports.ToList();
    }

    #endregion

    #region Private

    private void Configure(SerialPortSettings settings)
    {
        var termios = new Termios { c_cc = new byte[LinuxNativeMethods.NCCS] };

        if (LinuxNativeMethods.tcgetattr(_fd, ref termios) != 0)
        {
            throw CreateIOException($"{nameof(LinuxNativeMethods.tcgetattr)} failed.");
        }

        LinuxNativeMethods.cfmakeraw(ref termios);

        termios.c_cflag |= LinuxNativeMethods.CLOCAL | LinuxNativeMethods.CREAD;
        termios.c_cflag &= ~LinuxNativeMethods.CSIZE;
        termios.c_cflag |= ToLinuxDataBits(settings.DataBits);

        if (settings.StopBits == StopBits.Two)
        {
            termios.c_cflag |= LinuxNativeMethods.CSTOPB;
        }
        else
        {
            termios.c_cflag &= ~LinuxNativeMethods.CSTOPB;
        }

        termios.c_cflag &= ~(LinuxNativeMethods.PARENB | LinuxNativeMethods.PARODD | LinuxNativeMethods.CMSPAR);

        switch (settings.Parity)
        {
            case Parity.None:
                break;
            case Parity.Even:
                termios.c_cflag |= LinuxNativeMethods.PARENB;
                break;
            case Parity.Odd:
                termios.c_cflag |= LinuxNativeMethods.PARENB | LinuxNativeMethods.PARODD;
                break;
            case Parity.Mark:
                termios.c_cflag |= LinuxNativeMethods.PARENB | LinuxNativeMethods.CMSPAR | LinuxNativeMethods.PARODD;
                break;
            case Parity.Space:
                termios.c_cflag |= LinuxNativeMethods.PARENB | LinuxNativeMethods.CMSPAR;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(settings.Parity));
        }

        if (settings.Handshake == Handshake.RequestToSend || settings.Handshake == Handshake.RequestToSendXOnXOff)
        {
            termios.c_cflag |= LinuxNativeMethods.CRTSCTS;
        }
        else
        {
            termios.c_cflag &= ~LinuxNativeMethods.CRTSCTS;
        }

        if (settings.Handshake == Handshake.XOnXOff || settings.Handshake == Handshake.RequestToSendXOnXOff)
        {
            termios.c_iflag |= LinuxNativeMethods.IXON | LinuxNativeMethods.IXOFF;
        }
        else
        {
            termios.c_iflag &= ~(LinuxNativeMethods.IXON | LinuxNativeMethods.IXOFF);
        }

        termios.c_cc[LinuxNativeMethods.VMIN] = 0;
        termios.c_cc[LinuxNativeMethods.VTIME] = 0;

        uint speed = ToLinuxBaudRate(settings.BaudRate);
        if (LinuxNativeMethods.cfsetispeed(ref termios, speed) != 0)
        {
            throw CreateIOException($"{nameof(LinuxNativeMethods.cfsetispeed)} failed.");
        }

        if (LinuxNativeMethods.cfsetospeed(ref termios, speed) != 0)
        {
            throw CreateIOException($"{nameof(LinuxNativeMethods.cfsetospeed)} failed.");
        }

        if (LinuxNativeMethods.tcsetattr(_fd, LinuxNativeMethods.TCSANOW, ref termios) != 0)
        {
            throw CreateIOException($"{nameof(LinuxNativeMethods.tcsetattr)} failed.");
        }

        LinuxNativeMethods.tcflush(_fd, LinuxNativeMethods.TCIOFLUSH);
    }

    private short Poll(short events, int timeoutMs)
    {
        while (true)
        {
            var pollFd = new PollFd
            {
                fd = _fd,
                events = events,
                revents = 0
            };

            int result = LinuxNativeMethods.poll(ref pollFd, (UIntPtr)1, timeoutMs);
            if (result > 0)
            {
                return pollFd.revents;
            }

            if (result == 0)
            {
                return 0;
            }

            int errno = LinuxNativeMethods.Errno;
            if (errno == LinuxNativeMethods.EINTR)
            {
                continue;
            }

            throw CreateIOException("poll() failed.");
        }
    }

    private static int GetRemainingTimeout(int timeout, Stopwatch stopwatch)
    {
        if (timeout == SerialPort.InfiniteTimeout)
        {
            return -1;
        }

        if (timeout == 0)
        {
            return 0;
        }

        if (stopwatch == null)
        {
            return timeout;
        }

        long remaining = timeout - stopwatch.ElapsedMilliseconds;
        return remaining <= 0 ? 0 : remaining > int.MaxValue ? int.MaxValue : (int)remaining;
    }

    private static string NormalizePortName(string portName)
    {
        if (portName.StartsWith("/", StringComparison.Ordinal))
        {
            return portName;
        }

        return "/dev/" + portName;
    }

    private static uint ToLinuxDataBits(int dataBits)
    {
        return dataBits switch
        {
            5 => LinuxNativeMethods.CS5,
            6 => LinuxNativeMethods.CS6,
            7 => LinuxNativeMethods.CS7,
            8 => LinuxNativeMethods.CS8,
            _ => throw new ArgumentOutOfRangeException(nameof(dataBits))
        };
    }

    private static uint ToLinuxBaudRate(int baudRate)
    {
        return baudRate switch
        {
            0       => LinuxNativeMethods.B0,
            50      => LinuxNativeMethods.B50,
            75      => LinuxNativeMethods.B75,
            110     => LinuxNativeMethods.B110,
            134     => LinuxNativeMethods.B134,
            150     => LinuxNativeMethods.B150,
            200     => LinuxNativeMethods.B200,
            300     => LinuxNativeMethods.B300,
            600     => LinuxNativeMethods.B600,
            1200    => LinuxNativeMethods.B1200,
            1800    => LinuxNativeMethods.B1800,
            2400    => LinuxNativeMethods.B2400,
            4800    => LinuxNativeMethods.B4800,
            9600    => LinuxNativeMethods.B9600,
            19200   => LinuxNativeMethods.B19200,
            38400   => LinuxNativeMethods.B38400,
            57600   => LinuxNativeMethods.B57600,
            115200  => LinuxNativeMethods.B115200,
            230400  => LinuxNativeMethods.B230400,
            460800  => LinuxNativeMethods.B460800,
            500000  => LinuxNativeMethods.B500000,
            576000  => LinuxNativeMethods.B576000,
            921600  => LinuxNativeMethods.B921600,
            1000000 => LinuxNativeMethods.B1000000,
            1152000 => LinuxNativeMethods.B1152000,
            1500000 => LinuxNativeMethods.B1500000,
            2000000 => LinuxNativeMethods.B2000000,
            2500000 => LinuxNativeMethods.B2500000,
            3000000 => LinuxNativeMethods.B3000000,
            3500000 => LinuxNativeMethods.B3500000,
            4000000 => LinuxNativeMethods.B4000000,
            _ => throw new NotSupportedException($"Baud rate {baudRate} is not mapped for Linux in this implementation.")
        };
    }

    private void EnsureOpen()
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("The serial port backend is not open.");
        }
    }

    private static void AddMatches(SortedSet<string> ports, string directory, string pattern)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var path in Directory.GetFiles(directory, pattern))
        {
            ports.Add(path);
        }
    }

    private static string[] ToArray(SortedSet<string> ports)
    {
        var result = new string[ports.Count];
        ports.CopyTo(result);

        return result;
    }

    private static IOException CreateIOException(string message)
    {
        var inner = LinuxNativeMethods.LastErrnoException();
        return new IOException(message + " " + inner.Message, inner);
    }

    #endregion
}
