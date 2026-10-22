// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Collections
{
    /// <summary>
    /// Stores values in insertion order using fixed-size segments while allowing efficient oldest-value removal.
    /// </summary>
    /// <typeparam name="T">The stored value type.</typeparam>
    public sealed class SegmentedRingBuffer<T>
    {
        #region Constructor

        /// <summary>
        /// Creates a segmented buffer.
        /// </summary>
        /// <param name="segmentSize">The number of entries allocated per segment.</param>
        public SegmentedRingBuffer(int segmentSize = 256)
        {
            if (segmentSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentSize));
            }

            _segmentSize = segmentSize;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Stores the fixed capacity of each segment.
        /// </summary>
        private readonly int _segmentSize;

        /// <summary>
        /// Holds segments from oldest to newest.
        /// </summary>
        private readonly Queue<T[]> _segments = new();

        /// <summary>
        /// References the segment currently receiving new values.
        /// </summary>
        private T[] _tailSegment;

        /// <summary>
        /// Identifies the first retained entry in the oldest segment.
        /// </summary>
        private int _headIndex;

        /// <summary>
        /// Identifies the next free entry in the newest segment.
        /// </summary>
        private int _tailIndex;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of retained entries.
        /// </summary>
        public int Count { get; private set; }

        #endregion

        #region Public

        /// <summary>
        /// Appends a value to the newest end of the buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        public void Add(T value)
        {
            if (_segments.Count == 0 || _tailIndex == _segmentSize)
            {
                _tailSegment = new T[_segmentSize];
                _segments.Enqueue(_tailSegment);

                _tailIndex = 0;
            }

            _tailSegment[_tailIndex++] = value;

            ++Count;
        }

        /// <summary>
        /// Removes oldest entries while a predicate matches them.
        /// </summary>
        /// <param name="predicate">The condition tested against the oldest entry.</param>
        public void TrimWhile(Predicate<T> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            while (Count > 0 && predicate(_segments.Peek()[_headIndex]))
            {
                RemoveFirst();
            }
        }

        /// <summary>
        /// Removes oldest entries until the count is no greater than the requested maximum.
        /// </summary>
        /// <param name="maximumCount">The maximum number of retained entries.</param>
        public void TrimToCount(int maximumCount)
        {
            if (maximumCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCount));
            }

            while (Count > maximumCount)
            {
                RemoveFirst();
            }
        }

        /// <summary>
        /// Tries to read the newest entry.
        /// </summary>
        /// <param name="value">Receives the newest entry.</param>
        /// <returns>Whether an entry exists.</returns>
        public bool TryGetLast(out T value)
        {
            if (Count == 0)
            {
                value = default;
                return false;
            }

            value = _tailSegment[_tailIndex - 1];
            return true;
        }

        /// <summary>
        /// Copies all retained entries in insertion order.
        /// </summary>
        /// <returns>A new array of retained entries.</returns>
        public T[] ToArray()
        {
            var result = new T[Count];

            var offset = 0;
            var index = _headIndex;

            foreach (var segment in _segments)
            {
                var length = Math.Min(_segmentSize - index, Count - offset);
                Array.Copy(segment, index, result, offset, length);

                offset += length;
                index = 0;
            }

            return result;
        }

        #endregion

        #region Private

        /// <summary>
        /// Removes the oldest value and releases an empty segment.
        /// </summary>
        private void RemoveFirst()
        {
            var segment = _segments.Peek();
            segment[_headIndex] = default;

            ++_headIndex;

            --Count;

            if (_headIndex == _segmentSize || Count == 0)
            {
                _segments.Dequeue();
                _headIndex = 0;

                if (Count == 0)
                {
                    _tailSegment = null;
                    _tailIndex = 0;
                }
            }
        }

        #endregion
    }
}
