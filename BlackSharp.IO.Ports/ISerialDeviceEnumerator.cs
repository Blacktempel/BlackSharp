// SPDX-License-Identifier: MPL-2.0

using BlackSharp.IO.Ports.Models;

namespace BlackSharp.IO.Ports;

/// <summary>
/// Supplies descriptors for USB serial devices.
/// </summary>
public interface ISerialDeviceEnumerator
{
    /// <summary>
    /// Gets the available USB serial devices.
    /// </summary>
    /// <returns>A list of USB serial device descriptors.</returns>
    IReadOnlyList<SerialDeviceInfo> GetDevices();
}
