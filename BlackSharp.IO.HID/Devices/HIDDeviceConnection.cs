/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID;

/// <summary>
/// Describes a HID device connection and applies consistent stream timeouts when opening it.
/// </summary>
public sealed class HIDDeviceConnection
{
    #region Constructor

    /// <summary>
    /// Creates a connection descriptor for a HID device.
    /// </summary>
    /// <param name="device">The HID device to access.</param>
    /// <param name="ioTimeoutMilliseconds">The read and write timeout applied to opened streams.</param>
    public HIDDeviceConnection(HIDDevice device, int ioTimeoutMilliseconds)
    {
        if (ioTimeoutMilliseconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ioTimeoutMilliseconds));
        }

        Device = device ?? throw new ArgumentNullException(nameof(device));

        IOTimeoutMilliseconds = ioTimeoutMilliseconds;
        InputReportLength     = device.MaximumInputReportLength;
        OutputReportLength    = device.MaximumOutputReportLength;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the HID device represented by this connection.
    /// </summary>
    public HIDDevice Device { get; }

    /// <summary>
    /// Gets the maximum input report length reported by the device.
    /// </summary>
    public int InputReportLength { get; }

    /// <summary>
    /// Gets the configured read and write timeout in milliseconds.
    /// </summary>
    public int IOTimeoutMilliseconds { get; }

    /// <summary>
    /// Gets the maximum output report length reported by the device.
    /// </summary>
    public int OutputReportLength { get; }

    #endregion

    #region Public

    /// <summary>
    /// Attempts to open the device and applies the configured read and write timeouts.
    /// </summary>
    /// <param name="stream">Receives the opened stream when successful.</param>
    /// <returns><see langword="true"/> when the stream was opened; otherwise, <see langword="false"/>.</returns>
    public bool TryOpenStream(out HIDStream stream)
    {
        stream = null;
        try
        {
            // Apply connection-wide timeouts immediately after opening so every protocol using this
            // wrapper receives the same bounded I/O behavior.
            if (!Device.TryOpen(out stream))
            {
                return false;
            }

            stream.ReadTimeout  = IOTimeoutMilliseconds;
            stream.WriteTimeout = IOTimeoutMilliseconds;

            return true;
        }
        catch (IOException)
        {
            stream?.Dispose();

            stream = null;

            return false;
        }
        catch (UnauthorizedAccessException)
        {
            stream?.Dispose();

            stream = null;

            return false;
        }
    }

    #endregion
}
