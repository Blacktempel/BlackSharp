/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.IO.Ports.Interop.Linux;
using BlackSharp.IO.Ports.Interop.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.IO.Ports;

/// <summary>
/// Provides a common USB device stream backed by Win32 device I/O on Windows and libusb on Linux.
/// </summary>
public sealed class USBDeviceStream : IDisposable
{
    #region Constructor

    private USBDeviceStream(
        USBDeviceInfo deviceInfo,
        USBDeviceStreamOptions options,
        WindowsDeviceStream windowsStream)
    {
        DeviceInfo     = deviceInfo;
        _options       = options;
        _windowsStream = windowsStream;
    }

    private USBDeviceStream(
        USBDeviceInfo deviceInfo,
        USBDeviceStreamOptions options,
        ILibUsbDeviceApi libUsbApi,
        IntPtr libUsbContext,
        IntPtr libUsbHandle,
        bool detachedKernelDriver)
    {
        DeviceInfo            = deviceInfo;
        _options              = options;
        _libUsbApi            = libUsbApi;
        _libUsbContext        = libUsbContext;
        _libUsbHandle         = libUsbHandle;
        _detachedKernelDriver = detachedKernelDriver;
    }

    #endregion

    #region Fields

    private const int LibUsbErrorTimeout = -7;

    private readonly ILibUsbDeviceApi _libUsbApi;
    private readonly USBDeviceStreamOptions _options;
    private readonly WindowsDeviceStream _windowsStream;

    private bool _detachedKernelDriver;
    private IntPtr _libUsbContext;
    private IntPtr _libUsbHandle;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the descriptor of the open device.
    /// </summary>
    public USBDeviceInfo DeviceInfo { get; }

    /// <summary>
    /// Gets whether the underlying native device can still be used.
    /// </summary>
    public bool IsOpen => _windowsStream != null
        ? !_windowsStream.IsInvalid
        : _libUsbHandle != IntPtr.Zero;

    /// <summary>
    /// Gets the native transport used by this stream.
    /// </summary>
    public USBDeviceTransportKind TransportKind => DeviceInfo.TransportKind;

    #endregion

    #region Public

    /// <summary>
    /// Clears a halted libusb endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint address to clear.</param>
    /// <returns><see langword="true"/> when the endpoint was cleared; otherwise, <see langword="false"/>.</returns>
    public bool ClearHalt(byte endpoint)
    {
        return _libUsbHandle != IntPtr.Zero
            && endpoint != 0
            && _libUsbApi.ClearHalt(_libUsbHandle, endpoint) >= 0;
    }

    /// <summary>
    /// Sends a Windows device-control request.
    /// </summary>
    /// <param name="controlCode">The device-specific control code.</param>
    /// <param name="inputBuffer">The optional input data.</param>
    /// <param name="outputBuffer">The optional output buffer.</param>
    /// <param name="timeoutMilliseconds">The maximum operation duration in milliseconds.</param>
    /// <param name="bytesReturned">Receives the number of bytes returned.</param>
    /// <returns><see langword="true"/> when the request succeeds; otherwise, <see langword="false"/>.</returns>
    public bool DeviceControl(
        uint controlCode,
        byte[] inputBuffer,
        byte[] outputBuffer,
        int timeoutMilliseconds,
        out int bytesReturned)
    {
        bytesReturned = 0;

        return _windowsStream?.DeviceControl(
            controlCode,
            inputBuffer,
            outputBuffer,
            timeoutMilliseconds,
            out bytesReturned) == true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _windowsStream?.Dispose();

        var handle  = _libUsbHandle;
        var context = _libUsbContext;

        _libUsbHandle  = IntPtr.Zero;
        _libUsbContext = IntPtr.Zero;

        if (handle != IntPtr.Zero)
        {
            _libUsbApi.ReleaseInterface(handle, _options.InterfaceNumber);

            if (_detachedKernelDriver)
            {
                _libUsbApi.AttachKernelDriver(handle, _options.InterfaceNumber);
            }

            _libUsbApi.CloseDevice(handle);
        }

        if (context != IntPtr.Zero)
        {
            _libUsbApi.Exit(context);
        }

        _detachedKernelDriver = false;
    }

    /// <summary>
    /// Enumerates USB devices visible through libusb on Linux.
    /// </summary>
    /// <param name="predicate">An optional predicate selecting vendor and product identifiers.</param>
    /// <returns>The matching USB devices.</returns>
    public static IReadOnlyList<USBDeviceInfo> GetDevices(Func<int, int, bool> predicate = null)
    {
        if (!PlatformOperatingSystem.IsLinux())
        {
            return Array.Empty<USBDeviceInfo>();
        }

        return GetDevices(predicate, NativeLibUsbDeviceApi.Instance);
    }

    /// <summary>
    /// Opens a USB device through the native backend selected by its descriptor.
    /// </summary>
    /// <param name="deviceInfo">The device to open.</param>
    /// <param name="options">The native stream configuration.</param>
    /// <returns>The open stream, or <see langword="null"/> when the device could not be opened.</returns>
    public static USBDeviceStream Open(
        USBDeviceInfo deviceInfo,
        USBDeviceStreamOptions options = null)
    {
        if (deviceInfo == null)
        {
            return null;
        }

        var streamOptions = (options ?? new USBDeviceStreamOptions()).Clone();

        if (!ValidateOptions(streamOptions))
        {
            return null;
        }

        if (PlatformOperatingSystem.IsWindows()
         && deviceInfo.TransportKind == USBDeviceTransportKind.WindowsDevice)
        {
            try
            {
                var stream = WindowsDeviceStream.Open(deviceInfo.DevicePath);

                return stream == null
                     ? null
                     : new USBDeviceStream(deviceInfo, streamOptions, stream);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        if (!PlatformOperatingSystem.IsLinux()
         || deviceInfo.TransportKind != USBDeviceTransportKind.LibUsb)
        {
            return null;
        }

        return OpenLibUsb(deviceInfo, streamOptions, NativeLibUsbDeviceApi.Instance);
    }

    /// <summary>
    /// Performs a native libusb control transfer.
    /// </summary>
    /// <param name="requestType">The USB request type.</param>
    /// <param name="request">The USB request.</param>
    /// <param name="value">The request value.</param>
    /// <param name="index">The request index.</param>
    /// <param name="data">The optional transfer buffer.</param>
    /// <param name="timeoutMilliseconds">The maximum operation duration in milliseconds.</param>
    /// <param name="bytesTransferred">Receives the native transfer result.</param>
    /// <returns><see langword="true"/> when the transfer succeeds; otherwise, <see langword="false"/>.</returns>
    public bool ControlTransfer(
        byte requestType,
        byte request,
        ushort value,
        ushort index,
        byte[] data,
        int timeoutMilliseconds,
        out int bytesTransferred)
    {
        bytesTransferred = 0;

        if (_libUsbHandle == IntPtr.Zero || timeoutMilliseconds <= 0)
        {
            return false;
        }

        var length = data?.Length ?? 0;

        if (length > ushort.MaxValue)
        {
            return false;
        }

        bytesTransferred = _libUsbApi.TransferControl(
            _libUsbHandle,
            requestType,
            request,
            value,
            index,
            data,
            checked((ushort)length),
            checked((uint)timeoutMilliseconds));

        return bytesTransferred >= 0;
    }

    /// <summary>
    /// Reads data through the configured platform backend.
    /// </summary>
    /// <param name="buffer">The buffer receiving the data.</param>
    /// <param name="timeoutMilliseconds">The maximum operation duration in milliseconds.</param>
    /// <param name="bytesRead">Receives the number of bytes read.</param>
    /// <returns><see langword="true"/> when the read succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Read(byte[] buffer, int timeoutMilliseconds, out int bytesRead)
    {
        bytesRead = 0;

        if (!IsOpen || buffer == null || buffer.Length == 0 || timeoutMilliseconds <= 0)
        {
            return false;
        }

        if (_windowsStream != null)
        {
            // Use a device-control read when configured, otherwise use the standard read.
            return _options.WindowsReadControlCode.HasValue
                ? _windowsStream.DeviceControl(
                    _options.WindowsReadControlCode.Value,
                    null,
                    buffer,
                    timeoutMilliseconds,
                    out bytesRead)
                : _windowsStream.Read(buffer, timeoutMilliseconds, out bytesRead);
        }

        var status = _libUsbApi.TransferBulk(
            _libUsbHandle,
            _options.ReadEndpoint,
            buffer,
            buffer.Length,
            out bytesRead,
            checked((uint)timeoutMilliseconds));

        return status >= 0 || status == LibUsbErrorTimeout && bytesRead > 0;
    }

    /// <summary>
    /// Reads until the requested minimum was received or the shared timeout expires.
    /// </summary>
    /// <param name="buffer">The buffer receiving the data.</param>
    /// <param name="minimumLength">The minimum number of bytes required for success.</param>
    /// <param name="timeoutMilliseconds">The timeout shared by all native reads.</param>
    /// <param name="bytesRead">Receives the number of bytes read.</param>
    /// <returns><see langword="true"/> when the minimum was received; otherwise, <see langword="false"/>.</returns>
    public bool ReadAtLeast(
        byte[] buffer,
        int minimumLength,
        int timeoutMilliseconds,
        out int bytesRead)
    {
        bytesRead = 0;

        if (buffer == null
         || minimumLength <= 0
         || minimumLength > buffer.Length
         || timeoutMilliseconds <= 0)
        {
            return false;
        }

        var stopwatch = Stopwatch.StartNew();

        // Read until the minimum is reached or the timeout expires.
        while (bytesRead < minimumLength)
        {
            var elapsed = Math.Min(timeoutMilliseconds, stopwatch.ElapsedMilliseconds);
            var remainingTimeout = timeoutMilliseconds - checked((int)elapsed);

            if (remainingTimeout <= 0)
            {
                break;
            }

            var part = new byte[buffer.Length - bytesRead];

            if (!Read(part, remainingTimeout, out var partLength) || partLength <= 0)
            {
                break;
            }

            Buffer.BlockCopy(part, 0, buffer, bytesRead, partLength);
            bytesRead += partLength;
        }

        return bytesRead >= minimumLength;
    }

    /// <summary>
    /// Writes data through the configured platform backend.
    /// </summary>
    /// <param name="buffer">The data to write.</param>
    /// <param name="timeoutMilliseconds">The maximum operation duration in milliseconds.</param>
    /// <param name="bytesWritten">Receives the number of bytes written.</param>
    /// <returns><see langword="true"/> when the write succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Write(byte[] buffer, int timeoutMilliseconds, out int bytesWritten)
    {
        bytesWritten = 0;

        if (!IsOpen || buffer == null || buffer.Length == 0 || timeoutMilliseconds <= 0)
        {
            return false;
        }

        if (_windowsStream != null)
        {
            return _windowsStream.Write(buffer, timeoutMilliseconds, out bytesWritten);
        }

        return _libUsbApi.TransferBulk(
            _libUsbHandle,
            _options.WriteEndpoint,
            buffer,
            buffer.Length,
            out bytesWritten,
            checked((uint)timeoutMilliseconds)) >= 0;
    }

    #endregion

    #region Internal

    internal static IReadOnlyList<USBDeviceInfo> GetDevices(
        Func<int, int, bool> predicate,
        ILibUsbDeviceApi api)
    {
        if (api == null)
        {
            throw new ArgumentNullException(nameof(api));
        }

        var result  = new List<USBDeviceInfo>();
        var context = IntPtr.Zero;
        var list    = IntPtr.Zero;

        try
        {
            if (api.Initialize(out context) < 0)
            {
                return result;
            }

            var count = api.GetDeviceList(context, out list).ToInt64();

            if (count < 0 || list == IntPtr.Zero)
            {
                return result;
            }

            // Enumerate all devices and filter by the optional predicate.
            for (long index = 0; index < count; ++index)
            {
                var device = Marshal.ReadIntPtr(list, checked((int)(index * IntPtr.Size)));

                if (device == IntPtr.Zero
                 || api.GetDeviceDescriptor(device, out var descriptor) < 0
                 || predicate != null && !predicate(descriptor.VendorID, descriptor.ProductID))
                {
                    continue;
                }

                result.Add(new USBDeviceInfo(
                    descriptor.VendorID,
                    descriptor.ProductID,
                    api.GetBusNumber(device),
                    api.GetDeviceAddress(device)));
            }
        }
        catch (DllNotFoundException)
        {
            return result;
        }
        catch (EntryPointNotFoundException)
        {
            return result;
        }
        catch (BadImageFormatException)
        {
            return result;
        }
        finally
        {
            if (list != IntPtr.Zero)
            {
                api.FreeDeviceList(list, 1);
            }

            if (context != IntPtr.Zero)
            {
                api.Exit(context);
            }
        }

        return result;
    }

    internal static USBDeviceStream OpenLibUsb(
        USBDeviceInfo deviceInfo,
        USBDeviceStreamOptions options,
        ILibUsbDeviceApi api)
    {
        if (deviceInfo == null
         || options == null
         || api == null
         || deviceInfo.TransportKind != USBDeviceTransportKind.LibUsb
         || !ValidateOptions(options))
        {
            return null;
        }

        var context = IntPtr.Zero;
        var list    = IntPtr.Zero;
        var handle  = IntPtr.Zero;

        var claimedInterface     = false;
        var detachedKernelDriver = false;

        try
        {
            if (api.Initialize(out context) < 0)
            {
                return null;
            }

            var count = api.GetDeviceList(context, out list).ToInt64();

            if (count < 0 || list == IntPtr.Zero)
            {
                return null;
            }

            // Enumerate all devices and find the matching one.
            for (long index = 0; index < count; ++index)
            {
                var device = Marshal.ReadIntPtr(list, checked((int)(index * IntPtr.Size)));

                if (device == IntPtr.Zero
                 || api.GetBusNumber(device) != deviceInfo.BusNumber
                 || api.GetDeviceAddress(device) != deviceInfo.DeviceAddress
                 || api.GetDeviceDescriptor(device, out var descriptor) < 0
                 || descriptor.VendorID != deviceInfo.VendorID
                 || descriptor.ProductID != deviceInfo.ProductID)
                {
                    continue;
                }

                if (api.OpenDevice(device, out handle) < 0)
                {
                    return null;
                }

                break;
            }

            // Configure the device and claim the interface, detaching the kernel driver if requested.
            if (handle == IntPtr.Zero
             || api.GetConfiguration(handle, out var configuration) < 0
             || configuration == 0 && api.SetConfiguration(handle, options.Configuration) < 0)
            {
                return null;
            }

            // Detach the kernel driver if requested and active.
            if (api.GetKernelDriverActive(handle, options.InterfaceNumber) == 1)
            {
                if (!options.DetachKernelDriver
                 || api.DetachKernelDriver(handle, options.InterfaceNumber) < 0)
                {
                    return null;
                }

                detachedKernelDriver = true;
            }

            if (api.ClaimInterface(handle, options.InterfaceNumber) < 0)
            {
                return null;
            }

            claimedInterface = true;

            var result = new USBDeviceStream(
                deviceInfo,
                options.Clone(),
                api,
                context,
                handle,
                detachedKernelDriver);

            context              = IntPtr.Zero;
            handle               = IntPtr.Zero;
            claimedInterface     = false;
            detachedKernelDriver = false;

            return result;
        }
        catch (DllNotFoundException)
        {
            return null;
        }
        catch (EntryPointNotFoundException)
        {
            return null;
        }
        catch (BadImageFormatException)
        {
            return null;
        }
        finally
        {
            if (list != IntPtr.Zero)
            {
                api.FreeDeviceList(list, 1);
            }

            if (handle != IntPtr.Zero)
            {
                if (claimedInterface)
                {
                    api.ReleaseInterface(handle, options.InterfaceNumber);
                }

                if (detachedKernelDriver)
                {
                    api.AttachKernelDriver(handle, options.InterfaceNumber);
                }

                api.CloseDevice(handle);
            }

            if (context != IntPtr.Zero)
            {
                api.Exit(context);
            }
        }
    }

    #endregion

    #region Private

    private static bool ValidateOptions(USBDeviceStreamOptions options)
    {
        return options != null
            && options.Configuration > 0
            && options.InterfaceNumber >= 0
            && options.ReadEndpoint != 0
            && options.WriteEndpoint != 0;
    }

    #endregion
}
