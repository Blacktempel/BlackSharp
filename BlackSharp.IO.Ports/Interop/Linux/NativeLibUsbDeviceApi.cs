/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Diagnostics.CodeAnalysis;
using static BlackSharp.IO.Ports.Interop.Linux.LibUsb;

namespace BlackSharp.IO.Ports.Interop.Linux;

/// <summary>
/// Provides the native libusb implementation used by <see cref="USBDeviceStream"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class NativeLibUsbDeviceApi : ILibUsbDeviceApi
{
    #region Fields

    internal static readonly NativeLibUsbDeviceApi Instance = new();

    #endregion

    #region Public

    public int AttachKernelDriver(IntPtr handle, int interfaceNumber) => libusb_attach_kernel_driver(handle, interfaceNumber);

    public int ClaimInterface(IntPtr handle, int interfaceNumber) => libusb_claim_interface(handle, interfaceNumber);

    public int ClearHalt(IntPtr handle, byte endpoint) => libusb_clear_halt(handle, endpoint);

    public void CloseDevice(IntPtr handle) => libusb_close(handle);

    public int DetachKernelDriver(IntPtr handle, int interfaceNumber) => libusb_detach_kernel_driver(handle, interfaceNumber);

    public void Exit(IntPtr context) => libusb_exit(context);

    public void FreeDeviceList(IntPtr list, int unrefDevices) => libusb_free_device_list(list, unrefDevices);

    public byte GetBusNumber(IntPtr device) => libusb_get_bus_number(device);

    public int GetConfiguration(IntPtr handle, out int configuration) => libusb_get_configuration(handle, out configuration);

    public byte GetDeviceAddress(IntPtr device) => libusb_get_device_address(device);

    public int GetDeviceDescriptor(IntPtr device, out LibUsbDeviceDescriptor descriptor) => libusb_get_device_descriptor(device, out descriptor);

    public IntPtr GetDeviceList(IntPtr context, out IntPtr list) => libusb_get_device_list(context, out list);

    public int GetKernelDriverActive(IntPtr handle, int interfaceNumber) => libusb_kernel_driver_active(handle, interfaceNumber);

    public int Initialize(out IntPtr context) => libusb_init(out context);

    public int OpenDevice(IntPtr device, out IntPtr handle) => libusb_open(device, out handle);

    public int ReleaseInterface(IntPtr handle, int interfaceNumber) => libusb_release_interface(handle, interfaceNumber);

    public int SetConfiguration(IntPtr handle, int configuration) => libusb_set_configuration(handle, configuration);

    public int TransferBulk(IntPtr handle, byte endpoint, byte[] data, int length, out int transferred, uint timeout)
        => libusb_bulk_transfer(handle, endpoint, data, length, out transferred, timeout);

    public int TransferControl(IntPtr handle, byte requestType, byte request, ushort value, ushort index, byte[] data, ushort length, uint timeout)
        => libusb_control_transfer(handle, requestType, request, value, index, data, length, timeout);

    #endregion
}
