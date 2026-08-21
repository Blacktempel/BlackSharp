/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID.Tests;

/// <summary>
/// Verifies the behavior of HID report descriptor info.
/// </summary>
[TestClass]
public sealed class HIDReportDescriptorInfoTests
{
    /// <summary>
    /// Verifies that unnumbered reports include synthetic report ID in lengths.
    /// </summary>
    [TestMethod]
    public void UnnumberedReportsIncludeSyntheticReportIDInLengths()
    {
        // Arrange
        var descriptor = new byte[]
        {
            0x75, 0x08,
            0x95, 0x40,
            0x81, 0x02,
            0x91, 0x02,
            0x95, 0x02,
            0xB1, 0x02,
        };

        // Act
        var info = HIDReportDescriptorInfo.Parse(descriptor);

        // Assert
        Assert.AreEqual(65, info.MaximumInputReportLength);
        Assert.AreEqual(65, info.MaximumOutputReportLength);
        Assert.AreEqual(3, info.MaximumFeatureReportLength);
        Assert.IsFalse(info.ReportsUseID);
    }

    /// <summary>
    /// Verifies that numbered reports are accumulated independently.
    /// </summary>
    [TestMethod]
    public void NumberedReportsAreAccumulatedIndependently()
    {
        // Arrange
        var descriptor = new byte[]
        {
            0x75, 0x08,
            0x85, 0x01,
            0x95, 0x08,
            0x81, 0x02,
            0x95, 0x04,
            0x81, 0x02,
            0x85, 0x02,
            0x95, 0x20,
            0x81, 0x02,
        };

        // Act
        var info = HIDReportDescriptorInfo.Parse(descriptor);

        // Assert
        Assert.AreEqual(33, info.MaximumInputReportLength);
        Assert.AreEqual(0, info.MaximumOutputReportLength);
        Assert.AreEqual(0, info.MaximumFeatureReportLength);
        Assert.IsTrue(info.ReportsUseID);
    }
}
