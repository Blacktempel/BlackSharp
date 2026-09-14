/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.Ports;

/// <summary>
/// Identifies the native transport used by a <see cref="USBDeviceStream"/>.
/// </summary>
public enum USBDeviceTransportKind
{
    /// <summary>
    /// Uses a Windows device path and the Win32 device APIs.
    /// </summary>
    WindowsDevice = 0,

    /// <summary>
    /// Uses libusb to communicate with a USB device directly.
    /// </summary>
    LibUsb = 1,
}
