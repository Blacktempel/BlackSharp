// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Globalization.Germany;

namespace BlackSharp.Core.Tests.Globalization.Germany
{
    /// <summary>
    /// Verifies Gregorian holiday dates and state-wide holiday lookups.
    /// </summary>
    [TestClass]
    public sealed class GermanHolidaysTests
    {
        #region Public

        /// <summary>
        /// Verifies Easter and its related movable holidays across different years.
        /// </summary>
        [TestMethod]
        public void MovableHolidaysFollowGregorianCalendar()
        {
            // Arrange
            var easter2024 = new DateTime(2024, 3, 31);
            var easter2026 = new DateTime(2026, 4, 5);

            // Act
            var computed2024 = GermanHolidays.CalculateEasterSunday(2024);
            var computed2026 = GermanHolidays.CalculateEasterSunday(2026);

            // Assert
            Assert.AreEqual(easter2024, computed2024);
            Assert.AreEqual(easter2026, computed2026);
            Assert.AreEqual(easter2026.AddDays(-2), GermanHolidays.CalculateGoodFriday(2026));
            Assert.AreEqual(easter2026.AddDays( 1), GermanHolidays.CalculateEasterMonday(2026));
            Assert.AreEqual(easter2026.AddDays(39), GermanHolidays.CalculateAscensionDay(2026));
            Assert.AreEqual(easter2026.AddDays(50), GermanHolidays.CalculateWhitMonday(2026));
            Assert.AreEqual(easter2026.AddDays(60), GermanHolidays.CalculateCorpusChristi(2026));
        }

        /// <summary>
        /// Verifies that state-specific days are resolved alongside nationwide holidays.
        /// </summary>
        [TestMethod]
        public void StateLookupsIncludeNationwideHolidays()
        {
            // Arrange
            var calendar = new GermanHolidays(2026);

            // Act
            var berlinEpiphany   = calendar.GetHoliday(new DateTime(2026,  1,  6), GermanState.Berlin );
            var bavarianEpiphany = calendar.GetHoliday(new DateTime(2026,  1,  6), GermanState.Bavaria);
            var berlinChristmas  = calendar.GetHoliday(new DateTime(2026, 12, 25), GermanState.Berlin );

            // Assert
            Assert.IsNull(berlinEpiphany);

            Assert.AreEqual("Heilige Drei Könige", bavarianEpiphany?.Name);
            Assert.AreEqual("1. Weihnachtsfeiertag", berlinChristmas?.Name);
            Assert.AreEqual(new DateTime(2026, 11, 18), GermanHolidays.CalculateDayOfPrayerAndRepentance(2026));

            Assert.IsTrue(calendar.IsHoliday(new DateTime(2026, 11, 18), GermanState.Saxony));
            Assert.IsFalse(calendar.IsHoliday(new DateTime(2026, 11, 18), GermanState.Berlin));
        }

        #endregion
    }
}
