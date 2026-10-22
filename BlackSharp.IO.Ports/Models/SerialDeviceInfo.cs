// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.IO.Ports.Models;

/// <summary>
/// Describes a serial device discovered by the operating system.
/// </summary>
public sealed class SerialDeviceInfo
{
    #region Properties

    /// <summary>
    /// Gets or sets the device hardware ID.
    /// </summary>
    public string HardwareID { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serial port name.
    /// </summary>
    public string PortName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the friendly device name.
    /// </summary>
    public string FriendlyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the device manufacturer.
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;

    #endregion
}
