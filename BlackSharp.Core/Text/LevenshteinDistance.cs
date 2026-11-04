// SPDX-License-Identifier: MPL-2.0

using System.Buffers;

namespace BlackSharp.Core.Text
{
    /// <summary>
    /// Calculates case-sensitive edit distances between Unicode text values.
    /// </summary>
    public static class LevenshteinDistance
    {
        #region Public

        /// <summary>
        /// Counts edits between two complete strings.
        /// </summary>
        /// <param name="source">The first string.</param>
        /// <param name="target">The second string.</param>
        /// <param name="allowTranspositions">Whether swapping adjacent characters counts as one edit.</param>
        /// <returns>The number of insertions, deletions, substitutions, and permitted swaps.</returns>
        /// <exception cref="ArgumentNullException">A string is <see langword="null"/>.</exception>
        public static int Calculate(string source, string target, bool allowTranspositions = false)
        {
            return CalculateCore(source, target, allowTranspositions, false);
        }

        /// <summary>
        /// Counts edits between a query and the closest substring of another string.
        /// </summary>
        /// <param name="query">The query to match.</param>
        /// <param name="text">The text containing candidate substrings.</param>
        /// <param name="allowTranspositions">Whether swapping adjacent characters counts as one edit.</param>
        /// <returns>The lowest edit distance to any substring of <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException">A string is <see langword="null"/>.</exception>
        public static int CalculateClosestSubstring(string query, string text, bool allowTranspositions = true)
        {
            return CalculateCore(query, text, allowTranspositions, true);
        }

        #endregion

        #region Private

        /// <summary>
        /// Computes complete-string or closest-substring distance using three pooled matrix rows.
        /// </summary>
        /// <param name="source">The first string.</param>
        /// <param name="target">The second string.</param>
        /// <param name="allowTranspositions">Whether adjacent swaps count as one edit.</param>
        /// <param name="closestSubstring">Whether any substring of the target can match.</param>
        /// <returns>The calculated edit distance.</returns>
        private static int CalculateCore(string source, string target, bool allowTranspositions, bool closestSubstring)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var sourceCharacters = ToCodePoints(source);
            var targetCharacters = ToCodePoints(target);

            if (sourceCharacters.Length == 0)
            {
                return closestSubstring ? 0 : targetCharacters.Length;
            }

            if (targetCharacters.Length == 0)
            {
                return sourceCharacters.Length;
            }

            var rowLength      = targetCharacters.Length + 1;
            var beforePrevious = ArrayPool<int>.Shared.Rent(rowLength);
            var previous       = ArrayPool<int>.Shared.Rent(rowLength);
            var current        = ArrayPool<int>.Shared.Rent(rowLength);

            try
            {
                // Seed the first row with target-prefix lengths, or zero for substring starts.
                for (var targetIndex = 0; targetIndex < rowLength; ++targetIndex)
                {
                    previous[targetIndex] = closestSubstring ? 0 : targetIndex;
                }

                // Advance the matrix through each source character.
                for (var sourceIndex = 1; sourceIndex <= sourceCharacters.Length; ++sourceIndex)
                {
                    current[0] = sourceIndex;

                    // Compare the source prefix against every target prefix.
                    for (var targetIndex = 1; targetIndex <= targetCharacters.Length; ++targetIndex)
                    {
                        current[targetIndex] = Math.Min(
                            Math.Min(previous[targetIndex] + 1, current[targetIndex - 1] + 1),
                            previous[targetIndex - 1] +
                            (sourceCharacters[sourceIndex - 1] == targetCharacters[targetIndex - 1] ? 0 : 1));

                        if (allowTranspositions
                         && sourceIndex > 1
                         && targetIndex > 1
                         && sourceCharacters[sourceIndex - 1] == targetCharacters[targetIndex - 2]
                         && sourceCharacters[sourceIndex - 2] == targetCharacters[targetIndex - 1])
                        {
                            current[targetIndex] = Math.Min(current[targetIndex], beforePrevious[targetIndex - 2] + 1);
                        }
                    }

                    (beforePrevious, previous, current) = (previous, current, beforePrevious);
                }

                if (!closestSubstring)
                {
                    return previous[targetCharacters.Length];
                }

                var minimum = int.MaxValue;

                // Find the best target endpoint for the closest-substring result.
                for (var targetIndex = 0; targetIndex < rowLength; ++targetIndex)
                {
                    minimum = Math.Min(minimum, previous[targetIndex]);
                }

                return minimum;
            }
            finally
            {
                ArrayPool<int>.Shared.Return(beforePrevious);
                ArrayPool<int>.Shared.Return(previous);
                ArrayPool<int>.Shared.Return(current);
            }
        }

        /// <summary>
        /// Converts UTF-16 text into Unicode scalar values.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The code points represented by the text.</returns>
        private static int[] ToCodePoints(string text)
        {
            var characters = new List<int>(text.Length);

            // Combine valid surrogate pairs and keep other UTF-16 characters as individual values.
            for (var index = 0; index < text.Length; ++index)
            {
                if (char.IsHighSurrogate(text[index])
                 && index + 1 < text.Length
                 && char.IsLowSurrogate(text[index + 1]))
                {
                    characters.Add(char.ConvertToUtf32(text[index], text[index + 1]));
                    ++index;
                }
                else
                {
                    characters.Add(text[index]);
                }
            }

            return characters.ToArray();
        }

        #endregion
    }
}
