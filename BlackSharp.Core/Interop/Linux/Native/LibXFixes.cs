// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Linux.Native
{
    /// <summary>
    /// Provides XFixes region and input-shape functions.
    /// </summary>
    public static class LibXFixes
    {
        #region Fields

        /// <summary>
        /// Names the system XFixes library.
        /// </summary>
        private const string DLL_NAME = "libXfixes.so.3";

        /// <summary>
        /// Selects the pointer-input shape of an X11 window.
        /// </summary>
        public const int ShapeInput = 2;

        #endregion

        #region Imports

        /// <summary>
        /// Creates an XFixes region from rectangles.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern IntPtr XFixesCreateRegion(IntPtr display, IntPtr rectangles, int rectangleCount);

        /// <summary>
        /// Destroys an XFixes region.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern void XFixesDestroyRegion(IntPtr display, IntPtr region);

        /// <summary>
        /// Checks whether the XFixes extension is available on the display.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int XFixesQueryExtension(IntPtr display, out int eventBase, out int errorBase);

        /// <summary>
        /// Sets the input shape of an X11 window.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern void XFixesSetWindowShapeRegion(IntPtr display, IntPtr window, int shapeKind, int xOffset, int yOffset, IntPtr region);

        #endregion
    }
}
