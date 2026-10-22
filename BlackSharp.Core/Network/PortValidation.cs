// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Network
{
    /// <summary>
    /// Validates numeric TCP and UDP port values.
    /// </summary>
    public static class PortValidation
    {
        #region Public

        /// <summary>
        /// Determines whether a value is an integral port in the usable range from 1 to 65535.
        /// </summary>
        /// <param name="port">The port value to validate.</param>
        /// <returns><see langword="true"/> when <paramref name="port"/> is a valid port.</returns>
        public static bool IsValid(decimal port)
        {
            return port >= 1 && port <= ushort.MaxValue && port == decimal.Truncate(port);
        }

        #endregion
    }
}
