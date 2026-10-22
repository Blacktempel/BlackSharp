// SPDX-License-Identifier: MPL-2.0

using BlackSharp.IO.Ports.Models;

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
