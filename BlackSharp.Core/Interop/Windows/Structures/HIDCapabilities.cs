/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures;

/// <summary>
/// Contains the capabilities of a Windows HID top-level collection.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct HIDCapabilities
{
    /// <summary>The collection usage.</summary>
    public ushort Usage;

    /// <summary>The collection usage page.</summary>
    public ushort UsagePage;

    /// <summary>The maximum input report length.</summary>
    public ushort InputReportByteLength;

    /// <summary>The maximum output report length.</summary>
    public ushort OutputReportByteLength;

    /// <summary>The maximum feature report length.</summary>
    public ushort FeatureReportByteLength;

    /// <summary>Values reserved by the Windows HID parser.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
    public ushort[] Reserved;

    /// <summary>The number of link collection nodes.</summary>
    public ushort NumberLinkCollectionNodes;

    /// <summary>The number of input button capability structures.</summary>
    public ushort NumberInputButtonCaps;

    /// <summary>The number of input value capability structures.</summary>
    public ushort NumberInputValueCaps;

    /// <summary>The number of input data indices.</summary>
    public ushort NumberInputDataIndices;

    /// <summary>The number of output button capability structures.</summary>
    public ushort NumberOutputButtonCaps;

    /// <summary>The number of output value capability structures.</summary>
    public ushort NumberOutputValueCaps;

    /// <summary>The number of output data indices.</summary>
    public ushort NumberOutputDataIndices;

    /// <summary>The number of feature button capability structures.</summary>
    public ushort NumberFeatureButtonCaps;

    /// <summary>The number of feature value capability structures.</summary>
    public ushort NumberFeatureValueCaps;

    /// <summary>The number of feature data indices.</summary>
    public ushort NumberFeatureDataIndices;
}
