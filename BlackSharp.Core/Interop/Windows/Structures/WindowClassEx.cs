// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes a native Windows window class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WindowClassEx
    {
        #region Fields

        /// <summary>
        /// Contains the structure size.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Contains window-class styles.
        /// </summary>
        public uint Style;

        /// <summary>
        /// Contains the window procedure.
        /// </summary>
        public WindowProcedure WindowProcedure;

        /// <summary>
        /// Contains the number of extra class bytes.
        /// </summary>
        public int ClassExtraBytes;

        /// <summary>
        /// Contains the number of extra window bytes.
        /// </summary>
        public int WindowExtraBytes;

        /// <summary>
        /// Contains the module instance handle.
        /// </summary>
        public IntPtr Instance;

        /// <summary>
        /// Contains the class icon handle.
        /// </summary>
        public IntPtr Icon;

        /// <summary>
        /// Contains the class cursor handle.
        /// </summary>
        public IntPtr Cursor;

        /// <summary>
        /// Contains the background brush handle.
        /// </summary>
        public IntPtr BackgroundBrush;

        /// <summary>
        /// Contains the menu resource name.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string MenuName;

        /// <summary>
        /// Contains the window class name.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ClassName;

        /// <summary>
        /// Contains the small class icon handle.
        /// </summary>
        public IntPtr SmallIcon;

        #endregion
    }
}
