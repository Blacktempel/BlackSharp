// SPDX-License-Identifier: MPL-2.0

using OS = BlackSharp.Core.Platform.OperatingSystem;

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
    /// <returns>The HID interfaces discovered by the current platform backend.</returns>
    public static IReadOnlyList<HIDDevice> GetDevices()
    {
        if (OS.IsWindows())
        {
            return Backends.WindowsHIDDeviceEnumerator.GetDevices();
        }

        if (OS.IsLinux())
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
        if (OS.IsWindows())
        {
            return Backends.WindowsHIDDeviceEnumerator.GetDevicePaths();
        }

        if (OS.IsLinux())
        {
            return Backends.LinuxHIDDeviceEnumerator.GetDevicePaths();
        }

        return Array.Empty<string>();
    }

    #endregion
}
