# SerialPort

Small cross-platform C# serial-port implementation for Windows and Linux.

It intentionally does **not** wrap `System.IO.Ports.SerialPort`.<br/>
The goal is to avoid the problematic managed cleanup path and to provide a `TryClose(TimeSpan timeout)` method that cannot freeze the caller forever.

## Supported

- Windows COM ports through Win32 `CreateFile(FILE_FLAG_OVERLAPPED)`, `SetCommState`, overlapped `ReadFile`/`WriteFile`, `CancelIoEx`, `CloseHandle`
- Linux serial devices through libc/POSIX `open`, `termios`, `poll`, `read`, `write`, `tcflush`, `close`
- Baud rate, data bits, parity, stop bits
- None / XOnXOff / RTS-CTS / RTS-CTS + XOnXOff handshake
- DTR/RTS control
- Read/write timeouts
- Close with caller-side timeout

## Not implemented

- `DataReceived` event
- `BaseStream`
- modem/pin change events
- macOS backend
- arbitrary Linux custom baud through `termios2`/`BOTHER`

## Important close semantics

`TryClose(timeout)` returns `false` when the native close path did not complete in time.
In that case the native close continues on the current close worker and the caller is not blocked.

After `false`, discard the `SerialPort` instance.
The OS/device may still keep the real port handle busy until the stuck close returns or the process exits.
The close worker is then abandoned and a fresh worker is created for future close requests. Pending close requests that were still waiting behind the stuck one are moved to the fresh worker. This avoids both the single-worker deadlock pattern and the unnecessary thread-per-close pattern, but a permanently blocked kernel driver can still leave one abandoned background thread behind until process exit.

For a hard guarantee that the OS handle is gone even if a USB driver blocks forever, please use process isolation around the serial access.

## Example

```
using BlackSharp.IO.Ports;

using var port = new SerialPort("COM12")
{
    BaudRate = 115200,
    DataBits = 8,
    Parity = Parity.None,
    StopBits = StopBits.One,
    Handshake = Handshake.None,
    ReadTimeout = 500,
    WriteTimeout = 500,
    CloseTimeout = TimeSpan.FromMilliseconds(500),
    DtrEnable = false,
    RtsEnable = false
};

port.Open();
port.Write(new byte[] { 0x01, 0x02, 0x03 });

var buffer = new byte[256];
int read = port.Read(buffer, 0, buffer.Length);

bool closed = port.TryClose(TimeSpan.FromMilliseconds(500));
if (!closed)
{
    // Skip this update and discard this instance.
}
```

Linux port names can be full paths (`/dev/ttyUSB0`) or short names (`ttyUSB0`).
