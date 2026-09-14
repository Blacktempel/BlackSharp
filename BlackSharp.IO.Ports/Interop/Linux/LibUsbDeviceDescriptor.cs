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
internal struct LibUsbDeviceDescriptor
{
    public byte Length;
    public byte DescriptorType;
    public ushort UsbSpecification;
    public byte DeviceClass;
    public byte DeviceSubClass;
    public byte DeviceProtocol;
    public byte MaximumPacketSize;
    public ushort VendorID;
    public ushort ProductID;
    public ushort DeviceRelease;
    public byte ManufacturerIndex;
    public byte ProductIndex;
    public byte SerialNumberIndex;
    public byte ConfigurationCount;
}
