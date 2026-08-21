using BlackSharp.Core.Interop.Linux.Native;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.HID.Backends;

/// <summary>
/// Provides the HID raw implementation used by the hardware monitoring system.
/// </summary>
internal static class HIDRaw
{
    #region Fields

    /// <summary>
    /// Defines the expected size of maximum report descriptor length.
    /// </summary>
    private const int MaximumReportDescriptorLength = 4096;

    /// <summary>
    /// Defines the fixed number bits value used by this component.
    /// </summary>
    private const int NumberBits = 8;

    /// <summary>
    /// Defines the fixed type bits value used by this component.
    /// </summary>
    private const int TypeBits = 8;

    /// <summary>
    /// Defines the fixed size bits value used by this component.
    /// </summary>
    private const int SizeBits = 14;

    /// <summary>
    /// Defines the bit shift applied when decoding number shift.
    /// </summary>
    private const int NumberShift = 0;

    /// <summary>
    /// Defines the bit shift applied when decoding type shift.
    /// </summary>
    private const int TypeShift = NumberShift + NumberBits;

    /// <summary>
    /// Defines the bit shift applied when decoding size shift.
    /// </summary>
    private const int SizeShift = TypeShift + TypeBits;

    /// <summary>
    /// Defines the bit shift applied when decoding direction shift.
    /// </summary>
    private const int DirectionShift = SizeShift + SizeBits;

    /// <summary>
    /// Defines the fixed write direction value used by this component.
    /// </summary>
    private const uint WriteDirection = 1;

    /// <summary>
    /// Defines the fixed read direction value used by this component.
    /// </summary>
    private const uint ReadDirection = 2;

    /// <summary>
    /// Defines the fixed request type value used by this component.
    /// </summary>
    private const uint RequestType = 'H';

    /// <summary>
    /// Defines the fixed get report descriptor size number value used by this component.
    /// </summary>
    private const int GetReportDescriptorSizeNumber = 0x01;

    /// <summary>
    /// Defines the fixed get report descriptor number value used by this component.
    /// </summary>
    private const int GetReportDescriptorNumber = 0x02;

    /// <summary>
    /// Defines the fixed set feature number value used by this component.
    /// </summary>
    private const int SetFeatureNumber = 0x06;

    /// <summary>
    /// Defines the fixed get feature number value used by this component.
    /// </summary>
    private const int GetFeatureNumber = 0x07;

    #endregion

    #region Public

    /// <summary>
    /// Retrieves feature request.
    /// </summary>
    /// <param name="size">The size used by the operation.</param>
    /// <returns>The requested feature request.</returns>
    public static UIntPtr GetFeatureRequest(int size)
    {
        return CreateIOControlCode(true, true, GetFeatureNumber, size);
    }

    /// <summary>
    /// Reads report descriptor from the underlying device or platform.
    /// </summary>
    /// <param name="descriptor">The descriptor used by the operation.</param>
    /// <returns>The report descriptor entries read from the underlying device or platform.</returns>
    /// <exception cref="IOException">Thrown when the operation cannot be completed.</exception>
    public static byte[] ReadReportDescriptor(int descriptor)
    {
        // HIDRAW first reports the descriptor length; validate it before allocating the native request structure.
        if (LibC.ioctl(descriptor, GetReportDescriptorSizeRequest(), out var descriptorLength) < 0
         || descriptorLength <= 0
         || descriptorLength > MaximumReportDescriptorLength)
        {
            throw new IOException("Unable to read the HID report descriptor length.");
        }

        // The kernel layout starts with a UInt32 size followed by a fixed-capacity descriptor byte array.
        var nativeDescriptor = Marshal.AllocHGlobal(sizeof(uint) + MaximumReportDescriptorLength);

        try
        {
            Marshal.WriteInt32(nativeDescriptor, descriptorLength);

            if (LibC.ioctl(descriptor, GetReportDescriptorRequest(), nativeDescriptor) < 0)
            {
                throw new IOException("Unable to read the HID report descriptor.");
            }

            var value = new byte[descriptorLength];

            Marshal.Copy(IntPtr.Add(nativeDescriptor, sizeof(uint)), value, 0, value.Length);

            return value;
        }
        finally
        {
            Marshal.FreeHGlobal(nativeDescriptor);
        }
    }

    /// <summary>
    /// Updates feature request.
    /// </summary>
    /// <param name="size">The size used by the operation.</param>
    /// <returns>The value produced by set feature request.</returns>
    public static UIntPtr SetFeatureRequest(int size)
    {
        return CreateIOControlCode(true, true, SetFeatureNumber, size);
    }

    #endregion

    #region Private

    /// <summary>
    /// Creates I/O control code.
    /// </summary>
    /// <param name="read">The read used by the operation.</param>
    /// <param name="write">The write used by the operation.</param>
    /// <param name="number">The processor number within its group.</param>
    /// <param name="size">The size used by the operation.</param>
    /// <returns>A new I/O control code instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is outside the supported range.
    /// </exception>
    private static UIntPtr CreateIOControlCode(bool read, bool write, int number, int size)
    {
        if (size < 0 || size >= 1 << SizeBits)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        var direction = (read ? ReadDirection : 0)
                      | (write ? WriteDirection : 0);
        var value = direction << DirectionShift
                  | (uint)size << SizeShift
                  | RequestType << TypeShift
                  | (uint)number;

        return (UIntPtr)value;
    }

    /// <summary>
    /// Retrieves report descriptor request.
    /// </summary>
    /// <returns>The requested report descriptor request.</returns>
    private static UIntPtr GetReportDescriptorRequest()
    {
        return CreateIOControlCode(true, false, GetReportDescriptorNumber, sizeof(uint) + MaximumReportDescriptorLength);
    }

    /// <summary>
    /// Retrieves report descriptor size request.
    /// </summary>
    /// <returns>The requested report descriptor size request.</returns>
    private static UIntPtr GetReportDescriptorSizeRequest()
    {
        return CreateIOControlCode(true, false, GetReportDescriptorSizeNumber, sizeof(int));
    }

    #endregion
}
