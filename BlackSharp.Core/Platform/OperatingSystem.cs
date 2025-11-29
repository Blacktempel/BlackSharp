/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

/*

+------------------------------------------------------------------------------+
|                    |   PlatformID    |   Major version   |   Minor version   |
+------------------------------------------------------------------------------+
| Windows 95         |  Win32Windows   |         4         |          0        |
| Windows 98         |  Win32Windows   |         4         |         10        |
| Windows Me         |  Win32Windows   |         4         |         90        |
| Windows NT 4.0     |  Win32NT        |         4         |          0        |
| Windows 2000       |  Win32NT        |         5         |          0        |
| Windows XP         |  Win32NT        |         5         |          1        |
| Windows 2003       |  Win32NT        |         5         |          2        |
| Windows Vista      |  Win32NT        |         6         |          0        |
| Windows 2008       |  Win32NT        |         6         |          0        |
| Windows 7          |  Win32NT        |         6         |          1        |
| Windows 2008 R2    |  Win32NT        |         6         |          1        |
| Windows 8          |  Win32NT        |         6         |          2        |
| Windows 8.1        |  Win32NT        |         6         |          3        |
+------------------------------------------------------------------------------+
| Windows 10         |  Win32NT        |        10         |          0        |
+------------------------------------------------------------------------------+

*/

using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;

namespace BlackSharp.Core.Platform
{
    /// <summary>
    /// Represents information about an operating system.
    /// </summary>
    public static class OperatingSystem
    {
#if !NET8_0_OR_GREATER

        #region Constructor

        static OperatingSystem()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    _IsWindows = true;
                    break;
                case PlatformID.Unix:
                    _IsLinux = true;
                    break;
            }
        }

        #endregion

        #region Fields

        static readonly bool _IsWindows;
        static readonly bool _IsLinux;

        #endregion

#endif

        #region Public

        /// <summary>
        /// Indicates whether the current application is running on Windows.
        /// </summary>
        /// <returns>True if current application is running on Windows, false otherwise.</returns>
        public static bool IsWindows()
        {
#if NET8_0_OR_GREATER
            return System.OperatingSystem.IsWindows();
#else
            return _IsWindows;
#endif
        }

        /// <summary>
        /// Indicates whether the current application is running on Linux.
        /// </summary>
        /// <returns>True if current application is running on Linux, false otherwise.</returns>
        public static bool IsLinux()
        {
#if NET8_0_OR_GREATER
            return System.OperatingSystem.IsLinux();
#else
            return _IsLinux;
#endif
        }

        /// <summary>
        /// Retrieves the major and minor version numbers of the operating system.
        /// </summary>
        /// <param name="major">When this method returns, contains the major version number of the operating system if successful; otherwise, zero.</param>
        /// <param name="minor">When this method returns, contains the minor version number of the operating system if successful; otherwise, zero.</param>
        /// <returns>true if the operating system version was successfully retrieved; otherwise, false.</returns>
        public static bool GetOSVersion(out int major, out int minor)
        {
            if (IsWindows())
            {
                var versionInfo = new OSVERSIONINFOEX();

                if (NTDLL.RtlGetVersion(ref versionInfo) != 0)
                {
                    major = minor = 0;
                    return false;
                }

                major = versionInfo.MajorVersion;
                minor = versionInfo.MinorVersion;

                return true;
            }
            else
            {
                //Not implemented
                major = minor = 0;

                return false;
            }
        }

        #endregion
    }
}
