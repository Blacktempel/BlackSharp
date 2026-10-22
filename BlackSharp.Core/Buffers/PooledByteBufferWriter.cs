/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Buffers;

namespace BlackSharp.Core.Buffers
{
    /// <summary>
    /// Provides a growable <see cref="IBufferWriter{T}"/> backed by rented byte arrays.
    /// </summary>
    public sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Rents a buffer with at least the requested initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial number of bytes.</param>
        /// <param name="clearOnDispose">Whether returned buffers are cleared before reuse.</param>
        public PooledByteBufferWriter(int initialCapacity, bool clearOnDispose = false)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            }

            _buffer         = ArrayPool<byte>.Shared.Rent(Math.Max(1, initialCapacity));
            _clearOnDispose = clearOnDispose;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Requests clearing rented arrays before they return to the shared pool.
        /// </summary>
        private readonly bool _clearOnDispose;

        /// <summary>
        /// Holds the currently rented array.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Tracks how many bytes callers committed through <see cref="Advance"/>.
        /// </summary>
        private int _writtenCount;

        /// <summary>
        /// Prevents access after the rented array has been returned.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the written portion as read-only memory.
        /// </summary>
        public ReadOnlyMemory<byte> WrittenMemory => _buffer.AsMemory(0, _writtenCount);

        /// <summary>
        /// Gets the written portion as an array segment.
        /// </summary>
        public ArraySegment<byte> WrittenSegment => new(_buffer, 0, _writtenCount);

        /// <summary>
        /// Gets the written portion as a span.
        /// </summary>
        public Span<byte> WrittenSpan => _buffer.AsSpan(0, _writtenCount);

        /// <summary>
        /// Gets the number of committed bytes.
        /// </summary>
        public int WrittenCount => _writtenCount;

        #endregion

        #region Public

        /// <inheritdoc/>
        public void Advance(int count)
        {
            ThrowIfDisposed();

            if (count < 0 || _writtenCount > _buffer.Length - count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            _writtenCount += count;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            var buffer = _buffer;
            _buffer = Array.Empty<byte>();

            _writtenCount = 0;

            ArrayPool<byte>.Shared.Return(buffer, _clearOnDispose);
        }

        /// <inheritdoc/>
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _buffer.AsMemory(_writtenCount);
        }

        /// <inheritdoc/>
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _buffer.AsSpan(_writtenCount);
        }

        #endregion

        #region Private

        /// <summary>
        /// Expands the rented array when the requested writable space does not fit.
        /// </summary>
        /// <param name="sizeHint">The minimum contiguous writable byte count.</param>
        private void EnsureCapacity(int sizeHint)
        {
            ThrowIfDisposed();

            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            sizeHint = Math.Max(1, sizeHint);

            if (sizeHint <= _buffer.Length - _writtenCount)
            {
                return;
            }

            var requiredCapacity = checked(_writtenCount + sizeHint);
            var newCapacity = Math.Max(requiredCapacity, checked(_buffer.Length * 2));
            var newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);

            _buffer.AsSpan(0, _writtenCount).CopyTo(newBuffer);

            ArrayPool<byte>.Shared.Return(_buffer, _clearOnDispose);

            _buffer = newBuffer;
        }

        /// <summary>
        /// Rejects operations after disposal.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PooledByteBufferWriter));
            }
        }

        #endregion
    }
}
