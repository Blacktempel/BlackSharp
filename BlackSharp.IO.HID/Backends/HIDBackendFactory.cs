/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.IO.HID;

/// <summary>
/// Creates HID backend instances from the available platform data.
/// </summary>
internal static class HIDBackendFactory
{
    /// <summary>
    /// Processes open for the HID backend component.
    /// </summary>
    /// <param name="device">The device involved in the operation.</param>
    /// <returns>The value produced by open.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when the operation cannot be completed.</exception>
    public static IHIDTransport Open(HIDDevice device)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Backends.WindowsHIDTransport.Open(device);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Backends.LinuxHIDTransport.Open(device);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            throw new PlatformNotSupportedException("The macOS BlackSharp.IO.HID backend has not been implemented yet.");
        }

        throw new PlatformNotSupportedException("The current platform does not provide a BlackSharp.IO.HID backend.");
    }
}
