/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.Ports;

/// <summary>
/// Configures the native backend used by <see cref="USBDeviceStream"/>.
/// </summary>
public sealed class USBDeviceStreamOptions
{
    #region Properties

    /// <summary>
    /// Gets or sets the configuration selected when a libusb device is not configured yet.
    /// </summary>
    public int Configuration { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether an active Linux kernel driver may be detached while the stream is open.
    /// </summary>
    public bool DetachKernelDriver { get; set; } = true;

    /// <summary>
    /// Gets or sets the interface claimed by the libusb backend.
    /// </summary>
    public int InterfaceNumber { get; set; }

    /// <summary>
    /// Gets or sets the bulk input endpoint used by <see cref="USBDeviceStream.Read"/>.
    /// </summary>
    public byte ReadEndpoint { get; set; } = 0x81;

    /// <summary>
    /// Gets or sets the bulk output endpoint used by <see cref="USBDeviceStream.Write"/>.
    /// </summary>
    public byte WriteEndpoint { get; set; } = 0x01;

    /// <summary>
    /// Gets or sets the optional device-control code used by <see cref="USBDeviceStream.Read"/> on Windows.
    /// </summary>
    public uint? WindowsReadControlCode { get; set; }

    #endregion

    #region Internal

    /// <summary>
    /// Creates an isolated copy used for the lifetime of an open stream.
    /// </summary>
    internal USBDeviceStreamOptions Clone()
    {
        return new USBDeviceStreamOptions
        {
            Configuration          = Configuration,
            DetachKernelDriver     = DetachKernelDriver,
            InterfaceNumber        = InterfaceNumber,
            ReadEndpoint           = ReadEndpoint,
            WriteEndpoint          = WriteEndpoint,
            WindowsReadControlCode = WindowsReadControlCode,
        };
    }

    #endregion
}
