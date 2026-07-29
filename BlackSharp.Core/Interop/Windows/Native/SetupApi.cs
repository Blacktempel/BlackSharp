/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;
using System.Text;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected Windows Setup API functions and constants.
    /// </summary>
    public static class SetupApi
    {
        #region Fields

        const string DLL_NAME = "setupapi.dll";

        /// <summary>
        /// Includes only devices that are currently present.
        /// </summary>
        public const uint DigcfPresent         = 0x00000002;

        /// <summary>
        /// Includes device interfaces for the requested interface class.
        /// </summary>
        public const uint DigcfDeviceInterface = 0x00000010;

        /// <summary>
        /// Opens a device registry key using the global hardware profile.
        /// </summary>
        public const uint DicsFlagGlobal       = 0x00000001;

        /// <summary>
        /// Selects the device hardware registry key.
        /// </summary>
        public const uint DiregDevice          = 0x00000001;

        /// <summary>
        /// Selects the device-description registry property.
        /// </summary>
        public const uint SpdrpDeviceDescription          = 0x00000000;

        /// <summary>
        /// Selects the hardware-identifiers registry property.
        /// </summary>
        public const uint SpdrpHardwareId                 = 0x00000001;

        /// <summary>
        /// Selects the service-name registry property.
        /// </summary>
        public const uint SpdrpService                    = 0x00000004;

        /// <summary>
        /// Selects the driver registry property.
        /// </summary>
        public const uint SpdrpDriver                     = 0x00000009;

        /// <summary>
        /// Selects the manufacturer registry property.
        /// </summary>
        public const uint SpdrpManufacturer               = 0x0000000B;

        /// <summary>
        /// Selects the friendly-name registry property.
        /// </summary>
        public const uint SpdrpFriendlyName               = 0x0000000C;

        /// <summary>
        /// Selects the location-information registry property.
        /// </summary>
        public const uint SpdrpLocationInformation        = 0x0000000D;

        /// <summary>
        /// Selects the physical-device-object-name registry property.
        /// </summary>
        public const uint SpdrpPhysicalDeviceObjectName   = 0x0000000E;

        /// <summary>
        /// Selects the bus-number registry property.
        /// </summary>
        public const uint SpdrpBusNumber                  = 0x00000015;

        /// <summary>
        /// Selects the device-address registry property.
        /// </summary>
        public const uint SpdrpAddress                    = 0x0000001C;

        /// <summary>
        /// Selects the location-paths registry property.
        /// </summary>
        public const uint SpdrpLocationPaths              = 0x00000023;

        /// <summary>
        /// Identifies the display-adapter device setup class.
        /// </summary>
        public static readonly Guid DisplayClassGuid = new(
            0x4d36e968,
            0xe325,
            0x11ce,
            0xbf,
            0xc1,
            0x08,
            0x00,
            0x2b,
            0xe1,
            0x03,
            0x18);

        #endregion

        #region Imports

        /// <summary>
        /// Releases a device information set.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        /// <summary>
        /// Enumerates a device information element in a device information set.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, uint memberIndex, ref DeviceInfoData deviceInfoData);

        /// <summary>
        /// Enumerates a device interface in a device information set.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, uint memberIndex, ref DeviceInterfaceData deviceInterfaceData);

        /// <summary>
        /// Creates a device information set for a device setup class.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr parentWindow, uint flags);

        /// <summary>
        /// Creates a filtered device information set for a device setup class.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, IntPtr parentWindow, uint flags);

        /// <summary>
        /// Retrieves a device instance identifier into a string builder.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, StringBuilder deviceInstanceId, uint deviceInstanceIdSize, out uint requiredSize);

        /// <summary>
        /// Retrieves a device instance identifier into unmanaged memory.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, IntPtr deviceInstanceId, uint deviceInstanceIdSize, out uint requiredSize);

        /// <summary>
        /// Retrieves a device instance identifier into a character buffer.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, [Out] char[] deviceInstanceId, int deviceInstanceIdSize, out int requiredSize);

        /// <summary>
        /// Retrieves device-interface detail without returning device information.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref DeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, IntPtr deviceInfoData);

        /// <summary>
        /// Retrieves device-interface detail and the associated device information.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref DeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, ref DeviceInfoData deviceInfoData);

        /// <summary>
        /// Retrieves a device registry property into a byte buffer.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, uint property, out uint propertyRegistryDataType, [Out] byte[] propertyBuffer, uint propertyBufferSize, out uint requiredSize);

        /// <summary>
        /// Retrieves a device registry property into unmanaged memory.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, uint property, out uint propertyRegistryDataType, IntPtr propertyBuffer, uint propertyBufferSize, out uint requiredSize);

        /// <summary>
        /// Opens a registry key associated with a device information element.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern IntPtr SetupDiOpenDevRegKey(IntPtr deviceInfoSet, ref DeviceInfoData deviceInfoData, uint scope, uint hardwareProfile, uint keyType, uint desiredAccess);

        #endregion
    }
}
