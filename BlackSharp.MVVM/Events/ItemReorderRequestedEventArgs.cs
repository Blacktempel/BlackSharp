// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.MVVM.Events
{
    /// <summary>
    /// Provides information about a requested item reorder operation.
    /// </summary>
    public sealed class ItemReorderRequestedEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReorderRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="item">Item being reordered.</param>
        /// <param name="oldIndex">Current zero-based item index.</param>
        /// <param name="newIndex">Requested zero-based item index.</param>
        public ItemReorderRequestedEventArgs(
            object item,
            int oldIndex,
            int newIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(oldIndex);
            ArgumentOutOfRangeException.ThrowIfNegative(newIndex);

            Item     = item;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the reorder operation was handled.
        /// </summary>
        /// <remarks>
        /// If this value remains <see langword="false"/>, the originating component may attempt
        /// to move the item in its writable items source.
        /// </remarks>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the item being reordered.
        /// </summary>
        public object Item { get; }

        /// <summary>
        /// Gets the requested zero-based item index.
        /// </summary>
        public int NewIndex { get; }

        /// <summary>
        /// Gets the current zero-based item index.
        /// </summary>
        public int OldIndex { get; }

        #endregion
    }
}
