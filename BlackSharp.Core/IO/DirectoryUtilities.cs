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
    /// Provides reusable directory operations.
    /// </summary>
    public static class DirectoryUtilities
    {
        #region Public

        /// <summary>
        /// Recursively copies a directory and optionally preserves file metadata.
        /// </summary>
        /// <param name="sourceDirectory">The directory to copy.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        /// <param name="cancellationToken">The token used to cancel recursive copying.</param>
        /// <param name="preserveFileMetadata">Whether file attributes, timestamps, and supported Unix modes are copied.</param>
        /// <exception cref="ArgumentException">
        /// A path is missing, or <paramref name="targetDirectory"/> is equal to or below <paramref name="sourceDirectory"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="sourceDirectory"/> does not exist.</exception>
        public static void Copy(
            string sourceDirectory,
            string targetDirectory,
            CancellationToken cancellationToken = default,
            bool preserveFileMetadata = false)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                throw new ArgumentException("A source directory is required.", nameof(sourceDirectory));
            }

            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                throw new ArgumentException("A target directory is required.", nameof(targetDirectory));
            }

            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException(sourceDirectory);
            }

            if (PathUtilities.IsUnderDirectory(targetDirectory, sourceDirectory, includeDirectory: true))
            {
                throw new ArgumentException("The target directory must be outside the source directory.", nameof(targetDirectory));
            }

            Directory.CreateDirectory(targetDirectory);

            foreach (var directory in Directory.EnumerateDirectories(sourceDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                Copy(
                    directory,
                    Path.Combine(targetDirectory, Path.GetFileName(directory)),
                    cancellationToken,
                    preserveFileMetadata);
            }

            foreach (var file in Directory.EnumerateFiles(sourceDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var destination = Path.Combine(targetDirectory, Path.GetFileName(file));

                File.Copy(file, destination, false);

                if (!preserveFileMetadata)
                {
                    continue;
                }

                File.SetAttributes(destination, File.GetAttributes(file));
                File.SetLastWriteTimeUtc(destination, File.GetLastWriteTimeUtc(file));

#if NET6_0_OR_GREATER
                if (!System.OperatingSystem.IsWindows())
                {
                    try
                    {
                        File.SetUnixFileMode(destination, File.GetUnixFileMode(file));
                    }
                    catch (PlatformNotSupportedException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
#endif
            }
        }

        #endregion
    }
}
