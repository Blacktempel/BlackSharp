/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

#pragma warning disable CA1416 // Platform compatibility warning

using Avalonia.Platform;
using BlackSharp.UI.Avalonia.Platform.Windows.Interop;
using BlackSharp.UI.Avalonia.Platform.Windows.Interop.Enums;
using BlackSharp.UI.Avalonia.Platform.Windows.Interop.Structures;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.UI.Avalonia.Extensions
{
    /// <summary>
    /// Provides extension methods for retrieving hardware-related information from <see cref="Screen"/> instances.
    /// </summary>
    /// <remarks>Not all operating systems are supported yet.</remarks>
    public static class ScreenExtensions
    {
        #region Fields

        static readonly Regex _HWIDRegex = new(@"DISPLAY#([^#]+)", RegexOptions.IgnoreCase);

        #endregion

        #region Public

        /// <summary>
        /// Retrieves a unique hardware identifier for the specified screen device.
        /// </summary>
        /// <param name="screen">The screen for which to obtain the hardware identifier.</param>
        /// <returns>A <see cref="string"/> containing the hardware identifier for the specified screen, or <see langword="null"/> if the identifier cannot be
        /// determined.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown if the current operating system is not supported.</exception>
        public static string GetHardwareID(this Screen screen)
        {
            if (!OS.IsWindows())
            {
                throw new PlatformNotSupportedException($"Support for {nameof(GetHardwareID)} has not been added for this operating system.");
            }

            var devicePath = GetDevicePath(screen);

            var match = _HWIDRegex.Match(devicePath);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
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

            var devicePath = GetDevicePath(screen);

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

        static string GetDevicePath(Screen screen)
        {
            var rect = new RECT
            {
                left = screen.Bounds.X,
                top = screen.Bounds.Y,
                right = screen.Bounds.Right,
                bottom = screen.Bounds.Bottom
            };

            var monitor = User32.MonitorFromRect(ref rect, User32.MONITOR_DEFAULTTONEAREST);

            if (monitor == IntPtr.Zero)
            {
                return null;
            }

            if (User32.GetDisplayConfigBufferSizes(User32.QDC_ONLY_ACTIVE_PATHS, out uint pathCount, out uint modeCount) != 0)
            {
                return null;
            }

            var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            if (User32.QueryDisplayConfig(User32.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero) != 0)
            {
                return null;
            }

            foreach (var path in paths)
            {
                var source = path.sourceInfo;
                var target = path.targetInfo;

                var name = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                name.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                name.header.size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                name.header.adapterId = target.adapterId;
                name.header.id = target.id;

                if (User32.DisplayConfigGetDeviceInfo(ref name) != 0)
                {
                    continue;
                }

                return name.monitorDevicePath;
            }

            return null;
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
    }
}

#pragma warning restore CA1416 // Platform compatibility warning
