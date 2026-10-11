/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Logging
{
    /// <summary>
    /// Defines a contract for logging diagnostic events.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Records a diagnostic event.
        /// </summary>
        /// <param name="level">The severity of the event.</param>
        /// <param name="source">The component or operation that produced the event.</param>
        /// <param name="message">A description of the event.</param>
        /// <param name="exception">An optional exception associated with the event.</param>
        void Log(LogLevel level, string source, string message, Exception exception = null);
    }
}
