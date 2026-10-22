// SPDX-License-Identifier: MPL-2.0

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
