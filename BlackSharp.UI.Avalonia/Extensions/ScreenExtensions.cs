/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

#pragma warning disable CA1416 // Platform compatibility warning

using Avalonia.Platform;
using BlackSharp.Core.Interop;
using BlackSharp.Core.Interop.Windows;
using BlackSharp.Core.Interop.Windows.Structures;
using Microsoft.Win32;
using OS = BlackSharp.Core.Platform.OperatingSystem;
using System.Runtime.InteropServices;
using System.Text;
using WindowsUser32 = BlackSharp.Core.Interop.Windows.Native.User32;

namespace BlackSharp.UI.Avalonia.Extensions
{
    /// <summary>
    /// Provides extension methods for retrieving hardware-related information from <see cref="Screen"/> instances.
    /// </summary>
    /// <remarks>Not all operating systems are supported yet.</remarks>
    public static class ScreenExtensions
    {
        #region Public

        /// <summary>
        /// Retrieves a display name for the specified screen.
        /// </summary>
        /// <param name="screen">The screen for which to obtain the display name.</param>
        /// <returns>The best available display name, or <see langword="null"/> if no display name could be determined.</returns>
        /// <remarks>
        /// Avalonia 12 can return empty display names on Windows. This method uses the Windows display configuration API as a fallback.
        /// </remarks>
        public static string GetDisplayName(this Screen screen)
        {
            if (!OS.IsWindows())
            {
                throw new PlatformNotSupportedException($"Support for {nameof(GetDisplayName)} has not been added for this operating system.");
            }

            ArgumentNullException.ThrowIfNull(screen);

            var info = GetWindowsScreenDeviceInfo(screen);

            var friendlyName = NormalizeDisplayName(info?.FriendlyName);
            if (friendlyName != null)
            {
                return friendlyName;
            }

            var avaloniaName = NormalizeDisplayName(screen.DisplayName);
            if (avaloniaName != null)
            {
                return avaloniaName;
            }

            return NormalizeDisplayName(info?.SourceName);
        }

        /// <summary>
        /// Retrieves a unique hardware identifier for the specified screen device.
        /// </summary>
        /// <param name="screen">The screen for which to obtain the hardware identifier.</param>
        /// <returns>A <see cref="string"/> containing the hardware identifier for the specified screen, or <see langword="null"/> if the identifier cannot be
        /// determined.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown if the current operating system is not supported.</exception>
        public static string GetDevicePath(this Screen screen)
        {
            if (!OS.IsWindows())
            {
                throw new PlatformNotSupportedException($"Support for {nameof(GetDevicePath)} has not been added for this operating system.");
            }

            return GetDevicePathString(screen);
        }

        /// <summary>
        /// Retrieves the serial number of the specified screen, if available.
        /// </summary>
        /// <param name="screen">The screen for which to obtain the serial number.</param>
        /// <returns>A <see cref="string"/> containing the serial number of the screen if available; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown if the current operating system is not supported.</exception>
        public static string GetSerialNumber(this Screen screen)
        {
            if (!OS.IsWindows())
            {
                throw new PlatformNotSupportedException($"Support for {nameof(GetSerialNumber)} has not been added for this operating system.");
            }

            var devicePath = GetDevicePathString(screen);
            if (devicePath == null)
            {
                return null;
            }

            //Get registry path
            var registryPath = GetRegistryPathFromDevicePath(devicePath);
            if (registryPath == null)
            {
                return null;
            }

            var edid = GetEdid(registryPath);
            if (edid == null)
            {
                return null;
            }

            return ParseEdidSerial(edid);
        }

        #endregion

        #region Private

        static string GetDevicePathString(Screen screen)
        {
            return NormalizeDisplayName(GetWindowsScreenDeviceInfo(screen)?.DevicePath);
        }

        static WindowsScreenDeviceInfo GetWindowsScreenDeviceInfo(Screen screen)
        {
            var rect = new Win32Rectangle
            {
                Left   = screen.Bounds.X,
                Top    = screen.Bounds.Y,
                Right  = screen.Bounds.Right,
                Bottom = screen.Bounds.Bottom,
            };

            //Get the monitor handle from the screens bounds
            var monitor = MarshalUtilities.Invoke(
                ref rect,
                pointer => WindowsUser32.MonitorFromRect(pointer, WindowsUser32.MonitorDefaultToNearest));

            if (monitor == IntPtr.Zero)
            {
                return null;
            }

            var monitorInfo = new MonitorInfoEx
            {
                Size = Marshal.SizeOf<MonitorInfoEx>(),
            };

            //Get the monitor info to obtain the device name
            if (!MarshalUtilities.Invoke(
                ref monitorInfo,
                pointer => WindowsUser32.GetMonitorInfo(monitor, pointer)))
            {
                return null;
            }

            if (WindowsUser32.GetDisplayConfigBufferSizes(WindowsUser32.QueryDisplayConfigOnlyActivePaths, out uint pathCount, out uint modeCount) != 0)
            {
                return null;
            }

            var paths = new DisplayConfigPathInfo[pathCount];
            var modes = new DisplayConfigModeInfo[modeCount];

            if (DisplayConfigurationUtilities.QueryDisplayConfig(
                WindowsUser32.QueryDisplayConfigOnlyActivePaths,
                ref pathCount,
                paths,
                ref modeCount,
                modes,
                IntPtr.Zero) != 0)
            {
                return null;
            }

            foreach (var path in paths)
            {
                var source = path.SourceInfo;

                var sourceName = new DisplayConfigSourceDeviceName();
                sourceName.Header.Type      = DisplayConfigDeviceInfoType.GetSourceName;
                sourceName.Header.Size      = (uint)Marshal.SizeOf<DisplayConfigSourceDeviceName>();
                sourceName.Header.AdapterID = source.AdapterID;
                sourceName.Header.ID        = source.ID;

                if (DisplayConfigurationUtilities.GetDeviceInfo(ref sourceName) != 0)
                {
                    continue;
                }

                //Check if this is the monitor we are looking for
                if (monitorInfo.DeviceName.CompareTo(sourceName.ViewGdiDeviceName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }

                var target = path.TargetInfo;

                var name = new DisplayConfigTargetDeviceName();
                name.Header.Type      = DisplayConfigDeviceInfoType.GetTargetName;
                name.Header.Size      = (uint)Marshal.SizeOf<DisplayConfigTargetDeviceName>();
                name.Header.AdapterID = target.AdapterID;
                name.Header.ID        = target.ID;

                if (DisplayConfigurationUtilities.GetDeviceInfo(ref name) != 0)
                {
                    continue;
                }

                return new WindowsScreenDeviceInfo(
                    name.MonitorDevicePath,
                    name.MonitorFriendlyDeviceName,
                    sourceName.ViewGdiDeviceName);
            }

            return null;
        }

        static string NormalizeDisplayName(string displayName)
        {
            return string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        }

        static string GetRegistryPathFromDevicePath(string devicePath)
        {
            //Example:
            // "\\?\DISPLAY#ABC4096#5&10a6699c&0&UID4052#{GUID}"

            var start = devicePath.IndexOf("DISPLAY", StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return null;
            }

            string path = devicePath.Substring(start);

            int guidIndex = path.IndexOf('{');
            if (guidIndex > 0)
            {
                path = path.Substring(0, guidIndex - 1);
            }

            path = path.Replace('#', '\\');

            return $@"SYSTEM\CurrentControlSet\Enum\{path}";
        }

        static byte[] GetEdid(string registryPath)
        {
            using var key = Registry.LocalMachine.OpenSubKey(registryPath + @"\Device Parameters");

            return key?.GetValue("EDID") as byte[];
        }

        static string ParseEdidSerial(byte[] edid)
        {
            //Search descriptor blocks (starting at offset 54)
            for (int i = 54; i <= 108; i += 18)
            {
                if (edid[i    ] == 0x00
                 && edid[i + 1] == 0x00
                 && edid[i + 2] == 0x00
                 && edid[i + 3] == 0xFF)
                {
                    return Encoding.ASCII
                        .GetString(edid, i + 5, 13)
                        .Trim('\0', '\n', ' ');
                }
            }

            //Fallback: numeric serial in the header
            uint serial =
                (uint)(edid[12]
              | (edid[13] << 8)
              | (edid[14] << 16)
              | (edid[15] << 24));

            return serial != 0 ? serial.ToString() : null;
        }

        #endregion

        #region Nested types

        sealed class WindowsScreenDeviceInfo
        {
            public WindowsScreenDeviceInfo(string devicePath, string friendlyName, string sourceName)
            {
                DevicePath = devicePath;
                FriendlyName = friendlyName;
                SourceName = sourceName;
            }

            public string DevicePath { get; }

            public string FriendlyName { get; }

            public string SourceName { get; }
        }

        #endregion
    }
}

#pragma warning restore CA1416 // Platform compatibility warning
