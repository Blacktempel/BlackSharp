// SPDX-License-Identifier: MPL-2.0

using Avalonia.Controls;
using BlackSharp.Core.Interop.Linux.Native;
using System.Runtime.InteropServices;
using WindowsUser32 = BlackSharp.Core.Interop.Windows.Native.User32;

namespace BlackSharp.UI.Avalonia.Controller
{
    /// <summary>
    /// Changes whether a desktop window receives pointer input on Windows or X11.
    /// </summary>
    /// <remarks>
    /// Avalonia's default Linux backend uses X11, including XWayland on Wayland desktops.
    /// </remarks>
    public sealed class WindowClickThroughController : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Creates a controller for an Avalonia window.
        /// </summary>
        /// <param name="window">The window whose pointer input will be changed.</param>
        public WindowClickThroughController(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        #endregion

        #region Fields

        /// <summary>
        /// Stores the window controlled by this instance.
        /// </summary>
        private readonly Window _window;

        /// <summary>
        /// Records whether this controller added the Windows layered style.
        /// </summary>
        private bool _addedLayeredStyle;

        /// <summary>
        /// Records whether this controller added the Windows transparent style.
        /// </summary>
        private bool _addedTransparentStyle;

        private bool _isEnabled;

        /// <summary>
        /// Records whether this controller has been disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether pointer input currently passes through the window.
        /// </summary>
        public bool IsEnabled => _isEnabled;

        #endregion

        #region Public

        /// <summary>
        /// Attempts to enable or disable pointer pass-through for the open window.
        /// </summary>
        /// <param name="enabled">Whether pointer input should pass through the window.</param>
        /// <returns><see langword="true"/> when the requested mode is active or was applied.</returns>
        public bool TrySetEnabled(bool enabled)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WindowClickThroughController));
            }

            if (_isEnabled == enabled)
            {
                return true;
            }

            var handle = _window.TryGetPlatformHandle();

            if (handle == null || handle.Handle == IntPtr.Zero)
            {
                return false;
            }

            bool succeeded;

            if (string.Equals(handle.HandleDescriptor, "HWND", StringComparison.Ordinal))
            {
                succeeded = TrySetWindowsMode(handle.Handle, enabled);
            }
            else if (string.Equals(handle.HandleDescriptor, "XID", StringComparison.Ordinal))
            {
                succeeded = TrySetX11Mode(handle.Handle, enabled);
            }
            else
            {
                return false;
            }

            if (succeeded)
            {
                _isEnabled = enabled;
            }

            return succeeded;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_isEnabled)
            {
                TrySetEnabled(false);
            }

            _disposed = true;
        }

        #endregion

        #region Private

        /// <summary>
        /// Changes the extended styles of a Windows window.
        /// </summary>
        /// <param name="window">The native window handle.</param>
        /// <param name="enabled">Whether pointer input should pass through the window.</param>
        /// <returns>Whether the requested styles were applied.</returns>
        private bool TrySetWindowsMode(IntPtr window, bool enabled)
        {
            var current = IntPtr.Size == 8
                ? WindowsUser32.GetWindowLongPtr(window, WindowsUser32.WindowLongExtendedStyle).ToInt64()
                : WindowsUser32.GetWindowLong(window, WindowsUser32.WindowLongExtendedStyle);

            var addedLayered     = enabled && (current & WindowsUser32.WindowExStyleLayered    ) == 0;
            var addedTransparent = enabled && (current & WindowsUser32.WindowExStyleTransparent) == 0;

            long requested;

            if (enabled)
            {
                requested = current | WindowsUser32.WindowExStyleLayered | WindowsUser32.WindowExStyleTransparent;
            }
            else
            {
                requested = current & ~(_addedLayeredStyle     ? WindowsUser32.WindowExStyleLayered     : 0)
                                    & ~(_addedTransparentStyle ? WindowsUser32.WindowExStyleTransparent : 0);
            }

            if (IntPtr.Size == 8)
            {
                WindowsUser32.SetWindowLongPtr(window, WindowsUser32.WindowLongExtendedStyle, new IntPtr(requested));
            }
            else
            {
                WindowsUser32.SetWindowLong(window, WindowsUser32.WindowLongExtendedStyle, unchecked((int)requested));
            }

            var actual = IntPtr.Size == 8
                ? WindowsUser32.GetWindowLongPtr(window, WindowsUser32.WindowLongExtendedStyle).ToInt64()
                : WindowsUser32.GetWindowLong   (window, WindowsUser32.WindowLongExtendedStyle);

            if (actual != requested)
            {
                return false;
            }

            if (enabled)
            {
                _addedLayeredStyle     = addedLayered;
                _addedTransparentStyle = addedTransparent;
            }
            else
            {
                _addedLayeredStyle     = false;
                _addedTransparentStyle = false;
            }

            return true;
        }

        /// <summary>
        /// Changes the input region of an X11 window and its render children.
        /// </summary>
        /// <param name="window">The X11 window identifier.</param>
        /// <param name="enabled">Whether pointer input should pass through the window.</param>
        /// <returns>Whether the input-region requests were submitted to the X11 display.</returns>
        private static bool TrySetX11Mode(IntPtr window, bool enabled)
        {
            IntPtr display;

            try
            {
                display = LibX11.XOpenDisplay(IntPtr.Zero);
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }

            if (display == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                if (LibXFixes.XFixesQueryExtension(display, out _, out _) == 0)
                {
                    return false;
                }

                var region = enabled
                    ? LibXFixes.XFixesCreateRegion(display, IntPtr.Zero, 0)
                    : IntPtr.Zero;

                if (enabled && region == IntPtr.Zero)
                {
                    return false;
                }

                try
                {
                    ApplyX11InputRegion(display, window, region);

                    LibX11.XFlush(display);
                    return true;
                }
                finally
                {
                    if (region != IntPtr.Zero)
                    {
                        LibXFixes.XFixesDestroyRegion(display, region);
                    }
                }
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
            finally
            {
                LibX11.XCloseDisplay(display);
            }
        }

        /// <summary>
        /// Sets an X11 input region on a window and its child windows.
        /// </summary>
        /// <param name="display">The open X11 display.</param>
        /// <param name="window">The X11 window identifier.</param>
        /// <param name="region">An empty region for pass-through, or a null handle for normal input.</param>
        private static void ApplyX11InputRegion(IntPtr display, IntPtr window, IntPtr region)
        {
            LibXFixes.XFixesSetWindowShapeRegion(display, window, LibXFixes.ShapeInput, 0, 0, region);

            if (LibX11.XQueryTree(display, window, out _, out _, out var children, out var childCount) == 0)
            {
                return;
            }

            try
            {
                // Apply the input region to each native render child of the window.
                for (var index = 0; index < childCount; ++index)
                {
                    var child = Marshal.ReadIntPtr(children, checked((int)index * IntPtr.Size));

                    ApplyX11InputRegion(display, child, region);
                }
            }
            finally
            {
                if (children != IntPtr.Zero)
                {
                    LibX11.XFree(children);
                }
            }
        }

        #endregion
    }
}
