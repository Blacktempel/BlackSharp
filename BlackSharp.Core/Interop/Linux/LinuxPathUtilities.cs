/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using LibC = BlackSharp.Core.Interop.Linux.Native.LibC;
using OperatingSystem = BlackSharp.Core.Platform.OperatingSystem;
using System.Text;

namespace BlackSharp.Core.Interop.Linux
{
    /// <summary>
    /// Provides Linux-specific path and symbolic-link operations.
    /// </summary>
    public static class LinuxPathUtilities
    {
        #region Public

        /// <summary>
        /// Reads the target stored in a symbolic link.
        /// </summary>
        /// <param name="path">The symbolic-link path.</param>
        /// <returns>The stored link target, or an empty string when it cannot be read or the host is not Linux.</returns>
        public static string ResolveLink(string path)
        {
            if (!OperatingSystem.IsLinux() || string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var buffer = new byte[4096];
            var length = LibC.readlink(path, buffer, (UIntPtr)buffer.Length).ToInt64();

            return length > 0 && length < buffer.Length
                 ? Encoding.UTF8.GetString(buffer, 0, (int)length)
                 : string.Empty;
        }

        /// <summary>
        /// Resolves a path through its complete symbolic-link chain.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <returns>The resolved path, or the original path when it cannot be resolved.</returns>
        public static string ResolveRealPath(string path)
        {
            try
            {
                return string.IsNullOrWhiteSpace(path)
                     ? string.Empty
                     : ResolveSymbolicLinkChain(Path.GetFullPath(path), 0);
            }
            catch
            {
                return path ?? string.Empty;
            }
        }

        /// <summary>
        /// Searches parent directories for a named file or directory.
        /// </summary>
        /// <param name="startDirectory">The directory at which the search starts.</param>
        /// <param name="entryName">The file or directory name to find.</param>
        /// <param name="maximumDepth">The maximum number of directories to inspect.</param>
        /// <returns>The matching path or an empty string.</returns>
        public static string SearchUpwardsForEntry(string startDirectory, string entryName, int maximumDepth = 8)
        {
            var current = startDirectory;

            for (var depth = 0; depth < maximumDepth && !string.IsNullOrWhiteSpace(current); ++depth)
            {
                var candidate = Path.Combine(current, entryName);

                if (File.Exists(candidate) || Directory.Exists(candidate))
                {
                    return candidate;
                }

                current = Directory.GetParent(current)?.FullName;
            }

            return string.Empty;
        }

        #endregion

        #region Private

        static string ResolveSymbolicLinkChain(string path, int depth)
        {
            const int maximumLinkDepth = 16;

            if (depth > maximumLinkDepth || string.IsNullOrWhiteSpace(path))
            {
                return path ?? string.Empty;
            }

            var target = ResolveLink(path);

            if (string.IsNullOrWhiteSpace(target))
            {
                return path;
            }

            if (!Path.IsPathRooted(target))
            {
                target = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, target);
            }

            return ResolveSymbolicLinkChain(Path.GetFullPath(target), depth + 1);
        }

        #endregion
    }
}
