/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Linux;

/// <summary>
/// Mirrors the native libusb device descriptor layout.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LibUsbDeviceDescriptor
{
    #region Fields

    /// <summary>
    /// Gets the descriptor length.
    /// </summary>
    public byte Length;
    /// <summary>
    /// Gets the descriptor type.
    /// </summary>
    public byte DescriptorType;
    /// <summary>
    /// Gets the encoded USB specification version.
    /// </summary>
    public ushort UsbSpecification;
    /// <summary>
    /// Gets the USB device class.
    /// </summary>
    public byte DeviceClass;
    /// <summary>
    /// Gets the USB device subclass.
    /// </summary>
    public byte DeviceSubClass;
    /// <summary>
    /// Gets the USB device protocol.
    /// </summary>
    public byte DeviceProtocol;
    /// <summary>
    /// Gets the maximum endpoint-zero packet size.
    /// </summary>
    public byte MaximumPacketSize;
    /// <summary>
    /// Gets the USB vendor identifier.
    /// </summary>
    public ushort VendorID;
    /// <summary>
    /// Gets the USB product identifier.
    /// </summary>
    public ushort ProductID;
    /// <summary>
    /// Gets the encoded device release number.
    /// </summary>
    public ushort DeviceRelease;
    /// <summary>
    /// Gets the manufacturer string index.
    /// </summary>
    public byte ManufacturerIndex;
    /// <summary>
    /// Gets the product string index.
    /// </summary>
    public byte ProductIndex;
    /// <summary>
    /// Gets the serial-number string index.
    /// </summary>
    public byte SerialNumberIndex;
    /// <summary>
    /// Gets the configuration count.
    /// </summary>
    public byte ConfigurationCount;

    #endregion
}
