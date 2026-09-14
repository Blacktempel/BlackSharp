/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Globalization;

namespace BlackSharp.IO.Ports;

/// <summary>
/// Identifies a USB device that can be opened through <see cref="USBDeviceStream"/>.
/// </summary>
public sealed class USBDeviceInfo
{
    #region Constructor

    /// <summary>
    /// Initializes a Windows USB device descriptor.
    /// </summary>
    /// <param name="devicePath">The native Windows device path.</param>
    /// <param name="vendorID">The USB vendor identifier.</param>
    /// <param name="productID">The USB product identifier.</param>
    public USBDeviceInfo(string devicePath, int vendorID, int productID)
        : this(
            devicePath,
            vendorID,
            productID,
            USBDeviceTransportKind.WindowsDevice,
            0,
            0)
    {
    }

    /// <summary>
    /// Initializes a libusb device descriptor.
    /// </summary>
    /// <param name="vendorID">The USB vendor identifier.</param>
    /// <param name="productID">The USB product identifier.</param>
    /// <param name="busNumber">The USB bus number.</param>
    /// <param name="deviceAddress">The USB device address.</param>
    public USBDeviceInfo(int vendorID, int productID, byte busNumber, byte deviceAddress)
        : this(
            string.Format(
                CultureInfo.InvariantCulture,
                "usb:{0:X4}:{1:X4}:{2:D3}:{3:D3}",
                vendorID,
                productID,
                busNumber,
                deviceAddress),
            vendorID,
            productID,
            USBDeviceTransportKind.LibUsb,
            busNumber,
            deviceAddress)
    {
    }

    /// <summary>
    /// Initializes a USB device descriptor.
    /// </summary>
    private USBDeviceInfo(
        string devicePath,
        int vendorID,
        int productID,
        USBDeviceTransportKind transportKind,
        byte busNumber,
        byte deviceAddress)
    {
        if (string.IsNullOrWhiteSpace(devicePath))
        {
            throw new ArgumentException("A device path is required.", nameof(devicePath));
        }

        if (vendorID < 0 || vendorID > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(vendorID));
        }

        if (productID < 0 || productID > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(productID));
        }

        DevicePath    = devicePath;
        VendorID      = vendorID;
        ProductID     = productID;
        TransportKind = transportKind;
        BusNumber     = busNumber;
        DeviceAddress = deviceAddress;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the USB bus number used by libusb.
    /// </summary>
    public byte BusNumber { get; }

    /// <summary>
    /// Gets the USB device address used by libusb.
    /// </summary>
    public byte DeviceAddress { get; }

    /// <summary>
    /// Gets the stable path used to identify the device within the current platform.
    /// </summary>
    public string DevicePath { get; }

    /// <summary>
    /// Gets the USB product identifier.
    /// </summary>
    public int ProductID { get; }

    /// <summary>
    /// Gets the native transport used to access this device.
    /// </summary>
    public USBDeviceTransportKind TransportKind { get; }

    /// <summary>
    /// Gets the USB vendor identifier.
    /// </summary>
    public int VendorID { get; }

    #endregion
}
