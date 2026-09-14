/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.Ports.Tests;

/// <summary>
/// Verifies the behavior of USB device descriptors.
/// </summary>
[TestClass]
public sealed class USBDeviceInfoTests
{
    #region Public

    /// <summary>
    /// Verifies that a native Windows path selects the Windows backend.
    /// </summary>
    [TestMethod]
    public void WindowsDeviceUsesNativePath()
    {
        // Act
        var device = new USBDeviceInfo("device-path", 0x2433, 0xB200);

        // Assert
        Assert.AreEqual("device-path", device.DevicePath);
        Assert.AreEqual(0x2433, device.VendorID);
        Assert.AreEqual(0xB200, device.ProductID);
        Assert.AreEqual(USBDeviceTransportKind.WindowsDevice, device.TransportKind);
    }

    /// <summary>
    /// Verifies that libusb identities receive deterministic paths.
    /// </summary>
    [TestMethod]
    public void LibUsbDeviceUsesDeterministicPath()
    {
        // Act
        var device = new USBDeviceInfo(0x1B1C, 0x0C12, 3, 7);

        // Assert
        Assert.AreEqual("usb:1B1C:0C12:003:007", device.DevicePath);
        Assert.AreEqual((byte)3, device.BusNumber);
        Assert.AreEqual((byte)7, device.DeviceAddress);
        Assert.AreEqual(USBDeviceTransportKind.LibUsb, device.TransportKind);
    }

    #endregion
}
