/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID;

/// <summary>
/// Provides the HID report descriptor info implementation used by the hardware monitoring system.
/// </summary>
internal sealed class HIDReportDescriptorInfo
{
    #region Fields

    /// <summary>
    /// Defines the stable prefix used when creating long item prefix identifiers.
    /// </summary>
    private const byte LongItemPrefix = 0xFE;

    /// <summary>
    /// Defines the expected size of long item header length.
    /// </summary>
    private const int LongItemHeaderLength = 2;

    /// <summary>
    /// Defines the bit mask used to extract short item size mask.
    /// </summary>
    private const int ShortItemSizeMask = 0x03;

    /// <summary>
    /// Defines the fixed four byte item size code value used by this component.
    /// </summary>
    private const int FourByteItemSizeCode = 3;

    /// <summary>
    /// Defines the expected size of four byte item size.
    /// </summary>
    private const int FourByteItemSize = 4;

    /// <summary>
    /// Defines the bit shift applied when decoding item type shift.
    /// </summary>
    private const int ItemTypeShift = 2;

    /// <summary>
    /// Defines the bit mask used to extract item type mask.
    /// </summary>
    private const int ItemTypeMask = 0x03;

    /// <summary>
    /// Defines the bit shift applied when decoding item tag shift.
    /// </summary>
    private const int ItemTagShift = 4;

    /// <summary>
    /// Defines the bit mask used to extract item tag mask.
    /// </summary>
    private const int ItemTagMask = 0x0F;

    /// <summary>
    /// Defines the fixed main item type value used by this component.
    /// </summary>
    private const int MainItemType = 0;

    /// <summary>
    /// Defines the fixed global item type value used by this component.
    /// </summary>
    private const int GlobalItemType = 1;

    /// <summary>
    /// Defines the fixed global report size tag value used by this component.
    /// </summary>
    private const int GlobalReportSizeTag = 7;

    /// <summary>
    /// Defines the fixed global report ID tag value used by this component.
    /// </summary>
    private const int GlobalReportIDTag = 8;

    /// <summary>
    /// Defines the fixed global report count tag value used by this component.
    /// </summary>
    private const int GlobalReportCountTag = 9;

    /// <summary>
    /// Defines the fixed global push tag value used by this component.
    /// </summary>
    private const int GlobalPushTag = 10;

    /// <summary>
    /// Defines the fixed global pop tag value used by this component.
    /// </summary>
    private const int GlobalPopTag = 11;

    /// <summary>
    /// Defines the fixed main input tag value used by this component.
    /// </summary>
    private const int MainInputTag = 8;

    /// <summary>
    /// Defines the fixed main output tag value used by this component.
    /// </summary>
    private const int MainOutputTag = 9;

    /// <summary>
    /// Defines the fixed main feature tag value used by this component.
    /// </summary>
    private const int MainFeatureTag = 11;

    /// <summary>
    /// Defines the fixed bits per byte value used by this component.
    /// </summary>
    private const int BitsPerByte = 8;

    /// <summary>
    /// Defines the expected size of report ID length.
    /// </summary>
    private const int ReportIDLength = 1;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="HIDReportDescriptorInfo"/> class.
    /// </summary>
    /// <param name="maximumInputReportLength">The maximum input report length used by the operation.</param>
    /// <param name="maximumOutputReportLength">The maximum output report length used by the operation.</param>
    /// <param name="maximumFeatureReportLength">The maximum feature report length used by the operation.</param>
    /// <param name="reportsUseID">The reports use identifier.</param>
    private HIDReportDescriptorInfo(
        int maximumInputReportLength,
        int maximumOutputReportLength,
        int maximumFeatureReportLength,
        bool reportsUseID)
    {
        MaximumInputReportLength = maximumInputReportLength;
        MaximumOutputReportLength = maximumOutputReportLength;
        MaximumFeatureReportLength = maximumFeatureReportLength;
        ReportsUseID = reportsUseID;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the maximum feature report length.
    /// </summary>
    public int MaximumFeatureReportLength { get; }

    /// <summary>
    /// Gets the maximum input report length.
    /// </summary>
    public int MaximumInputReportLength { get; }

    /// <summary>
    /// Gets the maximum output report length.
    /// </summary>
    public int MaximumOutputReportLength { get; }

    /// <summary>
    /// Gets the reports use identifier.
    /// </summary>
    public bool ReportsUseID { get; }

    #endregion

    #region Public

    /// <summary>
    /// Processes parse for the HID report descriptor info component.
    /// </summary>
    /// <param name="descriptor">The descriptor used by the operation.</param>
    /// <returns>The value produced by parse.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="descriptor"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="FormatException">Thrown when the operation cannot be completed.</exception>
    public static HIDReportDescriptorInfo Parse(byte[] descriptor)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        var inputBits = new Dictionary<int, long>();
        var outputBits = new Dictionary<int, long>();
        var featureBits = new Dictionary<int, long>();
        var globals = new GlobalState();
        var globalStack = new Stack<GlobalState>();
        var reportsUseID = false;

        for (var offset = 0; offset < descriptor.Length;)
        {
            var prefix = descriptor[offset++];

            if (prefix == LongItemPrefix)
            {
                if (offset + LongItemHeaderLength > descriptor.Length)
                {
                    throw new FormatException("The HID report descriptor contains a truncated long item.");
                }

                var length = descriptor[offset++];

                ++offset;

                if (offset + length > descriptor.Length)
                {
                    throw new FormatException("The HID report descriptor contains a truncated long item payload.");
                }

                offset += length;

                continue;
            }

            var encodedSize = prefix & ShortItemSizeMask;
            var itemSize = encodedSize == FourByteItemSizeCode
                ? FourByteItemSize
                : encodedSize;
            var itemType = (prefix >> ItemTypeShift) & ItemTypeMask;
            var itemTag = (prefix >> ItemTagShift) & ItemTagMask;

            if (offset + itemSize > descriptor.Length)
            {
                throw new FormatException("The HID report descriptor contains a truncated item.");
            }

            var value = ReadUnsignedValue(descriptor, offset, itemSize);

            offset += itemSize;

            if (itemType == GlobalItemType)
            {
                switch (itemTag)
                {
                    case GlobalReportSizeTag:
                    {
                        globals.ReportSize = value;
                    } break;
                    case GlobalReportIDTag:
                    {
                        if (value == 0 || value > byte.MaxValue)
                        {
                            throw new FormatException("The HID report descriptor contains an invalid report ID.");
                        }

                        globals.ReportID = checked((int)value);
                        reportsUseID = true;
                    } break;
                    case GlobalReportCountTag:
                    {
                        globals.ReportCount = value;
                    } break;
                    case GlobalPushTag:
                    {
                        globalStack.Push(globals);
                    } break;
                    case GlobalPopTag:
                    {
                        if (globalStack.Count == 0)
                        {
                            throw new FormatException("The HID report descriptor global stack is unbalanced.");
                        }

                        globals = globalStack.Pop();
                    } break;
                }

                continue;
            }

            if (itemType != MainItemType)
            {
                continue;
            }

            Dictionary<int, long> reportBits;

            switch (itemTag)
            {
                case MainInputTag:
                {
                    reportBits = inputBits;
                } break;
                case MainOutputTag:
                {
                    reportBits = outputBits;
                } break;
                case MainFeatureTag:
                {
                    reportBits = featureBits;
                } break;
                default:
                    continue;
            }

            long bitCount;

            // Guard the bit-count calculation because a malformed descriptor can exceed the addressable report length.
            try
            {
                bitCount = checked((long)globals.ReportSize * globals.ReportCount);
            }
            catch (OverflowException exception)
            {
                throw new FormatException("The HID report descriptor contains an oversized report.", exception);
            }

            if (reportBits.TryGetValue(globals.ReportID, out var existingBits))
            {
                bitCount = checked(bitCount + existingBits);
            }

            reportBits[globals.ReportID] = bitCount;
        }

        return new HIDReportDescriptorInfo(
            GetMaximumReportLength(inputBits),
            GetMaximumReportLength(outputBits),
            GetMaximumReportLength(featureBits),
            reportsUseID);
    }

    #endregion

    #region Private

    /// <summary>
    /// Retrieves maximum report length.
    /// </summary>
    /// <param name="reportBits">The report bits used by the operation.</param>
    /// <returns>The requested maximum report length.</returns>
    private static int GetMaximumReportLength(Dictionary<int, long> reportBits)
    {
        if (reportBits.Count == 0)
        {
            return 0;
        }

        var maximumBits = reportBits.Values.Max();

        return checked((int)((maximumBits + BitsPerByte - 1) / BitsPerByte) + ReportIDLength);
    }

    /// <summary>
    /// Reads unsigned value from the underlying device or platform.
    /// </summary>
    /// <param name="descriptor">The descriptor used by the operation.</param>
    /// <param name="offset">The zero-based byte offset at which processing begins.</param>
    /// <param name="size">The size used by the operation.</param>
    /// <returns>The unsigned value value read from the underlying device or platform.</returns>
    private static uint ReadUnsignedValue(byte[] descriptor, int offset, int size)
    {
        uint value = 0;

        for (var index = 0; index < size; ++index)
        {
            value |= (uint)descriptor[offset + index] << (index * BitsPerByte);
        }

        return value;
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Represents global state data.
    /// </summary>
    private struct GlobalState
    {
        /// <summary>
        /// Carries the report ID value exchanged with the native API.
        /// </summary>
        public int ReportID;

        /// <summary>
        /// Defines the expected size of report count.
        /// </summary>
        public uint ReportCount;

        /// <summary>
        /// Defines the expected size of report size.
        /// </summary>
        public uint ReportSize;
    }

    #endregion
}
