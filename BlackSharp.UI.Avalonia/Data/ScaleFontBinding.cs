/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Data;

namespace BlackSharp.UI.Avalonia.Data
{
    /// <summary>
    /// Binding for font scaling.
    /// </summary>
    public class ScaleFontBinding : ScaleBinding
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleFontBinding"/> class.
        /// </summary>
        public ScaleFontBinding()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleFontBinding"/> class.
        /// </summary>
        /// <param name="factor">The scale factor.</param>
        public ScaleFontBinding(double factor)
        {
            Factor = factor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleFontBinding"/> class.
        /// </summary>
        /// <param name="path">The binding path.</param>
        /// <param name="factor">The scale factor.</param>
        /// <param name="mode">The binding mode.</param>
        public ScaleFontBinding(string path, double factor, BindingMode mode = BindingMode.Default)
            : base(path, mode)
        {
            Factor = factor;
        }

        #endregion
    }
}
