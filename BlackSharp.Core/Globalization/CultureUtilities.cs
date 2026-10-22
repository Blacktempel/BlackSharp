// SPDX-License-Identifier: MPL-2.0

using System.Globalization;

namespace BlackSharp.Core.Globalization
{
    /// <summary>
    /// Applies a culture to the current thread and to threads created afterward.
    /// </summary>
    public static class CultureUtilities
    {
        #region Public

        /// <summary>
        /// Sets the formatting and resource cultures for the current thread and future threads.
        /// </summary>
        /// <param name="culture">The culture to apply.</param>
        public static void SetCurrent(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            CultureInfo.CurrentCulture                = culture;
            CultureInfo.CurrentUICulture              = culture;
            CultureInfo.DefaultThreadCurrentCulture   = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        #endregion
    }
}
