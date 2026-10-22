// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.MVVM.Platform.Interfaces
{
    /// <summary>
    /// Defines a hot key.
    /// </summary>
    /// <typeparam name="TIdentifier">The enumeration type used to uniquely identify the hot key.</typeparam>
    public interface IHotKey<TIdentifier>
        where TIdentifier : Enum
    {
        /// <summary>
        /// Gets or sets the ID identifying the hot key.
        /// </summary>
        TIdentifier ID { get; set; }

        /// <summary>
        /// Gets or sets the key (e.g. F5), respectively in its integer code.
        /// </summary>
        int Key { get; set; }

        /// <summary>
        /// Gets or sets the modifiers (e.g. CTRL), respectively in its integer code.
        /// </summary>
        int Modifiers { get; set; }
    }
}
