// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace BlackSharp.Core.IO
{
    /// <summary>
    /// Opens, compares and extracts GZip-compressed resources from a specified assembly.
    /// </summary>
    public sealed class ResourceFileExtractor
    {
        #region Constructor

        /// <summary>
        /// Creates an extractor for an assembly and an optional resource-name prefix.
        /// </summary>
        /// <param name="assembly">The assembly containing the resources.</param>
        /// <param name="resourceNamePrefix">A prefix to try before the unmodified resource name.</param>
        public ResourceFileExtractor(Assembly assembly, string resourceNamePrefix = null)
        {
            _assembly           = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _resourceNamePrefix = resourceNamePrefix;
        }

        #endregion

        #region Fields

        private const int ValidateFileTimeoutMS = 2000;

        /// <summary>
        /// Contains the embedded compressed resources.
        /// </summary>
        private readonly Assembly _assembly;

        /// <summary>
        /// Provides an optional namespace prefix for resource lookup.
        /// </summary>
        private readonly string _resourceNamePrefix;

        #endregion

        #region Public

        /// <summary>
        /// Decompresses an embedded resource into an existing directory and verifies the resulting file length.
        /// </summary>
        /// <param name="filePath">The destination file path.</param>
        /// <param name="resourceFile">The embedded resource name.</param>
        /// <returns>
        /// <see langword="true"/> when the extracted file has the expected length; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ExtractResourceFileToFilePath(string filePath, string resourceFile)
        {
            var fullPath  = Path.GetFullPath(filePath);
            var directory = Path.GetDirectoryName(fullPath);

            long requiredLength = 0;

            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                return false;
            }

            try
            {
                using var gzipStream = GetResourceFileGZipStream(resourceFile);

                if (gzipStream == null)
                {
                    return false;
                }

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                using FileStream target = new(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);

                gzipStream.CopyTo(target);
                target.Flush(true);

                requiredLength = target.Length;
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                File.SetAttributes(fullPath, FileAttributes.Hidden);
            }
            catch
            {
                // Hidden attributes are not available on every platform.
            }

            if (ValidateUnzippedFile(fullPath, requiredLength))
            {
                return true;
            }

            // Allow a delayed file-system view to expose the completed write.
            var sw = new Stopwatch();

            sw.Start();

            while (sw.ElapsedMilliseconds < ValidateFileTimeoutMS)
            {
                if (ValidateUnzippedFile(fullPath, requiredLength))
                {
                    return true;
                }

                Thread.Yield();
            }

            return false;
        }

        /// <summary>
        /// Compares an existing file with the decompressed content of an embedded resource.
        /// </summary>
        /// <param name="filePath">The existing file path.</param>
        /// <param name="resourceFile">The embedded resource name.</param>
        /// <returns>
        /// <see langword="true"/> when both files contain the same bytes; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ResourceFileMatchesFilePath(string filePath, string resourceFile)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                using GZipStream resourceStream = GetResourceFileGZipStream(resourceFile);

                if (resourceStream == null)
                {
                    return false;
                }

                using var resourceContent = new MemoryStream();

                // Compare the decompressed embedded payload, not the gzip container metadata.
                resourceStream.CopyTo(resourceContent);

                var resourceBytes = resourceContent.ToArray();
                var fileBytes     = new byte[resourceBytes.Length];

                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

                // Length is a cheap rejection before allocating work to the byte comparison.
                if (fileStream.Length != resourceBytes.Length)
                {
                    return false;
                }

                int offset = 0;

                // Fill the comparison buffer completely, accounting for partial reads from the file stream.
                while (offset < fileBytes.Length)
                {
                    // FileStream.Read may legally return a partial buffer, even for local files.
                    var bytesRead = fileStream.Read(fileBytes, offset, fileBytes.Length - offset);

                    if (bytesRead == 0)
                    {
                        return false;
                    }

                    offset += bytesRead;
                }

                // Compare the decompressed embedded payload with the existing file.
                for (int i = 0; i < resourceBytes.Length; ++i)
                {
                    if (fileBytes[i] != resourceBytes[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Opens the GZip stream of an embedded resource.
        /// </summary>
        /// <param name="resourceFile">The embedded resource name.</param>
        /// <returns>The resource stream, or <see langword="null"/> when no matching resource exists.</returns>
        public GZipStream GetResourceFileGZipStream(string resourceFile)
        {
            return string.IsNullOrEmpty(_resourceNamePrefix)
                 ? _assembly.GetGZipResourceStream(resourceFile)
                 : _assembly.GetGZipResourceStream(_resourceNamePrefix + resourceFile, resourceFile);
        }

        #endregion

        #region Private

        /// <summary>
        /// Checks whether an extracted file exists with its expected size.
        /// </summary>
        /// <param name="filePath">The extracted file path.</param>
        /// <param name="requiredLength">The expected file length.</param>
        /// <returns>
        /// <see langword="true"/> when the file has the expected size; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool ValidateUnzippedFile(string filePath, long requiredLength)
        {
            try
            {
                return File.Exists(filePath) && new FileInfo(filePath).Length == requiredLength;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
