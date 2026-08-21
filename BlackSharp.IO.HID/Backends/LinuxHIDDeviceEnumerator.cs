/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.IO;
using BlackSharp.Core.Interop.Linux.Native;
using BlackSharp.Core.Utilities;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.HID.Backends;

/// <summary>
/// Discovers Linux HID device entries exposed by the system.
/// </summary>
internal static class LinuxHIDDeviceEnumerator
{
    #region Fields

    /// <summary>
    /// Defines the fixed HID raw class path value used by this component.
    /// </summary>
    private const string HIDRawClassPath = "/sys/class/hidraw";
    #endregion

    #region Public

    /// <summary>
    /// Retrieves devices.
    /// </summary>
    /// <returns>The requested devices entries.</returns>
    public static IReadOnlyList<HIDDevice> GetDevices()
    {
        var devices = new List<HIDDevice>();

        foreach (var classPath in GetClassPaths())
        {
            try
            {
                var device = TryCreateDevice(classPath);

                if (device != null)
                {
                    devices.Add(device);
                }
            }
            catch
            {
                // A single disappearing or malformed interface must not abort enumeration.
            }
        }

        return devices;
    }

    /// <summary>
    /// Retrieves device paths.
    /// </summary>
    /// <returns>The requested device paths entries.</returns>
    public static IReadOnlyList<string> GetDevicePaths()
    {
        return GetClassPaths()
            .Select(classPath => "/dev/" + Path.GetFileName(classPath))
            .ToArray();
    }

    #endregion

    #region Private

    /// <summary>
    /// Retrieves class paths.
    /// </summary>
    /// <returns>The requested class paths entries.</returns>
    private static string[] GetClassPaths()
    {
        if (!Directory.Exists(HIDRawClassPath))
        {
            return Array.Empty<string>();
        }

        return Directory.GetDirectories(HIDRawClassPath, "hidraw*")
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>
    /// Attempts to create device.
    /// </summary>
    /// <param name="classPath">The class path used by the operation.</param>
    /// <returns>The value produced by try create device.</returns>
    private static HIDDevice TryCreateDevice(string classPath)
    {
        var fileSystemPath = "/dev/" + Path.GetFileName(classPath);

        // Combine hidraw uevent properties with parent USB metadata because neither sysfs level contains the complete identity.
        if (!File.Exists(fileSystemPath))
        {
            return null;
        }

        var deviceDirectory = Path.Combine(classPath, "device");
        var properties = ReadProperties(Path.Combine(deviceDirectory, "uevent"));

        if (!TryParseHIDIdentifier(properties, out var vendorID, out var productID))
        {
            return null;
        }

        var manufacturer = string.Empty;
        var productName = properties.TryGetValue("HID_NAME", out var hidName)
            ? hidName
            : string.Empty;
        var serialNumber = properties.TryGetValue("HID_UNIQ", out var hidUnique)
            ? hidUnique
            : string.Empty;
        var releaseNumberBcd = 0;
        var interfaceNumber = TryFindInterfaceNumber(deviceDirectory);

        if (TryFindUSBParent(deviceDirectory, out var usbParent))
        {
            if (TryReadHexFile(Path.Combine(usbParent, "idVendor"), out var usbVendorID))
            {
                vendorID = usbVendorID;
            }

            if (TryReadHexFile(Path.Combine(usbParent, "idProduct"), out var usbProductID))
            {
                productID = usbProductID;
            }

            if (TryReadHexFile(Path.Combine(usbParent, "bcdDevice"), out var usbReleaseNumberBcd))
            {
                releaseNumberBcd = usbReleaseNumberBcd;
            }

            manufacturer = FileUtilities.ReadAllTextOrEmpty(Path.Combine(usbParent, "manufacturer"));

            productName  = StringUtilities.FirstNonEmpty(FileUtilities.ReadAllTextOrEmpty(Path.Combine(usbParent, "product")), productName );
            serialNumber = StringUtilities.FirstNonEmpty(FileUtilities.ReadAllTextOrEmpty(Path.Combine(usbParent, "serial" )), serialNumber);
        }

        var descriptor = ReadReportDescriptor(Path.Combine(deviceDirectory, "report_descriptor"), fileSystemPath);
        var reportInfo = HIDReportDescriptorInfo.Parse(descriptor);

        return new HIDDevice(
            GetRealPath(classPath),
            fileSystemPath,
            vendorID,
            productID,
            releaseNumberBcd,
            interfaceNumber,
            manufacturer,
            productName,
            serialNumber,
            reportInfo.MaximumInputReportLength,
            reportInfo.MaximumOutputReportLength,
            reportInfo.MaximumFeatureReportLength,
            reportInfo.ReportsUseID);
    }

    /// <summary>
    /// Retrieves real path.
    /// </summary>
    /// <param name="path">The platform path identifying the target resource.</param>
    /// <returns>The requested real path.</returns>
    private static string GetRealPath(string path)
    {
        var pointer = LibC.realpath(path, IntPtr.Zero);

        if (pointer == IntPtr.Zero)
        {
            return path;
        }

        try
        {
            return Marshal.PtrToStringAnsi(pointer) ?? path;
        }
        finally
        {
            LibC.free(pointer);
        }
    }

    /// <summary>
    /// Reads report descriptor from the underlying device or platform.
    /// </summary>
    /// <param name="sysfsPath">The sysfs path used by the operation.</param>
    /// <param name="fileSystemPath">The file system path used by the operation.</param>
    /// <returns>The report descriptor entries read from the underlying device or platform.</returns>
    /// <exception cref="IOException">Thrown when the operation cannot be completed.</exception>
    private static byte[] ReadReportDescriptor(string sysfsPath, string fileSystemPath)
    {
        try
        {
            // The sysfs descriptor is optional and can disappear during unplug; an empty result lets
            // enumeration continue with basic HID identity only.
            var descriptor = File.ReadAllBytes(sysfsPath);

            if (descriptor.Length > 0)
            {
                return descriptor;
            }
        }
        catch (IOException)
        {
            // Hot-unplug and unsupported descriptor files are expected enumeration races.
        }
        catch (UnauthorizedAccessException)
        {
            // Insufficient sysfs permissions must not hide the HID device itself.
        }

        var descriptorHandle = LibC.open(fileSystemPath, LibC.O_RDONLY | LibC.O_NONBLOCK | LibC.O_CLOEXEC);

        if (descriptorHandle < 0)
        {
            throw new IOException("Unable to open the HID interface to read its report descriptor.");
        }

        try
        {
            return HIDRaw.ReadReportDescriptor(descriptorHandle);
        }
        finally
        {
            LibC.close(descriptorHandle);
        }
    }

    /// <summary>
    /// Reads properties from the underlying device or platform.
    /// </summary>
    /// <param name="path">The platform path identifying the target resource.</param>
    /// <returns>The properties value read from the underlying device or platform.</returns>
    private static Dictionary<string, string> ReadProperties(string path)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var line in File.ReadAllLines(path))
        {
            var separator = line.IndexOf('=');

            if (separator > 0)
            {
                result[line.Substring(0, separator)] = line.Substring(separator + 1);
            }
        }

        return result;
    }

    /// <summary>
    /// Attempts to find USB parent.
    /// </summary>
    /// <param name="deviceDirectory">The device directory involved in the operation.</param>
    /// <param name="usbParent">When this method returns, contains the USB parent result.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryFindUSBParent(string deviceDirectory, out string usbParent)
    {
        var candidate = deviceDirectory;

        for (var depth = 0; depth < 10; ++depth)
        {
            if (File.Exists(Path.Combine(candidate, "idVendor"))
             && File.Exists(Path.Combine(candidate, "idProduct")))
            {
                usbParent = candidate;

                return true;
            }

            candidate = Path.Combine(candidate, "..");
        }

        usbParent = string.Empty;

        return false;
    }

    /// <summary>
    /// Attempts to find interface number.
    /// </summary>
    /// <param name="deviceDirectory">The device directory involved in the operation.</param>
    /// <returns>The value produced by try find interface number.</returns>
    private static int? TryFindInterfaceNumber(string deviceDirectory)
    {
        var candidate = deviceDirectory;

        for (var depth = 0; depth < 6; ++depth)
        {
            if (TryReadHexFile(Path.Combine(candidate, "bInterfaceNumber"), out var interfaceNumber))
            {
                return interfaceNumber;
            }

            candidate = Path.Combine(candidate, "..");
        }

        return null;
    }

    /// <summary>
    /// Attempts to parse HID identifier.
    /// </summary>
    /// <param name="properties">The properties used by the operation.</param>
    /// <param name="vendorID">When this method returns, contains the vendor ID result.</param>
    /// <param name="productID">When this method returns, contains the product ID result.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryParseHIDIdentifier(Dictionary<string, string> properties, out int vendorID, out int productID)
    {
        vendorID = 0;
        productID = 0;

        if (!properties.TryGetValue("HID_ID", out var identifier))
        {
            return false;
        }

        var segments = identifier.Split(':');

        return segments.Length == 3
            && TryParseHex(segments[1], out vendorID)
            && TryParseHex(segments[2], out productID);
    }

    /// <summary>
    /// Attempts to read hex file.
    /// </summary>
    /// <param name="path">The platform path identifying the target resource.</param>
    /// <param name="value">When this method returns, contains the value result.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryReadHexFile(string path, out int value)
    {
        return TryParseHex(FileUtilities.ReadAllTextOrEmpty(path), out value);
    }

    /// <summary>
    /// Attempts to parse hex.
    /// </summary>
    /// <param name="text">The text used by the operation.</param>
    /// <param name="value">When this method returns, contains the value result.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryParseHex(string text, out int value)
    {
        if (uint.TryParse(text, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var parsed))
        {
            value = (int)(parsed & ushort.MaxValue);

            return true;
        }

        value = 0;

        return false;
    }

    #endregion
}
