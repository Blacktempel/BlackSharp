/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID;

/// <summary>
/// Provides safe access to optional HID device metadata.
/// </summary>
public static class HIDDeviceExtensions
{
    #region Public

    /// <summary>
    /// Gets the device path, using an empty string when the device does not expose one.
    /// </summary>
    /// <param name="device">The HID device to inspect.</param>
    /// <returns>The device path.</returns>
    public static string GetDevicePathOrEmpty(this HIDDevice device)
    {
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        return device.DevicePath ?? string.Empty;
    }

    /// <summary>
    /// Gets and trims the product name, returning a fallback when it is absent or inaccessible.
    /// </summary>
    /// <param name="device">The HID device to inspect.</param>
    /// <param name="fallbackValue">The value returned when no product name is available.</param>
    /// <returns>The product name or the fallback value.</returns>
    public static string GetProductNameOrDefault(this HIDDevice device, string fallbackValue)
    {
        return GetDeviceString(device?.ProductName, fallbackValue);
    }

    /// <summary>
    /// Gets and trims the serial number, returning an empty string when it is absent or inaccessible.
    /// </summary>
    /// <param name="device">The HID device to inspect.</param>
    /// <returns>The serial number or an empty string.</returns>
    public static string GetSerialNumberOrEmpty(this HIDDevice device)
    {
        return GetDeviceString(device?.SerialNumber, string.Empty);
    }

    #endregion

    #region Private

    /// <summary>
    /// Retrieves device string.
    /// </summary>
    /// <param name="value">The value to use for get device string.</param>
    /// <param name="fallbackValue">The fallback value used by the operation.</param>
    /// <returns>The requested device string.</returns>
    static string GetDeviceString(string value, string fallbackValue)
    {
        return string.IsNullOrWhiteSpace(value) ? fallbackValue : value.Trim();
    }

    #endregion
}
