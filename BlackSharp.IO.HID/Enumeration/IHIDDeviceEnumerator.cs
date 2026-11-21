// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.IO.HID;

/// <summary>
/// Supplies HID device descriptors.
/// </summary>
public interface IHIDDeviceEnumerator
{
    /// <summary>
    /// Gets the available HID devices.
    /// </summary>
    /// <returns>A list of HID device descriptors.</returns>
    IReadOnlyList<HIDDevice> GetDevices();
}
