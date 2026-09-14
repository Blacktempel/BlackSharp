# BlackSharp.IO.Ports

Cross-platform serial-port and raw USB-device access for Windows and Linux.

The serial-port API intentionally does **not** wrap `System.IO.Ports.SerialPort`.

## Supported

- Windows COM ports through Win32 `CreateFile(FILE_FLAG_OVERLAPPED)`, `SetCommState`, overlapped `ReadFile`/`WriteFile`, `CancelIoEx`, `CloseHandle`
- Linux serial devices through libc/POSIX `open`, `termios`, `poll`, `read`, `write`, `tcflush`, `close`
- USB serial device discovery through the Windows registry and Linux sysfs
- Baud rate, data bits, parity, stop bits
- None / XOnXOff / RTS-CTS / RTS-CTS + XOnXOff handshake
- DTR/RTS control
- Read/write timeouts
- Close with caller-side timeout
- Raw USB access through Win32 device handles and libusb
- Filtered Linux USB-device discovery by vendor and product ID
- Configurable libusb interface, bulk endpoints and kernel-driver detachment
- Cross-platform `USBDeviceStream` read/write API
- libusb control transfers and endpoint halt recovery
- Optional Windows read IOCTL mapping for driver-backed USB devices

## Not implemented

- `DataReceived` event
- `BaseStream`
- modem/pin change events
- macOS serial backend
- macOS raw USB backend
- arbitrary Linux custom baud through `termios2`/`BOTHER`

## Important close semantics

`TryClose(timeout)` returns `false` when the native close path did not complete in time.
In that case the native close continues on the current close worker and the caller is not blocked.

After `false`, discard the `SerialPort` instance.
The OS/device may still keep the real port handle busy until the stuck close returns or the process exits.
The close worker is then abandoned and a fresh worker is created for future close requests. Pending close requests that were still waiting behind the stuck one are moved to the fresh worker. This avoids both the single-worker deadlock pattern and the unnecessary thread-per-close pattern, but a permanently blocked kernel driver can still leave one abandoned background thread behind until process exit.

For a hard guarantee that the OS handle is gone even if a USB driver blocks forever, please use process isolation around the serial access.

## Serial example

```csharp
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
    RtsEnable = false,
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

## Linux raw USB example

```csharp
var devices = USBDeviceStream.GetDevices((vendorID, productID) => vendorID == 0x1234 && productID == 0x5678);

var options = new USBDeviceStreamOptions
{
    InterfaceNumber = 0,
    ReadEndpoint = 0x81,
    WriteEndpoint = 0x01,
};

using var stream = USBDeviceStream.Open(devices.FirstOrDefault(), options);

if (stream != null)
{
    stream.Write(new byte[] { 0x01 }, 500, out _);

    var response = new byte[64];
    stream.Read(response, 500, out var bytesRead);
}
```

Linux raw USB access requires the `libusb-1.0.so.0` shared library and operating-system permissions for the target device.
On Windows, construct `USBDeviceInfo` with the native device path supplied by the device-specific enumerator. The same `USBDeviceStream` read/write API is then used; driver-backed devices can map reads to an IOCTL through `WindowsReadControlCode`.
