/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Data;
using Avalonia.Data.Converters;
using BlackSharp.UI.Avalonia.Converter;

namespace BlackSharp.UI.Avalonia.Data
{
    /// <summary>
    /// Binding which scales to given <see cref="Factor"/>.
    /// </summary>
    public class ScaleBinding : Binding
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleBinding"/> class.
        /// </summary>
        public ScaleBinding()
            : base()
        {
            base.Converter = _Converter;
            ConverterParameter = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleBinding"/> class.
        /// </summary>
        /// <param name="path">The binding path.</param>
        /// <param name="mode">The binding mode.</param>
        public ScaleBinding(string path, BindingMode mode = BindingMode.Default)
            : base(path, mode)
        {
            base.Converter = _Converter;
            ConverterParameter = 1.0;
        }

        #endregion

        #region Fields

        DoubleMultiplyConverter _Converter = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the scale factor.
        /// </summary>
        public double Factor
        {
            get { return (double)ConverterParameter; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException($"{nameof(Factor)} must be greater than zero (received: {value}).");
                ConverterParameter = value;
            }
        }

        /// <inheritdoc cref="BindingBase.Converter"/>
        public new IValueConverter Converter
        {
            get { return _Converter; }
            set
            {
                throw new InvalidOperationException($"Cannot set Converter on {nameof(ScaleBinding)}. The converter is built-in.");
            }
        }

        #endregion
    }
}
