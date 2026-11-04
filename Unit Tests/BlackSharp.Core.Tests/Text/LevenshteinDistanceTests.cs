// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Text;

namespace BlackSharp.Core.Tests.Text
{
    /// <summary>
    /// Verifies complete-string and closest-substring edit distances.
    /// </summary>
    [TestClass]
    public sealed class LevenshteinDistanceTests
    {
        #region Public

        /// <summary>
        /// Verifies the standard insertion, deletion, and substitution distance.
        /// </summary>
        [TestMethod]
        public void CalculateCountsStandardEdits()
        {
            // Arrange
            const string source = "kitten";
            const string target = "sitting";

            // Act
            var distance = LevenshteinDistance.Calculate(source, target);

            // Assert
            Assert.AreEqual(3, distance);
        }

        /// <summary>
        /// Verifies that adjacent swaps are optional in complete-string matching.
        /// </summary>
        [TestMethod]
        public void CalculateCanCountAdjacentSwapAsOneEdit()
        {
            // Arrange
            const string source = "ab";
            const string target = "ba";

            // Act
            var standard = LevenshteinDistance.Calculate(source, target);
            var withSwap = LevenshteinDistance.Calculate(source, target, true);

            // Assert
            Assert.AreEqual(2, standard);
            Assert.AreEqual(1, withSwap);
        }

        /// <summary>
        /// Verifies that the modified algorithm finds a swapped query inside longer text.
        /// </summary>
        [TestMethod]
        public void CalculateClosestSubstringFindsTypoInsideText()
        {
            // Arrange
            const string query = "memroy";
            const string text  = "system memory load";

            // Act
            var withSwap    = LevenshteinDistance.CalculateClosestSubstring(query, text);
            var withoutSwap = LevenshteinDistance.CalculateClosestSubstring(query, text, false);

            // Assert
            Assert.AreEqual(1, withSwap);
            Assert.AreEqual(2, withoutSwap);
        }

        /// <summary>
        /// Verifies empty input and exact substring distances.
        /// </summary>
        [TestMethod]
        public void CalculateHandlesEmptyAndExactInputs()
        {
            // Arrange
            const string text = "abc";

            // Act
            var emptySource     = LevenshteinDistance.Calculate(string.Empty, text        );
            var emptyTarget     = LevenshteinDistance.Calculate(text        , string.Empty);

            var emptyQuery      = LevenshteinDistance.CalculateClosestSubstring(string.Empty, text        );
            var emptySearchText = LevenshteinDistance.CalculateClosestSubstring(text        , string.Empty);
            var exactSubstring  = LevenshteinDistance.CalculateClosestSubstring("bc"        , text        );

            // Assert
            Assert.AreEqual(3, emptySource);
            Assert.AreEqual(3, emptyTarget);
            Assert.AreEqual(0, emptyQuery);
            Assert.AreEqual(3, emptySearchText);
            Assert.AreEqual(0, exactSubstring);
        }

        /// <summary>
        /// Verifies that a supplementary Unicode character counts as one character.
        /// </summary>
        [TestMethod]
        public void CalculateCountsUnicodeScalarValues()
        {
            // Arrange
            const string first  = "a😀b";
            const string second = "a🐱b";

            // Act
            var distance = LevenshteinDistance.Calculate(first, second);
            var substringDistance = LevenshteinDistance.CalculateClosestSubstring("😀", first);

            // Assert
            Assert.AreEqual(1, distance);
            Assert.AreEqual(0, substringDistance);
        }

        /// <summary>
        /// Verifies that both public methods reject null strings.
        /// </summary>
        [TestMethod]
        public void CalculateRejectsNullInputs()
        {
            // Arrange
            const string text = "value";

            // Act
            Action nullSource = () => LevenshteinDistance.Calculate(null, text);
            Action nullTarget = () => LevenshteinDistance.Calculate(text, null);

            Action nullQuery  = () => LevenshteinDistance.CalculateClosestSubstring(null, text);
            Action nullText   = () => LevenshteinDistance.CalculateClosestSubstring(text, null);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(nullSource);
            Assert.ThrowsExactly<ArgumentNullException>(nullTarget);
            Assert.ThrowsExactly<ArgumentNullException>(nullQuery );
            Assert.ThrowsExactly<ArgumentNullException>(nullText  );
        }

        #endregion
    }
}
