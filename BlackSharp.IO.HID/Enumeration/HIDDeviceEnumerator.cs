// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.IO.HID;

/// <summary>
/// Enumerates the HID interfaces exposed by the operating system.
/// </summary>
public static class HIDDeviceEnumerator
{
    #region Public

    /// <summary>
    /// Returns the currently present HID interfaces.
    /// </summary>
    public static IReadOnlyList<HIDDevice> GetDevices()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Backends.WindowsHIDDeviceEnumerator.GetDevices();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Backends.LinuxHIDDeviceEnumerator.GetDevices();
        }

        throw new PlatformNotSupportedException("The current platform does not provide BlackSharp HID enumeration.");
    }

    #endregion

    #region Internal

    /// <summary>
    /// Retrieves device paths.
    /// </summary>
    /// <returns>The requested device paths entries.</returns>
    internal static IReadOnlyList<string> GetDevicePaths()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Backends.WindowsHIDDeviceEnumerator.GetDevicePaths();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Backends.LinuxHIDDeviceEnumerator.GetDevicePaths();
        }

        return Array.Empty<string>();
    }

    #endregion
}
