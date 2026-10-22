// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a message retrieved from a native Windows message queue.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowMessage
    {
        #region Fields

        /// <summary>
        /// Contains the destination window handle.
        /// </summary>
        public IntPtr Window;

        /// <summary>
        /// Identifies the message.
        /// </summary>
        public uint Message;

        /// <summary>
        /// Contains the message-specific unsigned parameter.
        /// </summary>
        public UIntPtr WParam;

        /// <summary>
        /// Contains the message-specific signed parameter.
        /// </summary>
        public IntPtr LParam;

        /// <summary>
        /// Contains the time at which the message was posted.
        /// </summary>
        public uint Time;

        /// <summary>
        /// Contains the cursor position when the message was posted.
        /// </summary>
        public Win32Point Point;

        #endregion
    }
}
