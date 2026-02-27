/*
* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at https://mozilla.org/MPL/2.0/.
*
* Copyright (c) 2026 Florian K.
*
*/

using BlackSharp.Core.Converters.Enums;
using System.Numerics;

namespace BlackSharp.Core.Converters
{
    /// <summary>
    /// Provides methods for converting data quantities between different units, including both decimal (SI) and binary
    /// (IEC) prefixes for bits and bytes.
    /// </summary>
    /// <remarks>This class supports conversions among a wide range of data units, such as bits, bytes,
    /// kilobytes, kibibytes, and larger multiples, using either decimal (powers of 10) or binary (powers of 2)
    /// standards.<br/>
    /// All methods are static and thread-safe. Use the appropriate method to convert a value from its
    /// original unit to the desired target unit.<br/>
    /// If the conversion result exceeds the range of the <see cref="decimal"/> type, an
    /// <see cref="OverflowException"/> is thrown.</remarks>
    public static class DataUnitConverter
    {
        #region Public

        /// <summary>
        /// Converts a data quantity from one unit to another.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="typeOfValue">Original unit of the value.</param>
        /// <param name="targetType">Target unit for the conversion.</param>
        /// <returns>Converted value in the target unit.</returns>
        /// <exception cref="OverflowException">Thrown when the conversion result exceeds the range of the <see cref="decimal"/> type.</exception>
        public static decimal Convert(decimal value, DataUnit typeOfValue, DataUnit targetType)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than or equal to zero.");
            }

            //If the original unit and target unit are the same, return the original value
            if (typeOfValue == targetType)
            {
                return value;
            }

            GetDecimalAsFraction(value, out var valueNumerator, out var valueDenominator);

            var sourceFactor = GetFactorInBits(typeOfValue);
            var targetFactor = GetFactorInBits(targetType);

            var numerator = valueNumerator * sourceFactor;
            var denominator = valueDenominator * targetFactor;

            //Perform the division to get the integral part and the remainder for the fractional part
            var integralPart = BigInteger.DivRem(numerator, denominator, out var remainder);

            try
            {
                var integralDecimal = (decimal)integralPart;

                //If there is no remainder, return the integral part as the final result
                if (remainder.IsZero)
                {
                    return integralDecimal;
                }

                //Calculate the fractional part by dividing the remainder by the target factor and add it to the integral part
                return integralDecimal + (decimal)remainder / (decimal)denominator;
            }
            catch (OverflowException)
            {
                throw new OverflowException($"Converted value exceeds {nameof(Decimal)} range.");
            }
        }

        ///<inheritdoc cref="Convert"/>
        public static decimal ToByte      (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.Byte      );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToKiloByte  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.KiloByte  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToMegaByte  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.MegaByte  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToGigaByte  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.GigaByte  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToTeraByte  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.TeraByte  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToPetaByte  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.PetaByte  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToExaByte   (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ExaByte   );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToZettaByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ZettaByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToYottaByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.YottaByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToRonnaByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.RonnaByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToQuettaByte(decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.QuettaByte);

        ///<inheritdoc cref="Convert"/>
        public static decimal ToKibiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.KibiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToMebiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.MebiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToGibiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.GibiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToTebiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.TebiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToPebiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.PebiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToExbiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ExbiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToZebiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ZebiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToYobiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.YobiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToRobiByte (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.RobiByte );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToQuebiByte(decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.QuebiByte);

        ///<inheritdoc cref="Convert"/>
        public static decimal ToBit      (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.Bit      );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToKiloBit  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.KiloBit  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToMegaBit  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.MegaBit  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToGigaBit  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.GigaBit  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToTeraBit  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.TeraBit  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToPetaBit  (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.PetaBit  );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToExaBit   (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ExaBit   );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToZettaBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ZettaBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToYottaBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.YottaBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToRonnaBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.RonnaBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToQuettaBit(decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.QuettaBit);

        ///<inheritdoc cref="Convert"/>
        public static decimal ToKibiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.KibiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToMebiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.MebiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToGibiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.GibiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToTebiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.TebiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToPebiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.PebiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToExbiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ExbiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToZebiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.ZebiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToYobiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.YobiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToRobiBit (decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.RobiBit );
        ///<inheritdoc cref="Convert"/>
        public static decimal ToQuebiBit(decimal value, DataUnit typeOfValue) => Convert(value, typeOfValue, DataUnit.QuebiBit);

        #endregion

        #region Internal

        internal static BigInteger GetFactorInBits(DataUnit type)
        {
            const int bitsPerByte = 8;

            var kilo   = BigInteger.Pow(10,  3);
            var mega   = BigInteger.Pow(10,  6);
            var giga   = BigInteger.Pow(10,  9);
            var tera   = BigInteger.Pow(10, 12);
            var peta   = BigInteger.Pow(10, 15);
            var exa    = BigInteger.Pow(10, 18);
            var zetta  = BigInteger.Pow(10, 21);
            var yotta  = BigInteger.Pow(10, 24);
            var ronna  = BigInteger.Pow(10, 27);
            var quetta = BigInteger.Pow(10, 30);

            var kibi  = BigInteger.Pow(2,  10);
            var mebi  = BigInteger.Pow(2,  20);
            var gibi  = BigInteger.Pow(2,  30);
            var tebi  = BigInteger.Pow(2,  40);
            var pebi  = BigInteger.Pow(2,  50);
            var exbi  = BigInteger.Pow(2,  60);
            var zebi  = BigInteger.Pow(2,  70);
            var yobi  = BigInteger.Pow(2,  80);
            var robi  = BigInteger.Pow(2,  90);
            var quebi = BigInteger.Pow(2, 100);

            return type switch
            {
                DataUnit.Bit       => BigInteger.One,
                DataUnit.KiloBit   => kilo,
                DataUnit.MegaBit   => mega,
                DataUnit.GigaBit   => giga,
                DataUnit.TeraBit   => tera,
                DataUnit.PetaBit   => peta,
                DataUnit.ExaBit    => exa,
                DataUnit.ZettaBit  => zetta,
                DataUnit.YottaBit  => yotta,
                DataUnit.RonnaBit  => ronna,
                DataUnit.QuettaBit => quetta,

                DataUnit.KibiBit  => kibi,
                DataUnit.MebiBit  => mebi,
                DataUnit.GibiBit  => gibi,
                DataUnit.TebiBit  => tebi,
                DataUnit.PebiBit  => pebi,
                DataUnit.ExbiBit  => exbi,
                DataUnit.ZebiBit  => zebi,
                DataUnit.YobiBit  => yobi,
                DataUnit.RobiBit  => robi,
                DataUnit.QuebiBit => quebi,

                DataUnit.Byte       => bitsPerByte,
                DataUnit.KiloByte   => bitsPerByte * kilo,
                DataUnit.MegaByte   => bitsPerByte * mega,
                DataUnit.GigaByte   => bitsPerByte * giga,
                DataUnit.TeraByte   => bitsPerByte * tera,
                DataUnit.PetaByte   => bitsPerByte * peta,
                DataUnit.ExaByte    => bitsPerByte * exa,
                DataUnit.ZettaByte  => bitsPerByte * zetta,
                DataUnit.YottaByte  => bitsPerByte * yotta,
                DataUnit.RonnaByte  => bitsPerByte * ronna,
                DataUnit.QuettaByte => bitsPerByte * quetta,

                DataUnit.KibiByte  => bitsPerByte * kibi,
                DataUnit.MebiByte  => bitsPerByte * mebi,
                DataUnit.GibiByte  => bitsPerByte * gibi,
                DataUnit.TebiByte  => bitsPerByte * tebi,
                DataUnit.PebiByte  => bitsPerByte * pebi,
                DataUnit.ExbiByte  => bitsPerByte * exbi,
                DataUnit.ZebiByte  => bitsPerByte * zebi,
                DataUnit.YobiByte  => bitsPerByte * yobi,
                DataUnit.RobiByte  => bitsPerByte * robi,
                DataUnit.QuebiByte => bitsPerByte * quebi,

                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        internal static void GetDecimalAsFraction(decimal value, out BigInteger numerator, out BigInteger denominator)
        {
            int[] bits = decimal.GetBits(value);

            numerator = (uint)bits[0];
            numerator += (BigInteger)(uint)bits[1] << 32;
            numerator += (BigInteger)(uint)bits[2] << 64;

            int scale = (bits[3] >> 16) & 0xFF;
            denominator = BigInteger.Pow(10, scale);
        }

        #endregion
    }
}
