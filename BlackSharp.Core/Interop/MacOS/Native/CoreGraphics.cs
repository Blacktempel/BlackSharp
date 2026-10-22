// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;
using BlackSharp.Core.Interop.MacOS.Structures;

namespace BlackSharp.Core.Interop.MacOS.Native
{
    /// <summary>
    /// Imports display-enumeration functions from the macOS CoreGraphics framework.
    /// </summary>
    public static class CoreGraphics
    {
        #region Fields

        /// <summary>
        /// Carries the core graphics value exchanged with the native API.
        /// </summary>
        private const string DLL_NAME = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";

        #endregion

        #region Imports

        /// <summary>
        /// Invokes the native <c>CGDisplayCopyAllDisplayModes</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <param name="options">The options that control the operation.</param>
        /// <returns>The value produced by CG display copy all display modes.</returns>
        [DllImport(DLL_NAME)]
        public static extern IntPtr CGDisplayCopyAllDisplayModes(uint display, IntPtr options);

        /// <summary>
        /// Invokes the native <c>CGDisplayCopyDisplayMode</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display copy display mode.</returns>
        [DllImport(DLL_NAME)]
        public static extern IntPtr CGDisplayCopyDisplayMode(uint display);

        /// <summary>
        /// Invokes the native <c>CGDisplayIsBuiltin</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display is builtin.</returns>
        [DllImport(DLL_NAME)]
        public static extern int CGDisplayIsBuiltin(uint display);

        /// <summary>
        /// Invokes the native <c>CGDisplayIsMain</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display is main.</returns>
        [DllImport(DLL_NAME)]
        public static extern int CGDisplayIsMain(uint display);

        /// <summary>
        /// Invokes the native <c>CGDisplayModeGetHeight</c> function.
        /// </summary>
        /// <param name="mode">The mode used by the operation.</param>
        /// <returns>The value produced by CG display mode get height.</returns>
        [DllImport(DLL_NAME)]
        public static extern IntPtr CGDisplayModeGetHeight(IntPtr mode);

        /// <summary>
        /// Invokes the native <c>CGDisplayModeGetRefreshRate</c> function.
        /// </summary>
        /// <param name="mode">The mode used by the operation.</param>
        /// <returns>The value produced by CG display mode get refresh rate.</returns>
        [DllImport(DLL_NAME)]
        public static extern double CGDisplayModeGetRefreshRate(IntPtr mode);

        /// <summary>
        /// Invokes the native <c>CGDisplayModeGetWidth</c> function.
        /// </summary>
        /// <param name="mode">The mode used by the operation.</param>
        /// <returns>The value produced by CG display mode get width.</returns>
        [DllImport(DLL_NAME)]
        public static extern IntPtr CGDisplayModeGetWidth(IntPtr mode);

        /// <summary>
        /// Invokes the native <c>CGDisplayModelNumber</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display model number.</returns>
        [DllImport(DLL_NAME)]
        public static extern uint CGDisplayModelNumber(uint display);

        /// <summary>
        /// Invokes the native <c>CGDisplayScreenSize</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display screen size.</returns>
        [DllImport(DLL_NAME)]
        public static extern CGSize CGDisplayScreenSize(uint display);

        /// <summary>
        /// Invokes the native <c>CGDisplaySerialNumber</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display serial number.</returns>
        [DllImport(DLL_NAME)]
        public static extern uint CGDisplaySerialNumber(uint display);

        /// <summary>
        /// Invokes the native <c>CGDisplayVendorNumber</c> function.
        /// </summary>
        /// <param name="display">The display used by the operation.</param>
        /// <returns>The value produced by CG display vendor number.</returns>
        [DllImport(DLL_NAME)]
        public static extern uint CGDisplayVendorNumber(uint display);

        /// <summary>
        /// Invokes the native <c>CGGetActiveDisplayList</c> function.
        /// </summary>
        /// <param name="maximumDisplays">The maximum displays used by the operation.</param>
        /// <param name="activeDisplays">The active displays used by the operation.</param>
        /// <param name="displayCount">When this method returns, contains the display count result.</param>
        /// <returns>The value produced by CG get active display list.</returns>
        [DllImport(DLL_NAME)]
        public static extern int CGGetActiveDisplayList(uint maximumDisplays, [Out] uint[] activeDisplays, out uint displayCount);

        #endregion
    }
}
