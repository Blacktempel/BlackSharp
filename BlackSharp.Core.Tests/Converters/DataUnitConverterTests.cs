/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using BlackSharp.Core.Converters;
using BlackSharp.Core.Converters.Enums;
using System.Numerics;

namespace BlackSharp.Core.Tests.Utilities
{
    [TestClass]
    public class DataUnitConverterTests
    {
        #region Public

        [TestMethod]
        [DynamicData(nameof(GetAllConversionCases))]
        public void Convert_AllUnitCombinations_ReturnsExpectedValue(ulong value, DataUnit sourceType, DataUnit targetType, bool expectOverflow, decimal expected)
        {
            if (expectOverflow)
            {
                Assert.ThrowsExactly<OverflowException>(() => DataUnitConverter.Convert(value, sourceType, targetType));
                return;
            }

            Assert.AreEqual(expected, DataUnitConverter.Convert(value, sourceType, targetType));
        }

        [TestMethod]
        public void Convert_SiAndIecReferenceValues_AreCorrect()
        {
            Assert.AreEqual(1M, DataUnitConverter.Convert(1_000, DataUnit.Byte, DataUnit.KiloByte));
            Assert.AreEqual(1M, DataUnitConverter.Convert(1_024, DataUnit.Byte, DataUnit.KibiByte));

            Assert.AreEqual(1.024M, DataUnitConverter.Convert(1, DataUnit.KibiByte, DataUnit.KiloByte));
            Assert.AreEqual(0.9765625M, DataUnitConverter.Convert(1, DataUnit.KiloByte, DataUnit.KibiByte));

            Assert.AreEqual(8M, DataUnitConverter.Convert(1, DataUnit.Byte, DataUnit.Bit));
            Assert.AreEqual(1_048_576M, DataUnitConverter.Convert(1, DataUnit.MebiByte, DataUnit.Byte));
        }

        [TestMethod]
        public void ShortcutMethods_AllTargets_MatchConvertResult()
        {
            const ulong value = 1_000_000;

            var targetMethods = new (DataUnit TargetType, Func<ulong, DataUnit, decimal> Method)[]
            {
                (DataUnit.Byte      , DataUnitConverter.ToByte      ),
                (DataUnit.KiloByte  , DataUnitConverter.ToKiloByte  ),
                (DataUnit.MegaByte  , DataUnitConverter.ToMegaByte  ),
                (DataUnit.GigaByte  , DataUnitConverter.ToGigaByte  ),
                (DataUnit.TeraByte  , DataUnitConverter.ToTeraByte  ),
                (DataUnit.PetaByte  , DataUnitConverter.ToPetaByte  ),
                (DataUnit.ExaByte   , DataUnitConverter.ToExaByte   ),
                (DataUnit.ZettaByte , DataUnitConverter.ToZettaByte ),
                (DataUnit.YottaByte , DataUnitConverter.ToYottaByte ),
                (DataUnit.RonnaByte , DataUnitConverter.ToRonnaByte ),
                (DataUnit.QuettaByte, DataUnitConverter.ToQuettaByte),
                (DataUnit.KibiByte  , DataUnitConverter.ToKibiByte  ),
                (DataUnit.MebiByte  , DataUnitConverter.ToMebiByte  ),
                (DataUnit.GibiByte  , DataUnitConverter.ToGibiByte  ),
                (DataUnit.TebiByte  , DataUnitConverter.ToTebiByte  ),
                (DataUnit.PebiByte  , DataUnitConverter.ToPebiByte  ),
                (DataUnit.ExbiByte  , DataUnitConverter.ToExbiByte  ),
                (DataUnit.ZebiByte  , DataUnitConverter.ToZebiByte  ),
                (DataUnit.YobiByte  , DataUnitConverter.ToYobiByte  ),
                (DataUnit.RobiByte  , DataUnitConverter.ToRobiByte  ),
                (DataUnit.QuebiByte , DataUnitConverter.ToQuebiByte ),
                (DataUnit.Bit       , DataUnitConverter.ToBit       ),
                (DataUnit.KiloBit   , DataUnitConverter.ToKiloBit   ),
                (DataUnit.MegaBit   , DataUnitConverter.ToMegaBit   ),
                (DataUnit.GigaBit   , DataUnitConverter.ToGigaBit   ),
                (DataUnit.TeraBit   , DataUnitConverter.ToTeraBit   ),
                (DataUnit.PetaBit   , DataUnitConverter.ToPetaBit   ),
                (DataUnit.ExaBit    , DataUnitConverter.ToExaBit    ),
                (DataUnit.ZettaBit  , DataUnitConverter.ToZettaBit  ),
                (DataUnit.YottaBit  , DataUnitConverter.ToYottaBit  ),
                (DataUnit.RonnaBit  , DataUnitConverter.ToRonnaBit  ),
                (DataUnit.QuettaBit , DataUnitConverter.ToQuettaBit ),
                (DataUnit.KibiBit   , DataUnitConverter.ToKibiBit   ),
                (DataUnit.MebiBit   , DataUnitConverter.ToMebiBit   ),
                (DataUnit.GibiBit   , DataUnitConverter.ToGibiBit   ),
                (DataUnit.TebiBit   , DataUnitConverter.ToTebiBit   ),
                (DataUnit.PebiBit   , DataUnitConverter.ToPebiBit   ),
                (DataUnit.ExbiBit   , DataUnitConverter.ToExbiBit   ),
                (DataUnit.ZebiBit   , DataUnitConverter.ToZebiBit   ),
                (DataUnit.YobiBit   , DataUnitConverter.ToYobiBit   ),
                (DataUnit.RobiBit   , DataUnitConverter.ToRobiBit   ),
                (DataUnit.QuebiBit  , DataUnitConverter.ToQuebiBit  ),
            };

            foreach (var sourceType in Enum.GetValues<DataUnit>())
            {
                foreach (var (targetType, method) in targetMethods)
                {
                    try
                    {
                        var expected = DataUnitConverter.Convert(value, sourceType, targetType);
                        var actual = method(value, sourceType);

                        Assert.AreEqual(expected, actual, $"Unexpected result for source '{sourceType}' and target '{targetType}'.");
                    }
                    catch (OverflowException)
                    {
                        Assert.ThrowsExactly<OverflowException>(() => method(value, sourceType));
                    }
                }
            }
        }

        [TestMethod]
        public void Convert_Overflow_ThrowsOverflowException()
        {
            Assert.ThrowsExactly<OverflowException>(() =>
                DataUnitConverter.Convert(ulong.MaxValue, DataUnit.TebiByte, DataUnit.Bit));
        }

        [TestMethod]
        public void Convert_Zero_ReturnsZero()
        {
            foreach (var sourceType in Enum.GetValues<DataUnit>())
            {
                foreach (var targetType in Enum.GetValues<DataUnit>())
                {
                    Assert.AreEqual(0M, DataUnitConverter.Convert(0, sourceType, targetType));
                }
            }
        }

        public static IEnumerable<object[]> GetAllConversionCases()
        {
            const ulong value = 1_000_000;

            foreach (var sourceType in Enum.GetValues<DataUnit>())
            {
                foreach (var targetType in Enum.GetValues<DataUnit>())
                {
                    if (TryCalculateExpected(value, sourceType, targetType, out var expected))
                    {
                        yield return [value, sourceType, targetType, false, expected];
                    }
                    else
                    {
                        yield return [value, sourceType, targetType, true, 0M];
                    }
                }
            }
        }

        #endregion

        #region Private

        static bool TryCalculateExpected(ulong value, DataUnit sourceType, DataUnit targetType, out decimal expected)
        {
            expected = 0M;

            try
            {
                expected = CalculateExpected(value, sourceType, targetType);
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        static decimal CalculateExpected(ulong value, DataUnit sourceType, DataUnit targetType)
        {
            if (sourceType == targetType)
            {
                return value;
            }

            var sourceFactor = DataUnitConverter.GetFactorInBits(sourceType);
            var targetFactor = DataUnitConverter.GetFactorInBits(targetType);

            var numerator = (BigInteger)value * sourceFactor;

            var integralPart = BigInteger.DivRem(numerator, targetFactor, out var remainder);

            var integralDecimal = (decimal)integralPart;

            if (remainder.IsZero)
            {
                return integralDecimal;
            }

            return integralDecimal + (decimal)remainder / (decimal)targetFactor;
        }

        #endregion
    }
}
