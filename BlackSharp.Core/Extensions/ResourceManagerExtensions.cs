// SPDX-License-Identifier: MPL-2.0

using System.Globalization;
using System.Resources;

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Provides guarded resource lookup operations.
    /// </summary>
    public static class ResourceManagerExtensions
    {
        #region Public

        /// <summary>
        /// Gets a localized resource value and returns its key when the resource is missing.
        /// </summary>
        /// <param name="resourceManager">The resource manager to query.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="culture">The culture used for the lookup.</param>
        /// <returns>The localized value or <paramref name="resourceKey"/>.</returns>
        public static string GetStringOrKey(this ResourceManager resourceManager, string resourceKey, CultureInfo culture)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }

            if (string.IsNullOrEmpty(resourceKey))
            {
                return string.Empty;
            }

            return resourceManager.GetString(resourceKey, culture) ?? resourceKey;
        }

        #endregion
    }
}
