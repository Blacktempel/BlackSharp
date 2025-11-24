/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions;
using System.Globalization;

namespace BlackSharp.UI.Avalonia.Controls
{
    #region TextBoxNumberType

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Specifies the numeric type that a text box is intended to accept or display.
    /// </summary>
    /// <remarks>Use this enumeration to indicate the expected numeric format for input or output in a text box.<br/>
    /// Selecting the appropriate value helps ensure correct parsing, validation, and formatting of numeric data.<br/>
    /// The values correspond to common signed and unsigned integer types, as well as floating-point and decimal types.</remarks>
    public enum TextBoxNumberType
    {
        SByte,
        Short,
        Int,
        Long,

        Byte,
        UShort,
        UInt,
        ULong,

        Float,
        Double,
        Decimal
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    #endregion

    /// <summary>
    /// Provides a text box control that restricts input to numeric values, supporting various number types and precision settings.
    /// </summary>
    /// <remarks>Use this control to ensure that user input is limited to valid numeric formats, such as
    /// integers or decimals, as specified by the <see cref="TextBoxNumberType"/> property.<br/>
    /// The control allows configuration of whether zero and negative values are permitted, as well as the precision of floating-point numbers.<br/>
    /// This is useful for scenarios where only specific numeric input is acceptable, such as financial or
    /// scientific data entry.</remarks>
    public class TextBoxNumber : TextBoxRestrictionBase
    {
        #region Fields

        Dictionary<TextBoxNumberType, Type> _NumberTypes = new Dictionary<TextBoxNumberType, Type>
        {
            { TextBoxNumberType.SByte,   typeof(sbyte  ) },
            { TextBoxNumberType.Short,   typeof(short  ) },
            { TextBoxNumberType.Int,     typeof(int    ) },
            { TextBoxNumberType.Long,    typeof(long   ) },

            { TextBoxNumberType.Byte,    typeof(byte   ) },
            { TextBoxNumberType.UShort,  typeof(ushort ) },
            { TextBoxNumberType.UInt,    typeof(uint   ) },
            { TextBoxNumberType.ULong,   typeof(ulong  ) },

            { TextBoxNumberType.Float,   typeof(float  ) },
            { TextBoxNumberType.Double,  typeof(double ) },
            { TextBoxNumberType.Decimal, typeof(decimal) },
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether zero is considered a valid input or result in relevant operations.
        /// </summary>
        /// <remarks>Set this property to <see langword="true"/> to allow zero values where applicable.<br/>
        /// If set to <see langword="false"/>, operations may reject or handle zero differently, depending on the context.</remarks>
        public bool AllowZero { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether negative values are permitted.
        /// </summary>
        /// <remarks>Set this property to <see langword="false"/> to restrict input or operations to non-negative values only.<br/>
        /// Changing this property may affect validation or calculation logic that depends on the allowance of negative values.</remarks>
        public bool AllowNegative { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of digits to use for numeric precision.
        /// </summary>
        /// <remarks>A value of -1 indicates that no specific precision is set and the default behavior will be used.<br/>
        /// Setting this property to a non-negative integer specifies the exact number of digits for precision in numeric operations.</remarks>
        public int Precision { get; set; } = -1;

        /// <summary>
        /// Gets or sets the type of numeric value that the text box accepts.
        /// </summary>
        /// <remarks>Set this property to specify whether the text box should accept integer, decimal, or
        /// other numeric formats as defined by the <see cref="TextBoxNumberType"/> enumeration.</remarks>
        public TextBoxNumberType NumberType { get; set; } = TextBoxNumberType.Int;

        #endregion

        #region Protected

        /// <inheritdoc cref="TextBoxRestrictionBase.IsTextAllowed"/>
        protected override bool IsTextAllowed(string text)
        {
            if (TryParseNumberType(text, out var number, out var isNegative))
            {
                if (!AllowZero && number.Equals(0.0))
                {
                    return false;
                }
                else if (!AllowNegative && isNegative)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Private

        bool TryParseNumberType(string s, out object number, out bool isNegative)
        {
            bool result = false;
            string invariantSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
            string negativeSign = CultureInfo.CurrentCulture.NumberFormat.NegativeSign;
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (invariantSeparator != separator) //Culture is not default culture (e.g. enUS != deDE)
            {
                if (s.IndexOf(invariantSeparator + invariantSeparator) != -1) //Two times the same invariant separator
                {
                    number = 0.0;
                    isNegative = false;
                    return false;
                }
            }

            if (s.Length == 1 && s.StartsWith(negativeSign))
            {
                if (!AllowNegative)
                {
                    number = 0.0;
                    isNegative = true;
                    return false;
                }
                s += '0';
            }

            if (s.StartsWith(negativeSign + separator))
            {
                number = 0.0;
                isNegative = false;
                return false;
            }

            if (s.EndsWith(separator) &&
                s.Count(Convert.ToChar(separator)) == 1 &&
                NumberType.Any(TextBoxNumberType.Float, TextBoxNumberType.Double, TextBoxNumberType.Decimal))
            {
                s += '0';
            }

            var type = _NumberTypes[NumberType];

            //Check precision
            if (Precision >= 0 && type.IsFloatingType())
            {
                var splitted = s.Split(Convert.ToChar(separator));

                //Precision is only available when Length == 2
                //Only cut if precision is too high
                if (splitted.Length == 2 && splitted[1].Length > Precision)
                {
                    number = 0.0;
                    isNegative = false;
                    return false;
                }
            }

            var tryParse = type.GetMethod(nameof(int.TryParse), new[] { typeof(string), type.MakeByRefType() });

            dynamic outParameter = null;
            object[] args = new object[] { s, outParameter };

            result = (bool)tryParse.Invoke(null, args);

            dynamic parsed = args[1];
            isNegative = parsed < type.GetDefault();
            number = parsed;

            return result;
        }

        #endregion
    }
}
