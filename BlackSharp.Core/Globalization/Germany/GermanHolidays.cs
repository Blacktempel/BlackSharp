// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Globalization.Germany
{
    /// <summary>
    /// Provides nationwide and state-wide German holidays using the Gregorian calendar.
    /// </summary>
    /// <remarks>
    /// State applicability follows the modeled calendar rules. Municipality-specific observances
    /// and partially observed federal states require a more precise location.
    /// </remarks>
    public sealed class GermanHolidays
    {
        #region Constructor

        /// <summary>
        /// Initializes the nationwide and state-wide holidays for a Gregorian year.
        /// </summary>
        /// <param name="year">The calendar year.</param>
        /// <remarks>
        /// The list covers observances applying throughout Germany or an entire federal state.
        /// </remarks>
        public GermanHolidays(int year)
        {
            var easter = CalculateEasterSunday(year);

            Year = year;

            var holidays = new List<GermanHoliday>
            {
                new("Neujahr"                  , new DateTime(year, 1, 1)               , GermanState.Nationwide),
                new("Heilige Drei Könige"      , new DateTime(year, 1, 6)               , GermanState.BadenWuerttemberg, GermanState.Bavaria, GermanState.SaxonyAnhalt),
                new("Karfreitag"               , easter.AddDays(-2)                     , GermanState.Nationwide),
                new("Ostersonntag"             , easter                                 , GermanState.Brandenburg),
                new("Ostermontag"              , easter.AddDays(1)                      , GermanState.Nationwide),
                new("Tag der Arbeit"           , new DateTime(year, 5, 1)               , GermanState.Nationwide),
                new("Christi Himmelfahrt"      , easter.AddDays(39)                     , GermanState.Nationwide),
                new("Pfingstsonntag"           , easter.AddDays(49)                     , GermanState.Brandenburg),
                new("Pfingstmontag"            , easter.AddDays(50)                     , GermanState.Nationwide),
                new("Fronleichnam"             , easter.AddDays(60)                     , GermanState.BadenWuerttemberg, GermanState.Bavaria, GermanState.Hesse, GermanState.NorthRhineWestphalia, GermanState.RhinelandPalatinate, GermanState.Saarland),
                new("Mariä Himmelfahrt"        , new DateTime(year, 8, 15)              , GermanState.Saarland),
                new("Tag der Deutschen Einheit", new DateTime(year, 10, 3)              , GermanState.Nationwide),
                new("Allerheiligen"            , new DateTime(year, 11, 1)              , GermanState.BadenWuerttemberg, GermanState.Bavaria, GermanState.NorthRhineWestphalia, GermanState.RhinelandPalatinate, GermanState.Saarland),
                new("Buß- und Bettag"          , CalculateDayOfPrayerAndRepentance(year), GermanState.Saxony),
                new("1. Weihnachtsfeiertag"    , new DateTime(year, 12, 25)             , GermanState.Nationwide),
                new("2. Weihnachtsfeiertag"    , new DateTime(year, 12, 26)             , GermanState.Nationwide),
            };

            var reformationStates = year == 2017
                ? new List<GermanState> { GermanState.Nationwide }
                : new List<GermanState> { GermanState.Brandenburg, GermanState.MecklenburgWesternPomerania, GermanState.Saxony, GermanState.SaxonyAnhalt, GermanState.Thuringia };

            if (year >= 2018)
            {
                reformationStates.AddRange(new[] { GermanState.Bremen, GermanState.Hamburg, GermanState.LowerSaxony, GermanState.SchleswigHolstein });
            }

            holidays.Add(new GermanHoliday("Reformationstag", new DateTime(year, 10, 31), reformationStates.ToArray()));

            if (year >= 2019)
            {
                holidays.Add(new GermanHoliday("Weltkindertag", new DateTime(year, 9, 20), GermanState.Thuringia));
                holidays.Add(new GermanHoliday("Internationaler Frauentag", new DateTime(year, 3, 8),
                    year >= 2023
                        ? new[] { GermanState.Berlin, GermanState.MecklenburgWesternPomerania }
                        : new[] { GermanState.Berlin }));
            }

            Holidays = holidays.OrderBy(holiday => holiday.Date).ToList().AsReadOnly();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the represented calendar year.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Gets the nationwide and state-wide holidays of the year.
        /// </summary>
        public IReadOnlyList<GermanHoliday> Holidays { get; }

        #endregion

        #region Public

        /// <summary>
        /// Finds a statutory holiday on a date in a German federal state.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <param name="state">The federal state, or <see cref="GermanState.Nationwide"/> for nationwide holidays only.</param>
        /// <returns>The matching holiday, if any.</returns>
        public GermanHoliday GetHoliday(DateTime date, GermanState state) =>
            Holidays.FirstOrDefault(holiday => holiday.Date == date.Date
                && (holiday.States.Contains(GermanState.Nationwide) || holiday.States.Contains(state)));

        /// <summary>
        /// Checks whether a date is a statutory holiday in a German federal state.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <param name="state">The federal state.</param>
        /// <returns>Whether a holiday applies.</returns>
        public bool IsHoliday(DateTime date, GermanState state) => GetHoliday(date, state) != null;

        /// <summary>
        /// Gets the holiday dates observed throughout Germany or in a given federal state.
        /// </summary>
        /// <param name="state">The federal state, or <see cref="GermanState.Nationwide"/>.</param>
        /// <returns>The ordered holiday dates.</returns>
        public IReadOnlyList<DateTime> GetDatesForState(GermanState state) =>
            Holidays.Where(holiday => holiday.States.Contains(GermanState.Nationwide) || holiday.States.Contains(state))
                .Select(holiday => holiday.Date)
                .Distinct()
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Calculates Easter Sunday for a Gregorian calendar year.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The date of Easter Sunday.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The year is outside the supported date range.</exception>
        public static DateTime CalculateEasterSunday(int year)
        {
            if (year < 1 || year > 9999)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            // Gregorian computus (Meeus/Jones/Butcher), valid for the DateTime calendar range.
            var century              = year / 100;
            var yearInCentury        = year % 100;
            var yearInMetonicCycle   = year % 19;
            var leapCenturyCorrection = century / 4;
            var lunarCorrection      = (century + 8) / 25;
            var solarCorrection      = (century - lunarCorrection + 1) / 3;
            var epact                = (19 * yearInMetonicCycle + century - leapCenturyCorrection - solarCorrection + 15) % 30;
            var weekdayCorrection    = (32 + 2 * (century % 4) + 2 * (yearInCentury / 4) - epact - yearInCentury % 4) % 7;

            var leapMonthCorrection = (yearInMetonicCycle + 11 * epact + 22 * weekdayCorrection) / 451;
            var offset              = epact + weekdayCorrection - 7 * leapMonthCorrection + 114;

            return new DateTime(year, offset / 31, offset % 31 + 1);
        }

        /// <summary>
        /// Calculates Good Friday for a Gregorian calendar year.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The date of Good Friday.</returns>
        public static DateTime CalculateGoodFriday(int year) => CalculateEasterSunday(year).AddDays(-2);

        /// <summary>
        /// Calculates Easter Monday for a Gregorian calendar year.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The date of Easter Monday.</returns>
        public static DateTime CalculateEasterMonday(int year) => CalculateEasterSunday(year).AddDays(1);

        /// <summary>
        /// Calculates Ascension Day for a Gregorian calendar year.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The date of Ascension Day.</returns>
        public static DateTime CalculateAscensionDay(int year) => CalculateEasterSunday(year).AddDays(39);

        /// <summary>
        /// Calculates Whit Monday for a Gregorian calendar year.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The date of Whit Monday.</returns>
        public static DateTime CalculateWhitMonday(int year) => CalculateEasterSunday(year).AddDays(50);

        /// <summary>
        /// Calculates Corpus Christi for a Gregorian calendar year.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The date of Corpus Christi.</returns>
        public static DateTime CalculateCorpusChristi(int year) => CalculateEasterSunday(year).AddDays(60);

        /// <summary>
        /// Calculates the Wednesday of Prayer and Repentance in Saxony.
        /// </summary>
        /// <param name="year">The Gregorian calendar year.</param>
        /// <returns>The Wednesday preceding the last Sunday before Advent.</returns>
        public static DateTime CalculateDayOfPrayerAndRepentance(int year)
        {
            var november23 = new DateTime(year, 11, 23);
            var daysSinceWednesday = ((int)november23.DayOfWeek - (int)DayOfWeek.Wednesday + 6) % 7 + 1;

            return november23.AddDays(-daysSinceWednesday);
        }

        #endregion
    }
}
