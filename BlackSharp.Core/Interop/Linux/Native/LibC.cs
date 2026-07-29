/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Linux.Structures;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Native;

/// <summary>
/// Provides selected Linux C library functions, flags, and status values.
/// </summary>
public static class LibC
{
    #region Fields

    private const string LIBRARY_NAME = "libc";

    /// <summary>Opens a file for read-only access.</summary>
    public const int O_RDONLY = 0x0000;
    /// <summary>Opens a file for write-only access.</summary>
    public const int O_WRONLY = 0x0001;
    /// <summary>Opens a file for read and write access.</summary>
    public const int O_RDWR = 0x0002;
    /// <summary>Prevents a terminal device from becoming the controlling terminal.</summary>
    public const int O_NOCTTY = 0x0100;
    /// <summary>Enables nonblocking file operations.</summary>
    public const int O_NONBLOCK = 0x0800;
    /// <summary>Closes the file descriptor during a successful exec operation.</summary>
    public const int O_CLOEXEC = 0x080000;

    /// <summary>Indicates that a system call was interrupted.</summary>
    public const int EINTR = 4;
    /// <summary>Indicates that a nonblocking operation should be retried later.</summary>
    public const int EAGAIN = 11;

    /// <summary>Requests notification when data can be read.</summary>
    public const short POLLIN = 0x0001;
    /// <summary>Requests notification when data can be written.</summary>
    public const short POLLOUT = 0x0004;
    /// <summary>Reports an error condition on a file descriptor.</summary>
    public const short POLLERR = 0x0008;
    /// <summary>Reports that the peer has closed or disconnected.</summary>
    public const short POLLHUP = 0x0010;
    /// <summary>Reports an invalid file descriptor.</summary>
    public const short POLLNVAL = 0x0020;

    /// <summary>Applies terminal attributes immediately.</summary>
    public const int TCSANOW = 0;
    /// <summary>Discards unread terminal input.</summary>
    public const int TCIFLUSH = 0;
    /// <summary>Discards queued terminal output.</summary>
    public const int TCOFLUSH = 1;
    /// <summary>Discards queued terminal input and output.</summary>
    public const int TCIOFLUSH = 2;

    /// <summary>Defines the number of terminal control characters.</summary>
    public const int NCCS = 32;
    /// <summary>Identifies the inter-byte timer control character.</summary>
    public const int VTIME = 5;
    /// <summary>Identifies the minimum-read-count control character.</summary>
    public const int VMIN = 6;

    /// <summary>Enables XON output-flow control.</summary>
    public const uint IXON = 0x00000400;
    /// <summary>Enables XOFF input-flow control.</summary>
    public const uint IXOFF = 0x00001000;

    /// <summary>Selects the character-size bits in the control flags.</summary>
    public const uint CSIZE = 0x00000030;
    /// <summary>Selects five data bits per character.</summary>
    public const uint CS5 = 0x00000000;
    /// <summary>Selects six data bits per character.</summary>
    public const uint CS6 = 0x00000010;
    /// <summary>Selects seven data bits per character.</summary>
    public const uint CS7 = 0x00000020;
    /// <summary>Selects eight data bits per character.</summary>
    public const uint CS8 = 0x00000030;
    /// <summary>Selects two stop bits instead of one.</summary>
    public const uint CSTOPB = 0x00000040;
    /// <summary>Enables the serial receiver.</summary>
    public const uint CREAD = 0x00000080;
    /// <summary>Enables parity generation and checking.</summary>
    public const uint PARENB = 0x00000100;
    /// <summary>Selects odd parity instead of even parity.</summary>
    public const uint PARODD = 0x00000200;
    /// <summary>Lowers modem control lines after the final close.</summary>
    public const uint HUPCL = 0x00000400;
    /// <summary>Ignores modem control lines for a local connection.</summary>
    public const uint CLOCAL = 0x00000800;
    /// <summary>Enables mark or space parity.</summary>
    public const uint CMSPAR = 0x40000000;
    /// <summary>Enables RTS/CTS hardware flow control.</summary>
    public const uint CRTSCTS = 0x80000000;

    /// <summary>Represents a hung-up terminal line.</summary>
    public const uint B0 = 0;
    /// <summary>Represents a 50 baud terminal speed.</summary>
    public const uint B50 = 1;
    /// <summary>Represents a 75 baud terminal speed.</summary>
    public const uint B75 = 2;
    /// <summary>Represents a 110 baud terminal speed.</summary>
    public const uint B110 = 3;
    /// <summary>Represents a 134 baud terminal speed.</summary>
    public const uint B134 = 4;
    /// <summary>Represents a 150 baud terminal speed.</summary>
    public const uint B150 = 5;
    /// <summary>Represents a 200 baud terminal speed.</summary>
    public const uint B200 = 6;
    /// <summary>Represents a 300 baud terminal speed.</summary>
    public const uint B300 = 7;
    /// <summary>Represents a 600 baud terminal speed.</summary>
    public const uint B600 = 8;
    /// <summary>Represents a 1200 baud terminal speed.</summary>
    public const uint B1200 = 9;
    /// <summary>Represents an 1800 baud terminal speed.</summary>
    public const uint B1800 = 10;
    /// <summary>Represents a 2400 baud terminal speed.</summary>
    public const uint B2400 = 11;
    /// <summary>Represents a 4800 baud terminal speed.</summary>
    public const uint B4800 = 12;
    /// <summary>Represents a 9600 baud terminal speed.</summary>
    public const uint B9600 = 13;
    /// <summary>Represents a 19200 baud terminal speed.</summary>
    public const uint B19200 = 14;
    /// <summary>Represents a 38400 baud terminal speed.</summary>
    public const uint B38400 = 15;
    /// <summary>Represents a 57600 baud terminal speed.</summary>
    public const uint B57600 = 4097;
    /// <summary>Represents a 115200 baud terminal speed.</summary>
    public const uint B115200 = 4098;
    /// <summary>Represents a 230400 baud terminal speed.</summary>
    public const uint B230400 = 4099;
    /// <summary>Represents a 460800 baud terminal speed.</summary>
    public const uint B460800 = 4100;
    /// <summary>Represents a 500000 baud terminal speed.</summary>
    public const uint B500000 = 4101;
    /// <summary>Represents a 576000 baud terminal speed.</summary>
    public const uint B576000 = 4102;
    /// <summary>Represents a 921600 baud terminal speed.</summary>
    public const uint B921600 = 4103;
    /// <summary>Represents a 1000000 baud terminal speed.</summary>
    public const uint B1000000 = 4104;
    /// <summary>Represents a 1152000 baud terminal speed.</summary>
    public const uint B1152000 = 4105;
    /// <summary>Represents a 1500000 baud terminal speed.</summary>
    public const uint B1500000 = 4106;
    /// <summary>Represents a 2000000 baud terminal speed.</summary>
    public const uint B2000000 = 4107;
    /// <summary>Represents a 2500000 baud terminal speed.</summary>
    public const uint B2500000 = 4108;
    /// <summary>Represents a 3000000 baud terminal speed.</summary>
    public const uint B3000000 = 4109;
    /// <summary>Represents a 3500000 baud terminal speed.</summary>
    public const uint B3500000 = 4110;
    /// <summary>Represents a 4000000 baud terminal speed.</summary>
    public const uint B4000000 = 4111;

    /// <summary>Retrieves modem control-line bits.</summary>
    public const int TIOCMGET = 0x5415;
    /// <summary>Sets selected modem control-line bits.</summary>
    public const int TIOCMBIS = 0x5416;
    /// <summary>Clears selected modem control-line bits.</summary>
    public const int TIOCMBIC = 0x5417;
    /// <summary>Replaces all modem control-line bits.</summary>
    public const int TIOCMSET = 0x5418;
    /// <summary>Retrieves the number of bytes available for reading.</summary>
    public const int FIONREAD = 0x541B;

    /// <summary>Identifies the data-terminal-ready control line.</summary>
    public const int TIOCM_DTR = 0x002;
    /// <summary>Identifies the request-to-send control line.</summary>
    public const int TIOCM_RTS = 0x004;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the error number captured by the most recent native call on the current thread.
    /// </summary>
    public static int Errno => Marshal.GetLastWin32Error();

    #endregion

    #region Imports

    /// <summary>
    /// Configures a terminal structure for raw input and output.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern void cfmakeraw(ref Termios termios);

    /// <summary>
    /// Sets the encoded input speed in a terminal structure.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int cfsetispeed(ref Termios termios, uint speed);

    /// <summary>
    /// Sets the encoded output speed in a terminal structure.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int cfsetospeed(ref Termios termios, uint speed);

    /// <summary>
    /// Closes a file descriptor.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int close(int fd);

    /// <summary>
    /// Performs a device-control request with an integer argument.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int ioctl(int fd, int request, ref int argp);

    /// <summary>
    /// Performs a device-control request with an unmanaged argument.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int ioctl(SafeFileHandle fd, UIntPtr request, IntPtr argument);

    /// <summary>
    /// Opens a file using the supplied flags.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int open(string pathname, int flags);

    /// <summary>
    /// Opens or creates a file using the supplied flags and mode.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int open(string pathname, int flags, int mode);

    /// <summary>
    /// Waits for events on a file descriptor.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int poll(ref PollFd fds, UIntPtr nfds, int timeout);

    /// <summary>
    /// Reads bytes from a file descriptor.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern IntPtr read(int fd, byte[] buffer, UIntPtr count);

    /// <summary>
    /// Reads the target of a symbolic link.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern IntPtr readlink(string path, byte[] buffer, UIntPtr bufferSize);

    /// <summary>
    /// Discards queued terminal input, output, or both.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int tcflush(int fd, int queueSelector);

    /// <summary>
    /// Retrieves the attributes of a terminal.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int tcgetattr(int fd, ref Termios termios);

    /// <summary>
    /// Applies attributes to a terminal.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern int tcsetattr(int fd, int optionalActions, ref Termios termios);

    /// <summary>
    /// Writes bytes to a file descriptor.
    /// </summary>
    [DllImport(LIBRARY_NAME, SetLastError = true)]
    public static extern IntPtr write(int fd, byte[] buffer, UIntPtr count);

    #endregion

    #region Public

    /// <summary>
    /// Creates an exception for the error number captured by the most recent native call.
    /// </summary>
    /// <returns>An exception containing the native error code and message.</returns>
    public static Win32Exception LastErrnoException()
    {
        return new Win32Exception(Errno);
    }

    #endregion
}
