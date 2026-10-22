// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Creates a full exception string, containing <see cref="Exception.Message"/> and <see cref="Exception.StackTrace"/> of itself and recursively all InnerExceptions.
        /// </summary>
        /// <param name="e">This Exception.</param>
        /// <returns>Constructed exception string.</returns>
        public static string FullExceptionString(this Exception e)
        {
            if (e == null)
                return string.Empty;

            string exceptionString = FullExceptionString(e.InnerException);

            if (e.Message != null)
            {
                if (!string.IsNullOrEmpty(exceptionString))
                    exceptionString += Environment.NewLine + Environment.NewLine;

                exceptionString += "Info:" + Environment.NewLine;
                exceptionString += (e.HResult >= 0 ? $"({e.HResult}) " : $"(0x{e.HResult.ToString("X")}) ") + e.Message;
            }

            if (e.StackTrace != null)
            {
                if (!string.IsNullOrEmpty(exceptionString))
                    exceptionString += Environment.NewLine + Environment.NewLine;

                exceptionString += "Details:" + Environment.NewLine;
                exceptionString += e.StackTrace;
            }

            return exceptionString;
        }
    }
}
