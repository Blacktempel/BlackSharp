// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.MVVM.Models
{
    /// <summary>
    /// Associates a selectable enum value with its display information.
    /// </summary>
    /// <typeparam name="T">The enum type represented by the item.</typeparam>
    public sealed class SelectionItem<T> : SelectionItem
        where T : struct, Enum
    {
        #region Constructor

        /// <summary>
        /// Initializes a selectable enum item.
        /// </summary>
        /// <param name="value">The enum value represented by the item.</param>
        /// <param name="displayName">The text displayed for the item.</param>
        /// <param name="toolTip">The optional tooltip displayed for the item.</param>
        public SelectionItem(T value, string displayName, string toolTip = null)
            : base(displayName, toolTip)
        {
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the enum value represented by the item.
        /// </summary>
        public T Value { get; }

        #endregion
    }
}
