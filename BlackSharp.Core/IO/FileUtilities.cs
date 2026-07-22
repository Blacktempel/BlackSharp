/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.IO
{
    /// <summary>
    /// Provides guarded file-system read operations.
    /// </summary>
    public static class FileUtilities
    {
        #region Public

        /// <summary>
        /// Reads and trims a text file, returning an empty string when the file cannot be read.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The trimmed file contents, or an empty string when reading fails.</returns>
        public static string ReadAllTextOrEmpty(string path)
        {
            try
            {
                return File.Exists(path)
                    ? File.ReadAllText(path).Trim()
                    : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
