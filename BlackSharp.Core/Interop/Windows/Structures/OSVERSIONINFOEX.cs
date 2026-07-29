/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes a Windows operating-system version and product configuration.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct OsVersionInfoEx
    {
        /// <summary>
        /// The size of this structure in bytes.
        /// </summary>
        public int OsVersionInfoSize;

        /// <summary>
        /// The major operating-system version.
        /// </summary>
        public int MajorVersion;

        /// <summary>
        /// The minor operating-system version.
        /// </summary>
        public int MinorVersion;

        /// <summary>
        /// The operating-system build number.
        /// </summary>
        public int BuildNumber;

        /// <summary>
        /// The operating-system platform identifier.
        /// </summary>
        public int PlatformId;

        /// <summary>
        /// A textual description of the installed service pack.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string CsdVersion;

        /// <summary>
        /// The major version of the installed service pack.
        /// </summary>
        public ushort ServicePackMajor;

        /// <summary>
        /// The minor version of the installed service pack.
        /// </summary>
        public ushort ServicePackMinor;

        /// <summary>
        /// A bit mask identifying installed product suites.
        /// </summary>
        public ushort SuiteMask;

        /// <summary>
        /// The Windows product type.
        /// </summary>
        public byte ProductType;

        /// <summary>
        /// A value reserved for the operating system.
        /// </summary>
        public byte Reserved;
    }
}
