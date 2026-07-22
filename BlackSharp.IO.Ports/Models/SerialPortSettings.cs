/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interfaces;

namespace BlackSharp.IO.Ports.Models;

internal sealed class SerialPortSettings : ICloneable<SerialPortSettings>
{
    #region Properties

    public string PortName { get; set; } = string.Empty;
    public int BaudRate { get; set; }
    public Parity Parity { get; set; }
    public int DataBits { get; set; }
    public StopBits StopBits { get; set; }
    public Handshake Handshake { get; set; }
    public bool DtrEnable { get; set; }
    public bool RtsEnable { get; set; }
    public int ReadTimeout { get; set; }
    public int WriteTimeout { get; set; }

    #endregion

    #region Public

    public SerialPortSettings Clone()
    {
        return (SerialPortSettings)MemberwiseClone();
    }

    #endregion
}
