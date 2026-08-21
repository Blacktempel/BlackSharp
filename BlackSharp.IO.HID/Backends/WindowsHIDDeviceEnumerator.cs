/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows;
using BlackSharp.Core.Interop.Windows.Enums;
using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using NativeHID = BlackSharp.Core.Interop.Windows.Native.HID;

namespace BlackSharp.IO.HID.Backends;

/// <summary>
/// Discovers Windows HID device entries exposed by the system.
/// </summary>
internal static class WindowsHIDDeviceEnumerator
{
    #region Public

    /// <summary>
    /// Retrieves devices.
    /// </summary>
    /// <returns>The requested devices entries.</returns>
    public static IReadOnlyList<HIDDevice> GetDevices()
    {
        var devices = new List<HIDDevice>();

        foreach (var path in GetDevicePaths())
        {
            try
            {
                var device = TryCreateDevice(path);

                if (device != null)
                {
                    devices.Add(device);
                }
            }
            catch
            {
                // A single disappearing or inaccessible interface must not abort enumeration.
            }
        }

        return devices;
    }

    /// <summary>
    /// Retrieves device paths.
    /// </summary>
    /// <returns>The requested device paths entries.</returns>
    /// <exception cref="Win32Exception">Thrown when the operation cannot be completed.</exception>
    public static IReadOnlyList<string> GetDevicePaths()
    {
        var result = new List<string>();

        // Enumerate present HID interfaces and retrieve each variable-length device path through SetupAPI's two-call pattern.
        NativeHID.HidD_GetHidGuid(out var hidGuid);

        var deviceInfoSet = SetupApi.SetupDiGetClassDevs(
            ref hidGuid,
            IntPtr.Zero,
            IntPtr.Zero,
            SetupApi.DigcfPresent | SetupApi.DigcfDeviceInterface);

        if (deviceInfoSet == IntPtr.Zero || deviceInfoSet == Kernel32.InvalidHandle)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            for (uint index = 0; ; ++index)
            {
                var interfaceData = new DeviceInterfaceData
                {
                    Size = Marshal.SizeOf(typeof(DeviceInterfaceData)),
                };

                if (!SetupApi.SetupDiEnumDeviceInterfaces(
                    deviceInfoSet,
                    IntPtr.Zero,
                    ref hidGuid,
                    index,
                    ref interfaceData))
                {
                    var errorCode = Marshal.GetLastWin32Error();

                    if (errorCode == Win32ErrorCodes.NoMoreItems)
                    {
                        break;
                    }

                    throw new Win32Exception(errorCode);
                }

                if (TryGetDevicePath(deviceInfoSet, ref interfaceData, out var path))
                {
                    result.Add(path);
                }
            }
        }
        finally
        {
            SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
        }

        return result;
    }

    #endregion

    #region Private

    /// <summary>
    /// Attempts to create device.
    /// </summary>
    /// <param name="path">The platform path identifying the target resource.</param>
    /// <returns>The value produced by try create device.</returns>
    private static HIDDevice TryCreateDevice(string path)
    {
        // Open the interface long enough to collect capabilities and strings, discarding entries that disappear during enumeration.
        using var handle = Kernel32.CreateFileS(
            path,
            DesiredAccess.None,
            FileShareMode.ReadWrite,
            IntPtr.Zero,
            FileCreationDisposition.OpenExisting,
            FileFlagsAndAttributes.None,
            IntPtr.Zero);

        if (handle.IsInvalid)
        {
            return null;
        }

        var attributes = new HIDAttributes
        {
            Size = Marshal.SizeOf(typeof(HIDAttributes)),
        };

        if (!NativeHID.HidD_GetAttributes(handle, ref attributes)
         || !NativeHID.HidD_GetPreparsedData(handle, out var preparsedData))
        {
            return null;
        }

        HIDCapabilities capabilities;

        try
        {
            if (NativeHID.HidP_GetCaps(preparsedData, out capabilities) != NativeHID.HidPStatusSuccess)
            {
                return null;
            }
        }
        finally
        {
            NativeHID.HidD_FreePreparsedData(preparsedData);
        }

        return new HIDDevice(
            path,
            path,
            attributes.VendorID,
            attributes.ProductID,
            attributes.VersionNumber,
            GetInterfaceNumber(path),
            GetString(handle, NativeHID.HidD_GetManufacturerString),
            GetString(handle, NativeHID.HidD_GetProductString),
            GetString(handle, NativeHID.HidD_GetSerialNumberString),
            capabilities.InputReportByteLength,
            capabilities.OutputReportByteLength,
            capabilities.FeatureReportByteLength,
            false);
    }

    /// <summary>
    /// Retrieves string.
    /// </summary>
    /// <param name="handle">The native handle used by the operation.</param>
    /// <param name="getter">The getter used by the operation.</param>
    /// <returns>The requested string.</returns>
    private static string GetString(SafeFileHandle handle, GetHIDString getter)
    {
        var buffer = new char[256];

        return getter(handle, buffer, buffer.Length * sizeof(char))
            ? new string(buffer).TrimEnd('\0').Trim()
            : string.Empty;
    }

    /// <summary>
    /// Retrieves interface number.
    /// </summary>
    /// <param name="path">The platform path identifying the target resource.</param>
    /// <returns>The requested interface number.</returns>
    private static int? GetInterfaceNumber(string path)
    {
        const string marker = "&mi_";

        var markerIndex = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (markerIndex < 0 || markerIndex + marker.Length + 2 > path.Length)
        {
            return null;
        }

        return int.TryParse(
            path.Substring(markerIndex + marker.Length, 2),
            NumberStyles.AllowHexSpecifier,
            CultureInfo.InvariantCulture,
            out var interfaceNumber)
            ? interfaceNumber
            : (int?)null;
    }

    /// <summary>
    /// Attempts to get device path.
    /// </summary>
    /// <param name="deviceInfoSet">The device info set involved in the operation.</param>
    /// <param name="interfaceData">The interface data used by the operation.</param>
    /// <param name="path">When this method returns, contains the path result.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetDevicePath(IntPtr deviceInfoSet, ref DeviceInterfaceData interfaceData, out string path)
    {
        path = string.Empty;

        // SetupAPI uses a size-query call followed by a caller-allocated detail buffer.
        SetupApi.SetupDiGetDeviceInterfaceDetail(
            deviceInfoSet,
            ref interfaceData,
            IntPtr.Zero,
            0,
            out var requiredSize,
            IntPtr.Zero);

        if (requiredSize == 0)
        {
            return false;
        }

        var detailData = Marshal.AllocHGlobal(checked((int)requiredSize));

        try
        {
            // SP_DEVICE_INTERFACE_DETAIL_DATA has architecture-specific packing mandated by SetupAPI.
            Marshal.WriteInt32(detailData, IntPtr.Size == 8 ? 8 : 6);

            if (!SetupApi.SetupDiGetDeviceInterfaceDetail(
                deviceInfoSet,
                ref interfaceData,
                detailData,
                requiredSize,
                out _,
                IntPtr.Zero))
            {
                return false;
            }

            // The UTF-16 path starts immediately after cbSize even though the x64 structure is padded.
            path = Marshal.PtrToStringUni(IntPtr.Add(detailData, 4)) ?? string.Empty;

            return path.Length > 0;
        }
        finally
        {
            Marshal.FreeHGlobal(detailData);
        }
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Represents the callback used to process get HID string.
    /// </summary>
    /// <param name="handle">The native handle used by the operation.</param>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    /// <param name="bufferLength">The buffer length used by the operation.</param>
    /// <returns>
    /// <see langword="true"/> when the requested condition is satisfied; otherwise, <see langword="false"/>.
    /// </returns>
    private delegate bool GetHIDString(SafeFileHandle handle, char[] buffer, int bufferLength);

    #endregion
}
