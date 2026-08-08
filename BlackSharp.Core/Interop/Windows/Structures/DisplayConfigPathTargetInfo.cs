/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Describes a target in a Windows display path.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigPathTargetInfo
    {
        #region Fields

        /// <summary>
        /// Identifies the target adapter.
        /// </summary>
        public DisplayConfigLuid AdapterID;

        /// <summary>
        /// Identifies the target on the adapter.
        /// </summary>
        public uint ID;

        /// <summary>
        /// Contains the related mode information index.
        /// </summary>
        public uint ModeInfoIndex;

        /// <summary>
        /// Identifies the video output technology.
        /// </summary>
        public uint OutputTechnology;

        /// <summary>
        /// Identifies the display rotation.
        /// </summary>
        public uint Rotation;

        /// <summary>
        /// Identifies the display scaling mode.
        /// </summary>
        public uint Scaling;

        /// <summary>
        /// Contains the display refresh rate.
        /// </summary>
        public DisplayConfigRational RefreshRate;

        /// <summary>
        /// Identifies the scan-line ordering.
        /// </summary>
        public uint ScanLineOrdering;

        /// <summary>
        /// Indicates whether the target is available.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool TargetAvailable;

        /// <summary>
        /// Contains target status flags.
        /// </summary>
        public uint StatusFlags;

        #endregion
    }
}
