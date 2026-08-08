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
    /// Provides normalized path comparison operations.
    /// </summary>
    public static class PathUtilities
    {
        #region Public

        /// <summary>
        /// Returns whether a path is located below a directory, optionally including the directory itself.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <param name="directory">The potential parent directory.</param>
        /// <param name="includeDirectory">Whether the directory itself is accepted.</param>
        /// <returns><see langword="true"/> when the path is within the directory.</returns>
        public static bool IsUnderDirectory(string path, string directory, bool includeDirectory = false)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            var fullPath = Normalize(path);
            var fullDirectory = Normalize(directory);

            var comparison = Platform.OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            return includeDirectory && string.Equals(fullPath, fullDirectory, comparison)
                || fullPath.StartsWith(fullDirectory + Path.DirectorySeparatorChar, comparison);
        }

        #endregion

        #region Private

        static string Normalize(string path)
        {
            return Path.GetFullPath(path).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
        }

        #endregion
    }
}
