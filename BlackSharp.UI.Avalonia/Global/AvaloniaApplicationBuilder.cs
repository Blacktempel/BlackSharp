// SPDX-License-Identifier: MPL-2.0

using Avalonia;

namespace BlackSharp.UI.Avalonia.Global
{
    /// <summary>
    /// Creates consistently configured Avalonia desktop application builders.
    /// </summary>
    public static class AvaloniaApplicationBuilder
    {
        #region Public

        /// <summary>
        /// Creates a platform-aware desktop builder with the Inter font and trace logging.
        /// </summary>
        /// <typeparam name="TApplication">The Avalonia application type.</typeparam>
        /// <returns>The configured application builder.</returns>
        public static AppBuilder Create<TApplication>()
            where TApplication : Application, new()
        {
            return AppBuilder.Configure<TApplication>()
                             .UsePlatformDetect()
                             .WithInterFont()
                             .LogToTrace();
        }

        #endregion
    }
}
