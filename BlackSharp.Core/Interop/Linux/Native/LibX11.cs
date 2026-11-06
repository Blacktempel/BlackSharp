// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Native
{
    /// <summary>
    /// Provides X11 display and window-tree functions.
    /// </summary>
    public static class LibX11
    {
        #region Fields

        /// <summary>
        /// Names the system X11 library.
        /// </summary>
        private const string DLL_NAME = "libX11.so.6";

        #endregion

        #region Imports

        /// <summary>
        /// Closes an X11 display connection.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int XCloseDisplay(IntPtr display);

        /// <summary>
        /// Releases memory returned by an X11 function.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int XFree(IntPtr data);

        /// <summary>
        /// Sends queued requests to the X11 server.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int XFlush(IntPtr display);

        /// <summary>
        /// Opens the display selected by the current environment.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern IntPtr XOpenDisplay(IntPtr displayName);

        /// <summary>
        /// Retrieves the parent and children of an X11 window.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int XQueryTree(IntPtr display, IntPtr window, out IntPtr root, out IntPtr parent, out IntPtr children, out uint childCount);

        #endregion
    }
}
