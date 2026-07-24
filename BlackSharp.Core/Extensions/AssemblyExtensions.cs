/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.IO.Compression;
using System.Reflection;

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for assemblies.
    /// </summary>
    public static class AssemblyExtensions
    {
        #region Public

        /// <summary>
        /// Opens the first available embedded GZip resource from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the resource.</param>
        /// <param name="resourceNames">The resource names to try in order.</param>
        /// <returns>
        /// A decompression stream for the first available resource, or <see langword="null"/> when no resource
        /// can be opened.
        /// </returns>
        public static GZipStream GetGZipResourceStream(
            this Assembly assembly,
            params string[] resourceNames)
        {
            if (assembly == null || resourceNames == null)
            {
                return null;
            }

            try
            {
                for (int index = 0; index < resourceNames.Length; ++index)
                {
                    var resourceName = resourceNames[index];

                    if (string.IsNullOrEmpty(resourceName))
                    {
                        continue;
                    }

                    var stream = assembly.GetManifestResourceStream(resourceName);

                    if (stream != null)
                    {
                        return new GZipStream(stream, CompressionMode.Decompress);
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        #endregion
    }
}
