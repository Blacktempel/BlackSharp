// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a method to copy the state from another instance of the same type.
    /// </summary>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Copies the values from the specified object to the current instance.
        /// </summary>
        /// <remarks>This shall copy value types only, not copying reference types at all.</remarks>
        void Copy(T from);
    }
}
