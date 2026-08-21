/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID;

/// <summary>
/// Describes an operating-system HID interface that can be opened for raw report I/O.
/// </summary>
public sealed class HIDDevice
{
    #region Constructor

    /// <summary>
    /// Creates a HID device descriptor.
    /// </summary>
    /// <param name="devicePath">The platform-specific HID interface path.</param>
    /// <param name="maximumInputReportLength">The maximum input report length including any report ID.</param>
    /// <param name="maximumOutputReportLength">The maximum output report length including any report ID.</param>
    /// <param name="maximumFeatureReportLength">The maximum feature report length including any report ID.</param>
    public HIDDevice(
        string devicePath,
        int maximumInputReportLength,
        int maximumOutputReportLength,
        int maximumFeatureReportLength = 0)
        : this(
            devicePath,
            devicePath,
            0,
            0,
            0,
            null,
            string.Empty,
            string.Empty,
            string.Empty,
            maximumInputReportLength,
            maximumOutputReportLength,
            maximumFeatureReportLength,
            false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HIDDevice"/> class.
    /// </summary>
    /// <param name="devicePath">The platform-specific path identifying the device.</param>
    /// <param name="fileSystemPath">The file system path used by the operation.</param>
    /// <param name="vendorID">The vendor identifier.</param>
    /// <param name="productID">The product identifier.</param>
    /// <param name="releaseNumberBcd">The release number bcd used by the operation.</param>
    /// <param name="interfaceNumber">The interface number used by the operation.</param>
    /// <param name="manufacturer">The manufacturer used by the operation.</param>
    /// <param name="productName">The product name used by the operation.</param>
    /// <param name="serialNumber">The serial number used by the operation.</param>
    /// <param name="maximumInputReportLength">The maximum input report length used by the operation.</param>
    /// <param name="maximumOutputReportLength">The maximum output report length used by the operation.</param>
    /// <param name="maximumFeatureReportLength">The maximum feature report length used by the operation.</param>
    /// <param name="reportsUseID">The reports use identifier.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="devicePath"/> is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="vendorID"/> is outside the supported range.
    /// </exception>
    internal HIDDevice(
        string devicePath,
        string fileSystemPath,
        int vendorID,
        int productID,
        int releaseNumberBcd,
        int? interfaceNumber,
        string manufacturer,
        string productName,
        string serialNumber,
        int maximumInputReportLength,
        int maximumOutputReportLength,
        int maximumFeatureReportLength,
        bool reportsUseID)
    {
        // Device descriptors cross an operating-system boundary and may originate from incomplete enumeration data.
        // Reject invalid widths here so transport implementations can rely on a safe, normalized descriptor.
        if (string.IsNullOrWhiteSpace(devicePath))
        {
            throw new ArgumentException("A HID device path is required.", nameof(devicePath));
        }

        if (string.IsNullOrWhiteSpace(fileSystemPath))
        {
            throw new ArgumentException("A HID file-system path is required.", nameof(fileSystemPath));
        }

        if (vendorID < 0 || vendorID > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(vendorID));
        }

        if (productID < 0 || productID > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(productID));
        }

        if (releaseNumberBcd < 0 || releaseNumberBcd > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(releaseNumberBcd));
        }

        if (interfaceNumber < 0 || interfaceNumber > byte.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(interfaceNumber));
        }

        if (maximumInputReportLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumInputReportLength));
        }

        if (maximumOutputReportLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumOutputReportLength));
        }

        if (maximumFeatureReportLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumFeatureReportLength));
        }

        if (maximumInputReportLength == 0
         && maximumOutputReportLength == 0
         && maximumFeatureReportLength == 0)
        {
            throw new ArgumentException("The HID device does not expose any reports.");
        }

        // String descriptors commonly contain padding supplied by firmware or the operating system. Store trimmed,
        // non-null values so identity comparisons do not need to repeat that cleanup.
        DevicePath                 = devicePath;
        FileSystemPath             = fileSystemPath;
        VendorID                   = vendorID;
        ProductID                  = productID;
        ReleaseNumberBcd           = releaseNumberBcd;
        InterfaceNumber            = interfaceNumber;
        Manufacturer               = manufacturer?.Trim() ?? string.Empty;
        ProductName                = productName?.Trim() ?? string.Empty;
        SerialNumber               = serialNumber?.Trim() ?? string.Empty;
        MaximumInputReportLength   = maximumInputReportLength;
        MaximumOutputReportLength  = maximumOutputReportLength;
        MaximumFeatureReportLength = maximumFeatureReportLength;
        ReportsUseID               = reportsUseID;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the platform-specific HID interface path.
    /// </summary>
    public string DevicePath { get; }

    /// <summary>
    /// Gets the operating-system path used to open the HID interface.
    /// </summary>
    public string FileSystemPath { get; }

    /// <summary>
    /// Gets the manufacturer string, or an empty string when it is unavailable.
    /// </summary>
    public string Manufacturer { get; }

    /// <summary>
    /// Gets the USB interface number, or <see langword="null"/> when it is unavailable.
    /// </summary>
    public int? InterfaceNumber { get; }

    /// <summary>
    /// Gets the maximum feature report length including any report ID.
    /// </summary>
    public int MaximumFeatureReportLength { get; }

    /// <summary>
    /// Gets the maximum input report length including any report ID.
    /// </summary>
    public int MaximumInputReportLength { get; }

    /// <summary>
    /// Gets the maximum output report length including any report ID.
    /// </summary>
    public int MaximumOutputReportLength { get; }

    /// <summary>
    /// Gets the USB product identifier.
    /// </summary>
    public int ProductID { get; }

    /// <summary>
    /// Gets the product string, or an empty string when it is unavailable.
    /// </summary>
    public string ProductName { get; }

    /// <summary>
    /// Gets the binary-coded device release number.
    /// </summary>
    public int ReleaseNumberBcd { get; }

    /// <summary>
    /// Gets the serial number, or an empty string when it is unavailable.
    /// </summary>
    public string SerialNumber { get; }

    /// <summary>
    /// Gets the USB vendor identifier.
    /// </summary>
    public int VendorID { get; }

    /// <summary>
    /// Gets the reports use identifier.
    /// </summary>
    internal bool ReportsUseID { get; }

    #endregion

    #region Public

    /// <summary>
    /// Opens the HID interface.
    /// </summary>
    /// <returns>An opened HID stream.</returns>
    public HIDStream Open()
    {
        return new HIDStream(this, HIDBackendFactory.Open(this));
    }

    /// <summary>
    /// Attempts to open the HID interface.
    /// </summary>
    /// <param name="stream">Receives the opened stream when successful.</param>
    /// <returns><see langword="true"/> when the interface was opened; otherwise, <see langword="false"/>.</returns>
    public bool TryOpen(out HIDStream stream)
    {
        try
        {
            stream = Open();

            return true;
        }
        catch (IOException)
        {
            stream = null;

            return false;
        }
        catch (UnauthorizedAccessException)
        {
            stream = null;

            return false;
        }
        catch (NotSupportedException)
        {
            stream = null;

            return false;
        }
    }

    #endregion
}
