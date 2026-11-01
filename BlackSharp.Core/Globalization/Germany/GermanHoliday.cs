// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Globalization.Germany
{
    /// <summary>
    /// Describes a German statutory holiday and the states in which it is observed.
    /// </summary>
    public sealed class GermanHoliday
    {
        #region Constructor

        /// <summary>
        /// Initializes a statutory holiday.
        /// </summary>
        /// <param name="name">The holiday's German name.</param>
        /// <param name="date">Its calendar date.</param>
        /// <param name="states">The observing states, or <see cref="GermanState.Nationwide"/>.</param>
        public GermanHoliday(string name, DateTime date, params GermanState[] states)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Date = date.Date;

            States = Array.AsReadOnly((GermanState[])(states ?? throw new ArgumentNullException(nameof(states))).Clone());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the holiday's German name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the holiday's calendar date.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Gets the federal states observing the holiday.
        /// </summary>
        public IReadOnlyList<GermanState> States { get; }

        #endregion
    }
}
