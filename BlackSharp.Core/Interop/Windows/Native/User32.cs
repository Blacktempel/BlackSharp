/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected Windows user-interface functions and constants.
    /// </summary>
    public static class User32
    {
        #region Fields

        const string DLL_NAME = "user32.dll";

        /// <summary>
        /// Selects the monitor nearest to a rectangle when the rectangle does not intersect a display.
        /// </summary>
        public const int MonitorDefaultToNearest = 2;

        /// <summary>
        /// Restricts display-configuration queries to currently active paths.
        /// </summary>
        public const uint QueryDisplayConfigOnlyActivePaths = 0x00000002;

        #endregion

        #region Imports

        /// <summary>
        /// Creates an overlapped, pop-up, or child window.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(uint extendedStyle, string className, string windowName, uint style, int x, int y, int width, int height, IntPtr parentWindow, IntPtr menu, IntPtr instance, IntPtr parameter);

        /// <summary>
        /// Performs default processing for a window message.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr window, uint message, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Destroys a window.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr window);

        /// <summary>
        /// Dispatches a message to a window procedure.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern IntPtr DispatchMessage(IntPtr message);

        /// <summary>
        /// Retrieves display-configuration information for a source or target.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int DisplayConfigGetDeviceInfo(IntPtr deviceInfo);

        /// <summary>
        /// Enumerates display devices.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayDevices(string deviceName, uint deviceNumber, IntPtr displayDevice, uint flags);

        /// <summary>
        /// Enumerates display monitors intersecting a region.
        /// </summary>
        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayMonitors(IntPtr deviceContext, IntPtr clipRectangle, IntPtr callback, IntPtr data);

        /// <summary>
        /// Retrieves a display device's graphics mode.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNumber, IntPtr deviceMode);

        /// <summary>
        /// Retrieves the buffer sizes required for a display-configuration query.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int GetDisplayConfigBufferSizes(uint flags, out uint pathCount, out uint modeCount);

        /// <summary>
        /// Retrieves a message from the calling thread's message queue.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetMessage(IntPtr message, IntPtr window, uint minimumMessage, uint maximumMessage);

        /// <summary>
        /// Retrieves information about a display monitor.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr monitor, IntPtr monitorInfo);

        /// <summary>
        /// Retrieves the monitor that intersects or is nearest to a rectangle.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern IntPtr MonitorFromRect(IntPtr rectangle, uint flags);

        /// <summary>
        /// Places a message in a window's message queue.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr window, uint message, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Retrieves display paths and mode information for the requested topology.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern int QueryDisplayConfig(uint flags, ref uint pathCount, IntPtr paths, ref uint modeCount, IntPtr modes, IntPtr currentTopologyId);

        /// <summary>
        /// Registers a window class.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern ushort RegisterClassEx(IntPtr windowClass);

        /// <summary>
        /// Registers a recipient for device notifications.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, uint flags);

        /// <summary>
        /// Defines a system-wide hot key.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr window, int id, uint modifiers, uint virtualKey);

        /// <summary>
        /// Translates virtual-key messages into character messages.
        /// </summary>
        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(IntPtr message);

        /// <summary>
        /// Removes a device-notification registration.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterDeviceNotification(IntPtr handle);

        /// <summary>
        /// Removes a system-wide hot-key registration.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr window, int id);

        #endregion
    }
}
