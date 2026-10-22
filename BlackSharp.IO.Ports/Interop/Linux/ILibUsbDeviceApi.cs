// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.IO.Ports.Interop.Linux;

/// <summary>
/// Defines the native libusb operations used by <see cref="USBDeviceStream"/>.
/// </summary>
internal interface ILibUsbDeviceApi
{
    int AttachKernelDriver(IntPtr handle, int interfaceNumber);

    int ClaimInterface(IntPtr handle, int interfaceNumber);

    int ClearHalt(IntPtr handle, byte endpoint);

    void CloseDevice(IntPtr handle);

    int DetachKernelDriver(IntPtr handle, int interfaceNumber);

    void Exit(IntPtr context);

    void FreeDeviceList(IntPtr list, int unrefDevices);

    byte GetBusNumber(IntPtr device);

    int GetConfiguration(IntPtr handle, out int configuration);

    byte GetDeviceAddress(IntPtr device);

    int GetDeviceDescriptor(IntPtr device, out LibUsbDeviceDescriptor descriptor);

    IntPtr GetDeviceList(IntPtr context, out IntPtr list);

    int GetKernelDriverActive(IntPtr handle, int interfaceNumber);

    int Initialize(out IntPtr context);

    int OpenDevice(IntPtr device, out IntPtr handle);

    int ReleaseInterface(IntPtr handle, int interfaceNumber);

    int SetConfiguration(IntPtr handle, int configuration);

    int TransferBulk(
        IntPtr handle,
        byte endpoint,
        byte[] data,
        int length,
        out int transferred,
        uint timeout);

    int TransferControl(
        IntPtr handle,
        byte requestType,
        byte request,
        ushort value,
        ushort index,
        byte[] data,
        ushort length,
        uint timeout);
}
