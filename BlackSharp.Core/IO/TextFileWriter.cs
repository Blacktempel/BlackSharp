/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Text;

namespace BlackSharp.Core.IO
{
    /// <summary>
    /// Writes text using CRLF line endings and UTF-8 with a byte-order mark.
    /// </summary>
    public static class TextFileWriter
    {
        #region Fields

        /// <summary>
        /// Provides the required UTF-8 encoding with a byte-order mark.
        /// </summary>
        private static readonly Encoding s_UTF8WithBom = new UTF8Encoding(true);

        #endregion

        #region Public

        /// <summary>
        /// Encodes text with CRLF line endings and a UTF-8 byte-order mark.
        /// </summary>
        /// <param name="contents">The text to encode.</param>
        /// <returns>The encoded bytes.</returns>
        public static byte[] Encode(string contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            var normalizedContents = contents.Replace("\r\n", "\n")
                                             .Replace('\r', '\n')
                                             .Replace("\n", "\r\n");

            var preamble        = s_UTF8WithBom.GetPreamble();
            var encodedContents = s_UTF8WithBom.GetBytes(normalizedContents);

            var result = new byte[preamble.Length + encodedContents.Length];

            Buffer.BlockCopy(preamble       , 0, result, 0              , preamble       .Length);
            Buffer.BlockCopy(encodedContents, 0, result, preamble.Length, encodedContents.Length);

            return result;
        }

        /// <summary>
        /// Writes text with CRLF line endings and a UTF-8 byte-order mark.
        /// </summary>
        /// <param name="file">The destination path.</param>
        /// <param name="contents">The text to write.</param>
        public static void WriteAllText(string file, string contents)
        {
            File.WriteAllBytes(file, Encode(contents));
        }

        #endregion
    }
}
