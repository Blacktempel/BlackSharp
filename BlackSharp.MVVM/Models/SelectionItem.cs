/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.MVVM.Models
{
    /// <summary>
    /// Provides display information shared by selectable items.
    /// </summary>
    public abstract class SelectionItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a selectable item.
        /// </summary>
        /// <param name="displayName">The text displayed for the item.</param>
        /// <param name="toolTip">The optional tooltip displayed for the item.</param>
        protected SelectionItem(string displayName, string toolTip = null)
        {
            DisplayName = displayName ?? string.Empty;
            ToolTip     = toolTip     ?? string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the text displayed for the item.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the optional tooltip displayed for the item.
        /// </summary>
        public string ToolTip { get; }

        /// <summary>
        /// Gets or sets whether the item can be selected.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        #endregion
    }
}
