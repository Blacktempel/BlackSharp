// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Linux
{
    /// <summary>
    /// Imports standard libusb-1.0 functions for device enumeration and transfers on Linux.
    /// </summary>
    public static class LibUsb
    {
        #region Fields

        private const string LIBRARY_NAME = "libusb-1.0.so.0";

        #endregion

        #region Imports

        /// <summary>
        /// Reattaches a kernel driver to an interface.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_attach_kernel_driver(IntPtr handle, int interfaceNumber);

        /// <summary>
        /// Transfers data through a bulk endpoint.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_bulk_transfer(IntPtr handle, byte endpoint, [In, Out] byte[] data, int length, out int transferred, uint timeout);

        /// <summary>
        /// Claims exclusive access to an interface.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_claim_interface(IntPtr handle, int interfaceNumber);

        /// <summary>
        /// Clears an endpoint halt condition.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_clear_halt(IntPtr handle, byte endpoint);

        /// <summary>
        /// Closes an open device handle.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libusb_close(IntPtr handle);

        /// <summary>
        /// Transfers a control request using a managed byte buffer.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_control_transfer(IntPtr handle, byte requestType, byte request, ushort value, ushort index, [In, Out] byte[] data, ushort length, uint timeout);

        /// <summary>
        /// Transfers a control request using unmanaged memory.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_control_transfer(IntPtr handle, byte requestType, byte request, ushort value, ushort index, IntPtr data, ushort length, uint timeout);

        /// <summary>
        /// Detaches an active kernel driver from an interface.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_detach_kernel_driver(IntPtr handle, int interfaceNumber);

        /// <summary>
        /// Releases a libusb context.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libusb_exit(IntPtr context);

        /// <summary>
        /// Releases a device list.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void libusb_free_device_list(IntPtr list, int unrefDevices);

        /// <summary>
        /// Gets a USB bus number.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte libusb_get_bus_number(IntPtr device);

        /// <summary>
        /// Gets the active device configuration.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_get_configuration(IntPtr handle, out int configuration);

        /// <summary>
        /// Gets a USB device address.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte libusb_get_device_address(IntPtr device);

        /// <summary>
        /// Gets a USB device descriptor.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_get_device_descriptor(IntPtr device, out LibUsbDeviceDescriptor descriptor);

        /// <summary>
        /// Gets the devices visible to a context.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libusb_get_device_list(IntPtr context, out IntPtr list);

        /// <summary>
        /// Checks whether a kernel driver owns an interface.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_kernel_driver_active(IntPtr handle, int interfaceNumber);

        /// <summary>
        /// Creates a libusb context.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_init(out IntPtr context);

        /// <summary>
        /// Opens a USB device.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_open(IntPtr device, out IntPtr handle);

        /// <summary>
        /// Releases a claimed interface.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_release_interface(IntPtr handle, int interfaceNumber);

        /// <summary>
        /// Selects a device configuration.
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int libusb_set_configuration(IntPtr handle, int configuration);

        #endregion
    }
}
