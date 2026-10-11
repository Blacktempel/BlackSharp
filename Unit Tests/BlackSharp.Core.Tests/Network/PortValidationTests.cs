/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Network;

namespace BlackSharp.Core.Tests.Network
{
    /// <summary>
    /// Verifies the boundaries and integer requirement of network port values.
    /// </summary>
    [TestClass]
    public sealed class PortValidationTests
    {
        #region Public

        /// <summary>
        /// Accepts only whole ports in the valid TCP and UDP range.
        /// </summary>
        [TestMethod]
        public void IsValidRejectsOutOfRangeAndFractionalPorts()
        {
            // Arrange
            var validPorts   = new[] { 1m, 28461m, 65535m };
            var invalidPorts = new[] { -1m, 0m, 1.5m, 65535.5m, 65536m };

            // Act
            var validResults   = validPorts  .Select(PortValidation.IsValid).ToArray();
            var invalidResults = invalidPorts.Select(PortValidation.IsValid).ToArray();

            // Assert
            Assert.IsTrue(validResults  .All(result =>  result));
            Assert.IsTrue(invalidResults.All(result => !result));
        }

        #endregion
    }
}
