/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.IO.Ports.Interop.Linux;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Tests;

/// <summary>
/// Verifies the cross-platform USB device stream behavior.
/// </summary>
[TestClass]
public sealed class USBDeviceStreamTests
{
    #region Public

    /// <summary>
    /// Verifies that libusb enumeration applies the requested USB identity filter.
    /// </summary>
    [TestMethod]
    public void GetDevicesAppliesIdentityFilter()
    {
        // Arrange
        var api = new TestLibUsbDeviceApi(0x1B1C, 0x0C12, 3, 7);

        // Act
        var rejected = USBDeviceStream.GetDevices((_, _) => false, api);
        var accepted = USBDeviceStream.GetDevices(
            (vendorID, productID) => vendorID == 0x1B1C && productID == 0x0C12,
            api);

        // Assert
        Assert.AreEqual(0, rejected.Count);
        Assert.AreEqual(1, accepted.Count);
        Assert.AreEqual("usb:1B1C:0C12:003:007", accepted[0].DevicePath);
    }

    /// <summary>
    /// Verifies configured bulk and control transfers and complete native cleanup.
    /// </summary>
    [TestMethod]
    public void LibUsbStreamUsesConfiguredEndpointsAndRestoresKernelDriver()
    {
        // Arrange
        var api = new TestLibUsbDeviceApi(0x2433, 0xB200, 2, 5)
        {
            ReadData = new byte[] { 0x10, 0x20, 0x30 },
        };

        var options = new USBDeviceStreamOptions
        {
            InterfaceNumber = 2,
            ReadEndpoint     = 0x82,
            WriteEndpoint    = 0x02,
        };

        // Act
        var stream = USBDeviceStream.OpenLibUsb(
            new USBDeviceInfo(0x2433, 0xB200, 2, 5),
            options,
            api);

        Assert.IsNotNull(stream);

        var writeResult = stream.Write(new byte[] { 0xAA, 0xBB }, 100, out var bytesWritten);
        var buffer = new byte[8];
        var readResult = stream.Read(buffer, 100, out var bytesRead);
        var controlResult = stream.ControlTransfer(0x40, 0x02, 0x0001, 0, null, 100, out var controlLength);
        var clearResult = stream.ClearHalt(0x82);

        stream.Dispose();

        // Assert
        Assert.IsTrue(writeResult);
        Assert.AreEqual(2, bytesWritten);
        CollectionAssert.AreEqual(new byte[] { 0xAA, 0xBB }, api.WrittenData);
        Assert.AreEqual((byte)0x02, api.LastWriteEndpoint);

        Assert.IsTrue(readResult);
        Assert.AreEqual(3, bytesRead);
        CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x30 }, buffer.Take(bytesRead).ToArray());
        Assert.AreEqual((byte)0x82, api.LastReadEndpoint);

        Assert.IsTrue(controlResult);
        Assert.AreEqual(0, controlLength);
        Assert.AreEqual((byte)0x40, api.LastRequestType);
        Assert.AreEqual((byte)0x02, api.LastRequest);
        Assert.AreEqual((ushort)0x0001, api.LastValue);
        Assert.IsTrue(clearResult);

        Assert.AreEqual(2, api.ClaimedInterface);
        Assert.AreEqual(2, api.DetachedInterface);
        Assert.AreEqual(2, api.ReleasedInterface);
        Assert.AreEqual(2, api.AttachedInterface);
        Assert.IsTrue(api.DeviceClosed);
        Assert.IsTrue(api.ContextExited);
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Provides deterministic libusb behavior without requiring USB hardware.
    /// </summary>
    private sealed class TestLibUsbDeviceApi : ILibUsbDeviceApi
    {
        private readonly byte _busNumber;
        private readonly byte _deviceAddress;
        private readonly LibUsbDeviceDescriptor _descriptor;

        public TestLibUsbDeviceApi(
            ushort vendorID,
            ushort productID,
            byte busNumber,
            byte deviceAddress)
        {
            _descriptor = new LibUsbDeviceDescriptor
            {
                VendorID  = vendorID,
                ProductID = productID,
            };

            _busNumber     = busNumber;
            _deviceAddress = deviceAddress;
        }

        public int AttachedInterface { get; private set; } = -1;

        public int ClaimedInterface { get; private set; } = -1;

        public bool ContextExited { get; private set; }

        public int DetachedInterface { get; private set; } = -1;

        public bool DeviceClosed { get; private set; }

        public byte LastReadEndpoint { get; private set; }

        public byte LastRequest { get; private set; }

        public byte LastRequestType { get; private set; }

        public ushort LastValue { get; private set; }

        public byte LastWriteEndpoint { get; private set; }

        public byte[] ReadData { get; set; } = Array.Empty<byte>();

        public int ReleasedInterface { get; private set; } = -1;

        public byte[] WrittenData { get; private set; } = Array.Empty<byte>();

        public int AttachKernelDriver(IntPtr handle, int interfaceNumber)
        {
            AttachedInterface = interfaceNumber;

            return 0;
        }

        public int ClaimInterface(IntPtr handle, int interfaceNumber)
        {
            ClaimedInterface = interfaceNumber;

            return 0;
        }

        public int ClearHalt(IntPtr handle, byte endpoint) => 0;

        public void CloseDevice(IntPtr handle)
        {
            DeviceClosed = true;
        }

        public int DetachKernelDriver(IntPtr handle, int interfaceNumber)
        {
            DetachedInterface = interfaceNumber;

            return 0;
        }

        public void Exit(IntPtr context)
        {
            ContextExited = true;
        }

        public void FreeDeviceList(IntPtr list, int unrefDevices)
        {
            Marshal.FreeHGlobal(list);
        }

        public byte GetBusNumber(IntPtr device) => _busNumber;

        public int GetConfiguration(IntPtr handle, out int configuration)
        {
            configuration = 1;

            return 0;
        }

        public byte GetDeviceAddress(IntPtr device) => _deviceAddress;

        public int GetDeviceDescriptor(IntPtr device, out LibUsbDeviceDescriptor descriptor)
        {
            descriptor = _descriptor;

            return 0;
        }

        public IntPtr GetDeviceList(IntPtr context, out IntPtr list)
        {
            list = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(list, new IntPtr(3));

            return new IntPtr(1);
        }

        public int GetKernelDriverActive(IntPtr handle, int interfaceNumber) => 1;

        public int Initialize(out IntPtr context)
        {
            context = new IntPtr(1);

            return 0;
        }

        public int OpenDevice(IntPtr device, out IntPtr handle)
        {
            handle = new IntPtr(2);

            return 0;
        }

        public int ReleaseInterface(IntPtr handle, int interfaceNumber)
        {
            ReleasedInterface = interfaceNumber;

            return 0;
        }

        public int SetConfiguration(IntPtr handle, int configuration) => 0;

        public int TransferBulk(
            IntPtr handle,
            byte endpoint,
            byte[] data,
            int length,
            out int transferred,
            uint timeout)
        {
            if ((endpoint & 0x80) != 0)
            {
                LastReadEndpoint = endpoint;
                transferred = Math.Min(length, ReadData.Length);
                Buffer.BlockCopy(ReadData, 0, data, 0, transferred);
            }
            else
            {
                LastWriteEndpoint = endpoint;
                transferred = length;
                WrittenData = data.Take(length).ToArray();
            }

            return 0;
        }

        public int TransferControl(
            IntPtr handle,
            byte requestType,
            byte request,
            ushort value,
            ushort index,
            byte[] data,
            ushort length,
            uint timeout)
        {
            LastRequestType = requestType;
            LastRequest     = request;
            LastValue       = value;

            return length;
        }
    }

    #endregion
}
