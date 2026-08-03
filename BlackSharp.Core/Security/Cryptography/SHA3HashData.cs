/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Security.Cryptography;

internal static class SHA3HashData
{
    #region Internal

    internal static byte[] HashData<THashAlgorithm>(byte[] source)
        where THashAlgorithm : SHA3, new()
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

#if NET8_0_OR_GREATER
        return HashData<THashAlgorithm>(source.AsSpan());
#else
        using (var algorithm = new THashAlgorithm())
        {
            return algorithm.ComputeHash(source);
        }
#endif
    }

#if NET8_0_OR_GREATER
    internal static byte[] HashData<THashAlgorithm>(ReadOnlySpan<byte> source)
        where THashAlgorithm : SHA3, new()
    {
        using (var algorithm = new THashAlgorithm())
        {
            var destination = new byte[algorithm.HashSize / 8];

            if (!algorithm.TryComputeHash(source, destination, out _))
            {
                throw new System.Security.Cryptography.CryptographicException("The SHA3 hash could not be written to the destination buffer.");
            }

            return destination;
        }
    }

    internal static int HashData<THashAlgorithm>(ReadOnlySpan<byte> source, Span<byte> destination)
        where THashAlgorithm : SHA3, new()
    {
        using (var algorithm = new THashAlgorithm())
        {
            ValidateDestination(destination, algorithm.HashSize / 8);

            if (!algorithm.TryComputeHash(source, destination, out var bytesWritten))
            {
                throw new System.Security.Cryptography.CryptographicException("The SHA3 hash could not be written to the destination buffer.");
            }

            return bytesWritten;
        }
    }
#endif

    internal static byte[] HashData<THashAlgorithm>(Stream source)
        where THashAlgorithm : SHA3, new()
    {
        ValidateSource(source);

        using (var algorithm = new THashAlgorithm())
        {
            return algorithm.ComputeHash(source);
        }
    }

#if NET8_0_OR_GREATER
    internal static int HashData<THashAlgorithm>(Stream source, Span<byte> destination)
        where THashAlgorithm : SHA3, new()
    {
        ValidateSource(source);

        using (var algorithm = new THashAlgorithm())
        {
            ValidateDestination(destination, algorithm.HashSize / 8);

            var hash = algorithm.ComputeHash(source);
            hash.CopyTo(destination);

            return hash.Length;
        }
    }
#endif

    #endregion

    #region Private

#if NET8_0_OR_GREATER
    static void ValidateDestination(Span<byte> destination, int hashByteLength)
    {
        if (destination.Length < hashByteLength)
        {
            throw new ArgumentException(
                $"The destination buffer must be at least {hashByteLength} bytes long.",
                nameof(destination));
        }
    }
#endif

    static void ValidateSource(Stream source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (!source.CanRead)
        {
            throw new ArgumentException("The stream does not support reading.", nameof(source));
        }
    }

    #endregion
}
