// SPDX-License-Identifier: MPL-2.0

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
