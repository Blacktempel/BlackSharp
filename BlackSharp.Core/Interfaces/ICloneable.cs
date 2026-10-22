// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Interfaces
{
    /// <inheritdoc cref="ICloneable"/>
    public interface ICloneable<T>
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        T Clone();
    }
}
