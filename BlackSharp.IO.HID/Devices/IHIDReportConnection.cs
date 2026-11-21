// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.IO.HID;

/// <summary>
/// Opens report streams for a HID device.
/// </summary>
public interface IHIDReportConnection
{
    /// <summary>
    /// Gets the maximum input report length in bytes.
    /// </summary>
    int InputReportLength { get; }

    /// <summary>
    /// Gets the maximum output report length in bytes.
    /// </summary>
    int OutputReportLength { get; }

    /// <summary>
    /// Attempts to open a report stream.
    /// </summary>
    /// <param name="stream">Receives the opened stream on success or <see langword="null"/> on failure.</param>
    /// <returns><see langword="true"/> when the stream was opened; otherwise, <see langword="false"/>.</returns>
    bool TryOpenStream(out Stream stream);
}
