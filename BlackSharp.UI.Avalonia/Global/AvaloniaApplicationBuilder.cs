/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using Avalonia;

namespace BlackSharp.UI.Avalonia
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
