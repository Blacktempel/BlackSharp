/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID;

/// <summary>
/// Defines the contract for HID transport operations.
/// </summary>
internal interface IHIDTransport : IDisposable
{
    /// <summary>
    /// Determines whether cel.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Retrieves feature.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    void GetFeature(byte[] buffer);

    /// <summary>
    /// Processes read for the HID transport component.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    /// <returns>The value produced by read.</returns>
    int Read(byte[] buffer);

    /// <summary>
    /// Updates feature.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    void SetFeature(byte[] buffer);

    /// <summary>
    /// Processes write for the HID transport component.
    /// </summary>
    /// <param name="buffer">The buffer that receives or supplies the operation data.</param>
    /// <param name="timeoutMilliseconds">The timeout milliseconds used by the operation.</param>
    /// <returns>The value produced by write.</returns>
    int Write(byte[] buffer, int timeoutMilliseconds);
}
