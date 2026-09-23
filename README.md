<h1 align="center">BlackSharp</h1>

[![Windows](https://img.shields.io/badge/Windows-supported-0078D4?logo=windows)](#platform-support)
[![Linux](https://img.shields.io/badge/Linux-supported-FCC624?logo=linux&logoColor=black)](#platform-support)
[![Build](https://img.shields.io/github/actions/workflow/status/Blacktempel/BlackSharp/master.yml?branch=master&label=Build&logo=githubactions&logoColor=white)](https://github.com/Blacktempel/BlackSharp/actions/workflows/master.yml)
[![.NET Framework](https://img.shields.io/badge/Framework-4.7.2%20%7C%204.8.1-512BD4?logo=dotnet&logoColor=white&labelColor=555555)](#supported-net-versions)
[![.NET](https://img.shields.io/badge/8%20%7C%209%20%7C%2010-912BD4?logo=dotnet&logoColor=white&labelColor=555555)](#supported-net-versions)
[![GitHub Sponsors](https://img.shields.io/badge/GitHub%20Sponsors-Sponsor-EA4AAA?logo=githubsponsors&logoColor=white)](https://github.com/sponsors/Blacktempel)
[![PayPal](https://img.shields.io/badge/PayPal-Donate-003087?logo=paypal&logoColor=white)](https://paypal.me/Blacktempel)
[![License](https://img.shields.io/badge/License-MPL--2.0-FF7518)](LICENSE)

**Reusable .NET building blocks for application logic, device communication and desktop interfaces.**

BlackSharp brings commonly needed functionality into a set of focused libraries:
core utilities, USB HID communication, native serial and raw USB access, MVVM
helpers and Avalonia controls. Share proven building blocks across applications
without copying infrastructure code into every project.

The libraries are general-purpose components.

## Key capabilities

- **Everyday utilities** for collections, strings, files, byte and bit
  operations, unit conversions, reflection, logging, asynchronous work & more
- **USB HID communication** with device discovery, report metadata and
  cancellable worker-based I/O on Windows and Linux
- **Native serial and raw USB access** with configurable timeouts, flow control,
  device discovery and platform-specific transports
- **MVVM foundations** built on
  [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet),
  including view-model bases, drag-and-drop contracts and shared dialog models
- **Avalonia desktop components** including dialogs, numeric text boxes, busy
  indicators, reorderable controls, behaviors, converters and shared styles
- **Modular dependencies** so utility and device-access consumers do not need to
  bring in a desktop UI framework

## Choosing a package

Install the required packages through your IDE's NuGet package manager.

Start with `BlackSharp.Core` for shared application utilities.

Add `BlackSharp.IO.HID` for HID devices or `BlackSharp.IO.Ports` for serial and raw
USB communication.

Use `BlackSharp.MVVM` for view-model infrastructure and
`BlackSharp.UI.Avalonia` for the Avalonia integration.

## Projects

| Project | NuGet package | Downloads | Purpose |
| --- | --- | --- | --- |
| [`BlackSharp.Core`](BlackSharp.Core/) | [![NuGet](https://img.shields.io/nuget/v/BlackSharp.Core?label=NuGet&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.Core/) | [![Downloads](https://img.shields.io/nuget/dt/BlackSharp.Core?label=Downloads&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.Core/) | General-purpose utilities, conversions, collections, logging and native interop helpers |
| [`BlackSharp.IO.HID`](BlackSharp.IO.HID/) | [![NuGet](https://img.shields.io/nuget/v/BlackSharp.IO.HID?label=NuGet&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.IO.HID/) | [![Downloads](https://img.shields.io/nuget/dt/BlackSharp.IO.HID?label=Downloads&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.IO.HID/) | USB HID discovery, report handling and device streams |
| [`BlackSharp.IO.Ports`](BlackSharp.IO.Ports/) | [![NuGet](https://img.shields.io/nuget/v/BlackSharp.IO.Ports?label=NuGet&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.IO.Ports/) | [![Downloads](https://img.shields.io/nuget/dt/BlackSharp.IO.Ports?label=Downloads&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.IO.Ports/) | Native serial communication and raw USB device access |
| [`BlackSharp.MVVM`](BlackSharp.MVVM/) | [![NuGet](https://img.shields.io/nuget/v/BlackSharp.MVVM?label=NuGet&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.MVVM/) | [![Downloads](https://img.shields.io/nuget/dt/BlackSharp.MVVM?label=Downloads&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.MVVM/) | View-model bases and shared MVVM contracts |
| [`BlackSharp.UI.Avalonia`](BlackSharp.UI.Avalonia/) | [![NuGet](https://img.shields.io/nuget/v/BlackSharp.UI.Avalonia?label=NuGet&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.UI.Avalonia/) | [![Downloads](https://img.shields.io/nuget/dt/BlackSharp.UI.Avalonia?label=Downloads&logo=nuget&logoColor=white)](https://www.nuget.org/packages/BlackSharp.UI.Avalonia/) | Avalonia controls, dialogs, behaviors, styles and desktop integration |

## Supported .NET versions

| Target | Core, IO.HID and IO.Ports | MVVM and UI.Avalonia |
| --- | --- | --- |
| .NET Framework 4.7.2 | ✅ | ❌ |
| .NET Framework 4.8.1 | ✅ | ❌ |
| .NET Standard 2.0 | ✅ | ❌ |
| .NET 8 | ✅ | ✅ |
| .NET 9 | ✅ | ✅ |
| .NET 10 | ✅ | ✅ |

## Quick start

### Core utilities

Reference `BlackSharp.Core` to use shared helpers without any UI or
device-transport dependencies:

```csharp
using System;
using BlackSharp.Core.Converters;
using BlackSharp.Core.Converters.Enums;

var mebibytes = DataUnitConverter.Convert(1048576m, DataUnit.Byte, DataUnit.MebiByte);

Console.WriteLine($"{mebibytes} MiB");
```

### HID discovery

Reference `BlackSharp.IO.HID` to enumerate HID interfaces on Windows or Linux:

```csharp
using System;
using BlackSharp.IO.HID;

foreach (var device in HIDDeviceEnumerator.GetDevices())
{
    Console.WriteLine($"{device.VendorID:X4}:{device.ProductID:X4} {device.ProductName}");
}
```

Open a selected interface through `HIDDevice.Open()` or `TryOpen()` when your
application is ready to communicate and dispose the returned stream when
finished. Device-specific report formats still need to be implemented by the
consumer.

## Versioning

[`Versions/BLACKSHARP.VERSION`](Versions/BLACKSHARP.VERSION) is the single
authoritative version for the five NuGet packages and the projects that produce
them. Change only this file to prepare the next version; individual project
files do not carry separate release numbers.

Use packages from the same BlackSharp release together.

## Troubleshooting and support

For device-access failures, check permissions, the installed device driver,
native dependencies and whether another application is using the device.
For UI issues, distinguish a reusable BlackSharp component from the Avalonia
backend and application-specific resource configuration.

When opening a [GitHub issue](https://github.com/Blacktempel/BlackSharp/issues),
include the affected package and version, target framework, operating system,
architecture, a minimal reproduction and the complete exception. For device
issues, include relevant device identifiers and transport settings; remove
serial numbers, credentials and other sensitive data before posting.

## Contributing

Everyone is welcome and encouraged to contribute. Bug reports, documentation,
tests, fixes and additional reusable components are all valuable. Keep changes
focused and follow the existing architecture, naming, documentation, formatting
and code style. Include or update tests and documentation where applicable,
and describe the affected packages, platforms and validation in the pull
request.

### AI-assisted contributions

Transparency about the use of artificial intelligence is required. Changes
created entirely by AI must be clearly marked as **AI-generated** in the pull
request description. Changes to which an AI tool contributed must likewise be
marked as **AI-assisted**, even when they were subsequently reviewed or edited
by the contributor. The disclosure should identify the affected parts of the
change. Contributors remain responsible for understanding, reviewing and
testing their submissions, as well as for their correctness, security and
license compliance.

_AI tools have supported the development of this project, including
documentation and unit testing._

## License

Licensed under the [Mozilla Public License 2.0](LICENSE).
