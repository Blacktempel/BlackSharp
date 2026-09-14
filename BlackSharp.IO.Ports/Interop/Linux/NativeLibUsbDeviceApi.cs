/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Linux;

/// <summary>
/// Provides the native libusb implementation used by <see cref="USBDeviceStream"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class NativeLibUsbDeviceApi : ILibUsbDeviceApi
{
    #region Fields

    private const string LibraryName = "libusb-1.0.so.0";

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

    #region Imports

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_attach_kernel_driver(IntPtr handle, int interfaceNumber);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_bulk_transfer(
        IntPtr handle,
        byte endpoint,
        [In, Out] byte[] data,
        int length,
        out int transferred,
        uint timeout);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_claim_interface(IntPtr handle, int interfaceNumber);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_clear_halt(IntPtr handle, byte endpoint);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void libusb_close(IntPtr handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_control_transfer(
        IntPtr handle,
        byte requestType,
        byte request,
        ushort value,
        ushort index,
        [In, Out] byte[] data,
        ushort length,
        uint timeout);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_detach_kernel_driver(IntPtr handle, int interfaceNumber);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void libusb_exit(IntPtr context);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void libusb_free_device_list(IntPtr list, int unrefDevices);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern byte libusb_get_bus_number(IntPtr device);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_get_configuration(IntPtr handle, out int configuration);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern byte libusb_get_device_address(IntPtr device);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_get_device_descriptor(IntPtr device, out LibUsbDeviceDescriptor descriptor);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr libusb_get_device_list(IntPtr context, out IntPtr list);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_kernel_driver_active(IntPtr handle, int interfaceNumber);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_init(out IntPtr context);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_open(IntPtr device, out IntPtr handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_release_interface(IntPtr handle, int interfaceNumber);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int libusb_set_configuration(IntPtr handle, int configuration);

    #endregion
}
