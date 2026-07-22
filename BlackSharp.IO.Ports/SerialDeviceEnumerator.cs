/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.IO.Ports.Models;
using Microsoft.Win32;
using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.IO.Ports;

#pragma warning disable CA1416

/// <summary>
/// Enumerates serial devices exposed through the Windows USB registry tree.
/// </summary>
public static class SerialDeviceEnumerator
{
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
        if (!OS.IsWindows())
        {
            return [];
        }

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

    #endregion

    #region Private

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

    #endregion
}

#pragma warning restore CA1416
