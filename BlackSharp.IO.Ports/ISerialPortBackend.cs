/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.Ports;

internal interface ISerialPortBackend
{
    bool IsOpen { get; }

    void Open(SerialPortSettings settings);

    int Read(byte[] buffer, int offset, int count);

    void Write(byte[] buffer, int offset, int count);

    int BytesToRead { get; }

    void DiscardInBuffer();

    void DiscardOutBuffer();

    void SetDtr(bool enabled);

    void SetRts(bool enabled);

    void RequestAbort();

    void CloseCore();
}
