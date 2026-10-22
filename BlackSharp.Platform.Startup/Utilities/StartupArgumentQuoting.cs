// SPDX-License-Identifier: MPL-2.0

using System.Text;

namespace BlackSharp.Platform.Startup.Utilities
{
    /// <summary>
    /// Quotes arguments for Windows command lines, systemd directives and XDG desktop entries.
    /// </summary>
    public static class StartupArgumentQuoting
    {
        #region Public

        /// <summary>
        /// Quotes one argument according to Windows process command-line rules.
        /// </summary>
        /// <param name="value">The unescaped argument.</param>
        /// <returns>The argument safe for a Windows command line.</returns>
        public static string QuoteWindowsArgument(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length > 0 && !value.Any(character => char.IsWhiteSpace(character) || character == '"'))
            {
                return value;
            }

            var result = new StringBuilder(value.Length + 2);
            var backslashCount = 0;

            result.Append('"');

            foreach (var character in value)
            {
                if (character == '\\')
                {
                    ++backslashCount;
                    continue;
                }

                result.Append('\\', character == '"' ? backslashCount * 2 + 1 : backslashCount);
                result.Append(character);

                backslashCount = 0;
            }

            result.Append('\\', backslashCount * 2);
            result.Append('"');

            return result.ToString();
        }

        /// <summary>
        /// Quotes one value for a systemd unit directive.
        /// </summary>
        /// <param name="value">The unescaped value.</param>
        /// <param name="escapeEnvironmentVariables">Whether the directive expands environment variables.</param>
        /// <returns>The quoted directive value.</returns>
        public static string QuoteSystemdArgument(string value, bool escapeEnvironmentVariables = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Contains('\n') || value.Contains('\r'))
            {
                throw new ArgumentException("Systemd arguments cannot contain line breaks.", nameof(value));
            }

            var escaped = value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("%", "%%");

            if (escapeEnvironmentVariables)
            {
                escaped = escaped.Replace("$", "$$");
            }

            return "\"" + escaped + "\"";
        }

        /// <summary>
        /// Quotes one value for an XDG desktop-entry Exec field.
        /// </summary>
        /// <param name="value">The unescaped argument.</param>
        /// <returns>The quoted desktop-entry argument.</returns>
        public static string QuoteDesktopArgument(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("$", "\\$").Replace("`", "\\`").Replace("%", "%%") + "\"";
        }

        #endregion
    }
}
