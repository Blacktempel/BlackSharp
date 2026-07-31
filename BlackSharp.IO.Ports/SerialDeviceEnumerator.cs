/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Extensions;
using BlackSharp.IO.Ports.Models;
using Microsoft.Win32;
using System.Globalization;
using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.IO.Ports;

#pragma warning disable CA1416

/// <summary>
/// Enumerates USB serial devices exposed by the operating system.
/// </summary>
public static class SerialDeviceEnumerator
{
    #region Fields

    private const string LinuxTTYClassPath = "/sys/class/tty";
    private const string LinuxDevicePath = "/dev";
    private const int MaximumLinuxParentDepth = 10;

    #endregion

    #region Public

    /// <summary>
    /// Gets the available USB serial devices that satisfy an optional hardware ID filter.
    /// </summary>
    /// <param name="hardwareIDFilter">
    /// An optional predicate used to select hardware IDs. A <see langword="null"/> predicate selects all IDs.
    /// </param>
    /// <returns>The discovered serial devices.</returns>
    public static IReadOnlyList<SerialDeviceInfo> GetDevices(
        Func<string, bool> hardwareIDFilter = null)
    {
        if (OS.IsWindows())
        {
            return GetWindowsDevices(hardwareIDFilter);
        }

        if (OS.IsLinux())
        {
            return GetLinuxDevices(
                LinuxTTYClassPath,
                LinuxDevicePath,
                hardwareIDFilter);
        }

        return [];
    }

    #endregion

    #region Private

    private static IReadOnlyList<SerialDeviceInfo> GetWindowsDevices(
        Func<string, bool> hardwareIDFilter)
    {
        var result = new List<SerialDeviceInfo>();

        try
        {
            using RegistryKey usbKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USB");

            if (usbKey == null)
            {
                return result;
            }

            foreach (var hardwareID in usbKey.GetSubKeyNames())
            {
                if (hardwareIDFilter != null && !hardwareIDFilter(hardwareID))
                {
                    continue;
                }

                using RegistryKey hardwareKey = usbKey.OpenSubKey(hardwareID);

                if (hardwareKey == null)
                {
                    continue;
                }

                foreach (var instanceName in hardwareKey.GetSubKeyNames())
                {
                    using RegistryKey instanceKey = hardwareKey.OpenSubKey(instanceName);
                    using RegistryKey parametersKey = instanceKey?.OpenSubKey("Device Parameters");

                    var portName = parametersKey?.GetValue("PortName") as string;

                    if (string.IsNullOrWhiteSpace(portName))
                    {
                        continue;
                    }

                    result.Add(new SerialDeviceInfo
                    {
                        HardwareID   = hardwareID,
                        PortName     = portName,
                        FriendlyName = ReadRegistryString(instanceKey, "FriendlyName"),
                        Manufacturer = ReadRegistryString(instanceKey, "Mfg"),
                    });
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Inaccessible serial devices are skipped while the rest remain discoverable.
        }

        return result;
    }

    private static IReadOnlyList<SerialDeviceInfo> GetLinuxDevices(
        string ttyClassPath,
        string devicePath,
        Func<string, bool> hardwareIDFilter = null)
    {
        if (string.IsNullOrWhiteSpace(ttyClassPath))
        {
            throw new ArgumentException("A Linux TTY class path is required.", nameof(ttyClassPath));
        }

        if (string.IsNullOrWhiteSpace(devicePath))
        {
            throw new ArgumentException("A Linux device path is required.", nameof(devicePath));
        }

        var result = new List<SerialDeviceInfo>();

        if (!Directory.Exists(ttyClassPath))
        {
            return result;
        }

        string[] ttyClassDevices;

        try
        {
            ttyClassDevices = Directory.GetDirectories(ttyClassPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
        }
        catch (IOException)
        {
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            return result;
        }

        foreach (var ttyClassDevice in ttyClassDevices)
        {
            try
            {
                var ttyName = Path.GetFileName(ttyClassDevice);
                var portName = Path.Combine(devicePath, ttyName);

                if (string.IsNullOrWhiteSpace(ttyName) || !File.Exists(portName)
                 || !TryFindLinuxUSBParent(
                        Path.Combine(ttyClassDevice, "device"),
                        out var usbParent)
                 || !TryReadHexFile(Path.Combine(usbParent, "idVendor"), out var vendorID)
                 || !TryReadHexFile(Path.Combine(usbParent, "idProduct"), out var productID))
                {
                    continue;
                }

                var hardwareID = $"VID_{vendorID:X4}&PID_{productID:X4}";

                if (hardwareIDFilter != null && !hardwareIDFilter(hardwareID))
                {
                    continue;
                }

                result.Add(new SerialDeviceInfo
                {
                    HardwareID   = hardwareID,
                    PortName     = portName,
                    FriendlyName = StringExtensions.FirstNotNullOrWhiteSpace(
                        ReadTextFile(Path.Combine(usbParent, "product")),
                        ttyName),
                    Manufacturer = ReadTextFile(Path.Combine(usbParent, "manufacturer")),
                });
            }
            catch (IOException)
            {
                // A disappearing or malformed TTY must not abort enumeration.
            }
            catch (UnauthorizedAccessException)
            {
                // Inaccessible TTY metadata is skipped while the rest remains discoverable.
            }
        }

        return result;
    }

    private static string ReadRegistryString(RegistryKey key, string name)
    {
        object value = key?.GetValue(name);

        if (value is string text)
        {
            int separator = text.LastIndexOf(';');

            return separator >= 0 ? text.Substring(separator + 1) : text;
        }

        return string.Empty;
    }

    private static string ReadTextFile(string path)
    {
        try
        {
            return File.Exists(path) ? File.ReadAllText(path).Trim() : string.Empty;
        }
        catch (IOException)
        {
            return string.Empty;
        }
        catch (UnauthorizedAccessException)
        {
            return string.Empty;
        }
    }

    private static bool TryFindLinuxUSBParent(string devicePath, out string usbParent)
    {
        var candidate = devicePath;

        for (int depth = 0; depth < MaximumLinuxParentDepth; ++depth)
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

    private static bool TryReadHexFile(string path, out int value)
    {
        if (uint.TryParse(
            ReadTextFile(path),
            NumberStyles.AllowHexSpecifier,
            CultureInfo.InvariantCulture,
            out var parsed))
        {
            value = (int)(parsed & ushort.MaxValue);

            return true;
        }

        value = 0;

        return false;
    }

    #endregion
}

#pragma warning restore CA1416
