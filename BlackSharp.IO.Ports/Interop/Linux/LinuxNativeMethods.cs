/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using BlackSharp.IO.Ports.Interop.Linux.Structures;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Linux;

internal static class LinuxNativeMethods
{
    #region Fields

    private const string LibC = "libc";

    public const int O_RDONLY = 0x0000;
    public const int O_WRONLY = 0x0001;
    public const int O_RDWR = 0x0002;
    public const int O_NOCTTY = 0x0100;
    public const int O_NONBLOCK = 0x0800;
    public const int O_CLOEXEC = 0x080000;

    public const int EINTR = 4;
    public const int EAGAIN = 11;

    public const short POLLIN = 0x0001;
    public const short POLLOUT = 0x0004;
    public const short POLLERR = 0x0008;
    public const short POLLHUP = 0x0010;
    public const short POLLNVAL = 0x0020;

    public const int TCSANOW = 0;
    public const int TCIFLUSH = 0;
    public const int TCOFLUSH = 1;
    public const int TCIOFLUSH = 2;

    public const int NCCS = 32;
    public const int VTIME = 5;
    public const int VMIN = 6;

    public const uint IXON = 0x00000400;
    public const uint IXOFF = 0x00001000;

    public const uint CSIZE = 0x00000030;
    public const uint CS5 = 0x00000000;
    public const uint CS6 = 0x00000010;
    public const uint CS7 = 0x00000020;
    public const uint CS8 = 0x00000030;
    public const uint CSTOPB = 0x00000040;
    public const uint CREAD = 0x00000080;
    public const uint PARENB = 0x00000100;
    public const uint PARODD = 0x00000200;
    public const uint HUPCL = 0x00000400;
    public const uint CLOCAL = 0x00000800;
    public const uint CMSPAR = 0x40000000;
    public const uint CRTSCTS = 0x80000000;

    public const uint B0 = 0;
    public const uint B50 = 1;
    public const uint B75 = 2;
    public const uint B110 = 3;
    public const uint B134 = 4;
    public const uint B150 = 5;
    public const uint B200 = 6;
    public const uint B300 = 7;
    public const uint B600 = 8;
    public const uint B1200 = 9;
    public const uint B1800 = 10;
    public const uint B2400 = 11;
    public const uint B4800 = 12;
    public const uint B9600 = 13;
    public const uint B19200 = 14;
    public const uint B38400 = 15;
    public const uint B57600 = 4097;
    public const uint B115200 = 4098;
    public const uint B230400 = 4099;
    public const uint B460800 = 4100;
    public const uint B500000 = 4101;
    public const uint B576000 = 4102;
    public const uint B921600 = 4103;
    public const uint B1000000 = 4104;
    public const uint B1152000 = 4105;
    public const uint B1500000 = 4106;
    public const uint B2000000 = 4107;
    public const uint B2500000 = 4108;
    public const uint B3000000 = 4109;
    public const uint B3500000 = 4110;
    public const uint B4000000 = 4111;

    public const int TIOCMGET = 0x5415;
    public const int TIOCMBIS = 0x5416;
    public const int TIOCMBIC = 0x5417;
    public const int TIOCMSET = 0x5418;
    public const int FIONREAD = 0x541B;

    public const int TIOCM_DTR = 0x002;
    public const int TIOCM_RTS = 0x004;

    #endregion

    #region Public

    [DllImport(LibC, SetLastError = true)]
    public static extern int open(string pathname, int flags);

    [DllImport(LibC, SetLastError = true)]
    public static extern IntPtr read(int fd, byte[] buffer, UIntPtr count);

    [DllImport(LibC, SetLastError = true)]
    public static extern IntPtr write(int fd, byte[] buffer, UIntPtr count);

    [DllImport(LibC, SetLastError = true)]
    public static extern int close(int fd);

    [DllImport(LibC, SetLastError = true)]
    public static extern int poll(ref PollFd fds, UIntPtr nfds, int timeout);

    [DllImport(LibC, SetLastError = true)]
    public static extern int tcgetattr(int fd, ref Termios termios);

    [DllImport(LibC, SetLastError = true)]
    public static extern int tcsetattr(int fd, int optionalActions, ref Termios termios);

    [DllImport(LibC, SetLastError = true)]
    public static extern int tcflush(int fd, int queueSelector);

    [DllImport(LibC, SetLastError = true)]
    public static extern int cfsetispeed(ref Termios termios, uint speed);

    [DllImport(LibC, SetLastError = true)]
    public static extern int cfsetospeed(ref Termios termios, uint speed);

    [DllImport(LibC, SetLastError = true)]
    public static extern void cfmakeraw(ref Termios termios);

    [DllImport(LibC, SetLastError = true)]
    public static extern int ioctl(int fd, int request, ref int argp);

    public static int Errno => Marshal.GetLastWin32Error();

    public static Win32Exception LastErrnoException()
    {
        return new Win32Exception(Errno);
    }

    #endregion
}
