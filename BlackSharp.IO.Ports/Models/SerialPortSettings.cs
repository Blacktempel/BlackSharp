// SPDX-License-Identifier: MPL-2.0

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
