/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Diagnostics;

namespace BlackSharp.IO.Ports;

/// <summary>
/// Provides a fault-tolerant, cross-platform stream-like adapter for a <see cref="SerialPort"/>.
/// </summary>
public sealed class SerialDeviceStream : IDisposable
{
    #region Constructor

    private SerialDeviceStream(SerialPort port)
    {
        _port = port;
    }

    #endregion

    #region Fields

    private SerialPort _port;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the underlying serial port is open.
    /// </summary>
    public bool IsOpen => _port?.IsOpen == true;

    #endregion

    #region Public

    /// <inheritdoc/>
    public void Dispose()
    {
        var port = _port;

        _port = null;

        port?.Dispose();
    }

    /// <summary>
    /// Opens and configures a serial device.
    /// </summary>
    /// <param name="path">The platform-specific serial device path.</param>
    /// <param name="baudRate">The baud rate.</param>
    /// <param name="timeoutMilliseconds">The read and write timeout in milliseconds.</param>
    /// <param name="enableRTS">Whether the Request to Send signal is enabled.</param>
    /// <param name="purgeAfterConfigure">Whether both buffers are discarded after opening.</param>
    /// <returns>The opened adapter, or <see langword="null"/> if the device could not be opened.</returns>
    public static SerialDeviceStream Open(
        string path,
        uint baudRate,
        int timeoutMilliseconds,
        bool enableRTS,
        bool purgeAfterConfigure)
    {
        if (string.IsNullOrWhiteSpace(path)
         || baudRate > int.MaxValue
         || timeoutMilliseconds <= 0)
        {
            return null;
        }

        var port = new SerialPort(path, checked((int)baudRate))
        {
            ReadTimeout  = timeoutMilliseconds,
            WriteTimeout = timeoutMilliseconds,
            RtsEnable    = enableRTS,
        };

        try
        {
            port.Open();

            if (purgeAfterConfigure)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }

            return new SerialDeviceStream(port);
        }
        catch (Exception exception) when (IsExpectedSerialException(exception))
        {
            port.Dispose();

            return null;
        }
    }

    /// <summary>
    /// Discards the input and output buffers.
    /// </summary>
    /// <returns><see langword="true"/> if both buffers were discarded; otherwise, <see langword="false"/>.</returns>
    public bool Purge()
    {
        try
        {
            if (!IsOpen)
            {
                return false;
            }

            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();

            return true;
        }
        catch (Exception exception) when (IsExpectedSerialException(exception))
        {
            return false;
        }
    }

    /// <summary>
    /// Reads until at least the requested number of bytes is available or the timeout expires.
    /// </summary>
    /// <param name="minimumLength">The minimum number of bytes required for success.</param>
    /// <param name="bufferLength">The maximum number of bytes to read.</param>
    /// <param name="timeoutMilliseconds">The total read timeout in milliseconds.</param>
    /// <param name="data">Receives the bytes that were read.</param>
    /// <returns><see langword="true"/> if at least <paramref name="minimumLength"/> bytes were read.</returns>
    public bool ReadAtLeast(
        int minimumLength,
        int bufferLength,
        int timeoutMilliseconds,
        out byte[] data)
    {
        data = null;

        if (!IsOpen
         || minimumLength <= 0
         || bufferLength < minimumLength
         || timeoutMilliseconds <= 0)
        {
            return false;
        }

        var buffer    = new byte[bufferLength];
        var total     = 0;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            while (total < minimumLength)
            {
                var elapsed = Math.Min(timeoutMilliseconds, stopwatch.ElapsedMilliseconds);
                var remainingTimeout = timeoutMilliseconds - (int)elapsed;

                if (remainingTimeout <= 0)
                {
                    break;
                }

                _port.ReadTimeout = remainingTimeout;

                var bytesRead = _port.Read(buffer, total, buffer.Length - total);

                if (bytesRead <= 0)
                {
                    break;
                }

                total += bytesRead;
            }
        }
        catch (Exception exception) when (IsExpectedSerialException(exception))
        {
            return false;
        }

        if (total < minimumLength)
        {
            return false;
        }

        if (total != buffer.Length)
        {
            Array.Resize(ref buffer, total);
        }

        data = buffer;

        return true;
    }

    /// <summary>
    /// Writes the complete byte array to the serial device.
    /// </summary>
    /// <param name="data">The bytes to write.</param>
    /// <param name="timeoutMilliseconds">The write timeout in milliseconds.</param>
    /// <param name="bytesWritten">Receives the number of bytes written.</param>
    /// <returns><see langword="true"/> if all bytes were written; otherwise, <see langword="false"/>.</returns>
    public bool Write(byte[] data, int timeoutMilliseconds, out int bytesWritten)
    {
        bytesWritten = 0;

        if (!IsOpen || data == null || timeoutMilliseconds <= 0)
        {
            return false;
        }

        try
        {
            _port.WriteTimeout = timeoutMilliseconds;
            _port.Write(data, 0, data.Length);

            bytesWritten = data.Length;

            return true;
        }
        catch (Exception exception) when (IsExpectedSerialException(exception))
        {
            return false;
        }
    }

    #endregion

    #region Private

    private static bool IsExpectedSerialException(Exception exception)
    {
        return exception is ArgumentException
            || exception is IOException
            || exception is InvalidOperationException
            || exception is PlatformNotSupportedException
            || exception is TimeoutException
            || exception is UnauthorizedAccessException;
    }

    #endregion
}
