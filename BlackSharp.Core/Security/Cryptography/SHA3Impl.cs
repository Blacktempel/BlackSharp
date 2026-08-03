/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Security.Cryptography
{
    /// <summary>
    /// Computes SHA3-224 hashes.
    /// </summary>
    public sealed class SHA3_224 : SHA3
    {
        #region Constructor

        /// <summary>
        /// Initializes a new SHA3-224 hash implementation.
        /// </summary>
        public SHA3_224()
            : base(224)
        {
        }

        #endregion

        #region Public

        /// <summary>
        /// Computes the SHA3-224 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static byte[] HashData(byte[] source)
        {
            return SHA3HashData.HashData<SHA3_224>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-224 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        public static byte[] HashData(ReadOnlySpan<byte> source)
        {
            return SHA3HashData.HashData<SHA3_224>(source);
        }

        /// <summary>
        /// Computes the SHA3-224 hash of the specified data and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer that receives the 28-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is too small.</exception>
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_224>(source, destination);
        }
#endif

        /// <summary>
        /// Computes the SHA3-224 hash of the remaining data in a stream.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/> does not support reading.</exception>
        public static byte[] HashData(Stream source)
        {
            return SHA3HashData.HashData<SHA3_224>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-224 hash of the remaining data in a stream and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <param name="destination">The buffer that receives the 28-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> does not support reading or <paramref name="destination"/> is too small.
        /// </exception>
        public static int HashData(Stream source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_224>(source, destination);
        }
#endif

        #endregion
    }

    /// <summary>
    /// Computes SHA3-256 hashes.
    /// </summary>
    public sealed class SHA3_256 : SHA3
    {
        #region Constructor

        /// <summary>
        /// Initializes a new SHA3-256 hash implementation.
        /// </summary>
        public SHA3_256()
            : base(256)
        {
        }

        #endregion

        #region Public

        /// <summary>
        /// Computes the SHA3-256 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static byte[] HashData(byte[] source)
        {
            return SHA3HashData.HashData<SHA3_256>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-256 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        public static byte[] HashData(ReadOnlySpan<byte> source)
        {
            return SHA3HashData.HashData<SHA3_256>(source);
        }

        /// <summary>
        /// Computes the SHA3-256 hash of the specified data and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer that receives the 32-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is too small.</exception>
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_256>(source, destination);
        }
#endif

        /// <summary>
        /// Computes the SHA3-256 hash of the remaining data in a stream.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/> does not support reading.</exception>
        public static byte[] HashData(Stream source)
        {
            return SHA3HashData.HashData<SHA3_256>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-256 hash of the remaining data in a stream and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <param name="destination">The buffer that receives the 32-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> does not support reading or <paramref name="destination"/> is too small.
        /// </exception>
        public static int HashData(Stream source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_256>(source, destination);
        }
#endif

        #endregion
    }

    /// <summary>
    /// Computes SHA3-384 hashes.
    /// </summary>
    public sealed class SHA3_384 : SHA3
    {
        #region Constructor

        /// <summary>
        /// Initializes a new SHA3-384 hash implementation.
        /// </summary>
        public SHA3_384()
            : base(384)
        {
        }

        #endregion

        #region Public

        /// <summary>
        /// Computes the SHA3-384 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static byte[] HashData(byte[] source)
        {
            return SHA3HashData.HashData<SHA3_384>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-384 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        public static byte[] HashData(ReadOnlySpan<byte> source)
        {
            return SHA3HashData.HashData<SHA3_384>(source);
        }

        /// <summary>
        /// Computes the SHA3-384 hash of the specified data and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer that receives the 48-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is too small.</exception>
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_384>(source, destination);
        }
#endif

        /// <summary>
        /// Computes the SHA3-384 hash of the remaining data in a stream.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/> does not support reading.</exception>
        public static byte[] HashData(Stream source)
        {
            return SHA3HashData.HashData<SHA3_384>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-384 hash of the remaining data in a stream and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <param name="destination">The buffer that receives the 48-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> does not support reading or <paramref name="destination"/> is too small.
        /// </exception>
        public static int HashData(Stream source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_384>(source, destination);
        }
#endif

        #endregion
    }

    /// <summary>
    /// Computes SHA3-512 hashes.
    /// </summary>
    public sealed class SHA3_512 : SHA3
    {
        #region Constructor

        /// <summary>
        /// Initializes a new SHA3-512 hash implementation.
        /// </summary>
        public SHA3_512()
            : base(512)
        {
        }

        #endregion

        #region Public

        /// <summary>
        /// Computes the SHA3-512 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public static byte[] HashData(byte[] source)
        {
            return SHA3HashData.HashData<SHA3_512>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-512 hash of the specified data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <returns>The computed hash.</returns>
        public static byte[] HashData(ReadOnlySpan<byte> source)
        {
            return SHA3HashData.HashData<SHA3_512>(source);
        }

        /// <summary>
        /// Computes the SHA3-512 hash of the specified data and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer that receives the 64-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is too small.</exception>
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_512>(source, destination);
        }
#endif

        /// <summary>
        /// Computes the SHA3-512 hash of the remaining data in a stream.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <returns>The computed hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/> does not support reading.</exception>
        public static byte[] HashData(Stream source)
        {
            return SHA3HashData.HashData<SHA3_512>(source);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Computes the SHA3-512 hash of the remaining data in a stream and writes it to a destination buffer.
        /// </summary>
        /// <param name="source">The stream to hash.</param>
        /// <param name="destination">The buffer that receives the 64-byte hash.</param>
        /// <returns>The number of bytes written to <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> does not support reading or <paramref name="destination"/> is too small.
        /// </exception>
        public static int HashData(Stream source, Span<byte> destination)
        {
            return SHA3HashData.HashData<SHA3_512>(source, destination);
        }
#endif

        #endregion
    }
}
