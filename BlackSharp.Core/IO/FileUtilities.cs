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

        /// <summary>
        /// Returns matching files in ordinal order, or an empty array when the directory cannot be enumerated.
        /// </summary>
        /// <param name="path">The directory to enumerate.</param>
        /// <param name="searchPattern">The file search pattern.</param>
        /// <returns>The matching file paths.</returns>
        public static string[] GetFilesOrEmpty(string path, string searchPattern = "*")
        {
            try
            {
                return Directory.Exists(path)
                     ? Directory.EnumerateFiles(path, searchPattern)
                         .OrderBy(item => item, StringComparer.Ordinal)
                         .ToArray()
                     : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Returns matching directories in ordinal order, or an empty array when the directory cannot be enumerated.
        /// </summary>
        /// <param name="path">The directory to enumerate.</param>
        /// <param name="searchPattern">The directory search pattern.</param>
        /// <returns>The matching directory paths.</returns>
        public static string[] GetDirectoriesOrEmpty(string path, string searchPattern = "*")
        {
            try
            {
                return Directory.Exists(path)
                     ? Directory.EnumerateDirectories(path, searchPattern)
                         .OrderBy(item => item, StringComparer.Ordinal)
                         .ToArray()
                     : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Attempts to write an integer using the invariant culture.
        /// </summary>
        /// <param name="path">The existing file to write.</param>
        /// <param name="value">The integer value.</param>
        /// <returns><see langword="true"/> when the value was written.</returns>
        public static bool TryWriteInteger(string path, int value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    return false;
                }

                File.WriteAllText(path, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to read a finite floating-point value using the invariant culture.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <param name="value">Receives the parsed value.</param>
        /// <returns><see langword="true"/> when a finite value was read.</returns>
        public static bool TryReadDouble(string path, out double value)
        {
            value = 0;

            return double.TryParse(
                       ReadAllTextOrEmpty(path),
                       System.Globalization.NumberStyles.Float,
                       System.Globalization.CultureInfo.InvariantCulture,
                       out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value);
        }

        /// <summary>
        /// Writes text through a uniquely named sibling file and atomically replaces the destination where supported.
        /// </summary>
        /// <param name="path">The destination file path.</param>
        /// <param name="contents">The text to write.</param>
        public static void WriteAllTextAtomic(string path, string contents)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A destination path is required.", nameof(path));
            }

            var fullPath      = Path.GetFullPath(path);
            var temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                File.WriteAllText(temporaryPath, contents ?? string.Empty);

#if NETSTANDARD2_0 || NETFRAMEWORK
                if (File.Exists(fullPath))
                {
                    File.Replace(temporaryPath, fullPath, null);
                }
                else
                {
                    File.Move(temporaryPath, fullPath);
                }
#else
                File.Move(temporaryPath, fullPath, true);
#endif
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        #endregion
    }
}
