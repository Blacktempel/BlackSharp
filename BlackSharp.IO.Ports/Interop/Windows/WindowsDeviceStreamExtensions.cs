namespace BlackSharp.IO.Ports.Interop.Windows;

/// <summary>
/// Provides higher-level read operations for <see cref="WindowsDeviceStream"/>.
/// </summary>
public static class WindowsDeviceStreamExtensions
{
    #region Public

    /// <summary>
    /// Reads until at least the requested number of bytes was received or an operation fails.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="minimumLength">The minimum number of bytes required for success.</param>
    /// <param name="bufferLength">The maximum number of bytes to read.</param>
    /// <param name="timeoutMilliseconds">The maximum duration of each individual read operation.</param>
    /// <param name="data">Receives the bytes read.</param>
    /// <returns>
    /// <see langword="true"/> when at least <paramref name="minimumLength"/> bytes were received; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public static bool ReadAtLeast(
        this WindowsDeviceStream stream,
        int minimumLength,
        int bufferLength,
        int timeoutMilliseconds,
        out byte[] data)
    {
        data = null;
        if (stream == null || minimumLength <= 0 || bufferLength < minimumLength)
        {
            return false;
        }

        var buffer = new byte[bufferLength];
        var total = 0;

        while (total < minimumLength)
        {
            var part = new byte[bufferLength - total];
            if (!stream.Read(part, timeoutMilliseconds, out int bytesRead) || bytesRead <= 0)
            {
                break;
            }

            Buffer.BlockCopy(part, 0, buffer, total, bytesRead);
            total += bytesRead;

            if (total == bufferLength)
            {
                break;
            }
        }

        if (total < minimumLength)
        {
            return false;
        }

        if (total != buffer.Length)
        {
            Array.Resize(ref buffer, total);
        }

        data = buffer;

        return true;
    }

    #endregion
}
