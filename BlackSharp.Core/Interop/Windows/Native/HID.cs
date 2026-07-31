/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native;

/// <summary>
/// Provides selected Windows HID functions and status values.
/// </summary>
public static class HID
{
    #region Fields

    private const string DLL_NAME = "hid.dll";

    /// <summary>Indicates that a HID parser operation completed successfully.</summary>
    public const int HidPStatusSuccess = 0x00110000;

    #endregion

    #region Imports

    /// <summary>Releases preparsed HID data.</summary>
    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

    /// <summary>Retrieves the HID attributes associated with a device.</summary>
    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_GetAttributes(SafeFileHandle handle, ref HIDAttributes attributes);

    /// <summary>Retrieves a HID feature report.</summary>
    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_GetFeature(SafeFileHandle device, [In, Out] byte[] reportBuffer, int reportBufferLength);

    /// <summary>Retrieves the system HID interface class identifier.</summary>
    [DllImport(DLL_NAME)]
    public static extern void HidD_GetHidGuid(out Guid hidGuid);

    /// <summary>Retrieves the HID manufacturer string.</summary>
    [DllImport(DLL_NAME, CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_GetManufacturerString(SafeFileHandle handle, char[] buffer, int bufferLength);

    /// <summary>Retrieves preparsed HID data.</summary>
    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_GetPreparsedData(SafeFileHandle handle, out IntPtr preparsedData);

    /// <summary>Retrieves the HID product string.</summary>
    [DllImport(DLL_NAME, CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_GetProductString(SafeFileHandle handle, char[] buffer, int bufferLength);

    /// <summary>Retrieves the HID serial-number string.</summary>
    [DllImport(DLL_NAME, CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_GetSerialNumberString(SafeFileHandle handle, char[] buffer, int bufferLength);

    /// <summary>Sends a HID feature report.</summary>
    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool HidD_SetFeature(SafeFileHandle device, byte[] reportBuffer, int reportBufferLength);

    /// <summary>Retrieves the top-level capabilities from preparsed HID data.</summary>
    [DllImport(DLL_NAME)]
    public static extern int HidP_GetCaps(IntPtr preparsedData, out HIDCapabilities capabilities);

    #endregion
}
