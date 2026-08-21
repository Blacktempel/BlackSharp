/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Extensions;

namespace BlackSharp.IO.HID;

/// <summary>
/// Contains normalized metadata used when representing a HID device.
/// </summary>
public sealed class HIDDeviceMetadata
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="HIDDeviceMetadata"/> class.
    /// </summary>
    /// <param name="devicePath">The platform-specific path identifying the device.</param>
    /// <param name="name">The human-readable name associated with the target object.</param>
    /// <param name="manufacturer">The manufacturer used by the operation.</param>
    /// <param name="model">The model used by the operation.</param>
    /// <param name="serialNumber">The serial number used by the operation.</param>
    HIDDeviceMetadata(string devicePath, string name, string manufacturer, string model, string serialNumber)
    {
        DevicePath   = devicePath;
        Name         = name;
        Manufacturer = manufacturer;
        Model        = model;
        SerialNumber = serialNumber;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the platform-specific device path.
    /// </summary>
    public string DevicePath { get; }

    /// <summary>
    /// Gets the normalized manufacturer name.
    /// </summary>
    public string Manufacturer { get; }

    /// <summary>
    /// Gets the normalized model name.
    /// </summary>
    public string Model { get; }

    /// <summary>
    /// Gets the normalized display name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the optional serial number.
    /// </summary>
    public string SerialNumber { get; }

    #endregion

    #region Public

    /// <summary>
    /// Reads and normalizes metadata for a HID device.
    /// </summary>
    /// <param name="device">The HID device.</param>
    /// <param name="manufacturer">The preferred manufacturer name.</param>
    /// <param name="model">The preferred model name.</param>
    /// <param name="fallbackName">The name used when the model is absent.</param>
    /// <returns>The normalized metadata.</returns>
    public static HIDDeviceMetadata Create(HIDDevice device, string manufacturer, string model, string fallbackName)
    {
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        var normalizedModel = StringExtensions.FirstNotNullOrWhiteSpace(model?.Trim(), fallbackName);

        return new HIDDeviceMetadata(
            device.GetDevicePathOrEmpty(),
            normalizedModel,
            StringExtensions.FirstNotNullOrWhiteSpace(manufacturer?.Trim(), string.Empty),
            normalizedModel,
            device.GetSerialNumberOrEmpty());
    }

    #endregion
}
