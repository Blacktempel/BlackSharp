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
    /// Describes the timing of a Windows display video signal.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayConfigVideoSignalInfo
    {
        #region Fields

        /// <summary>
        /// Contains the pixel clock rate.
        /// </summary>
        public ulong PixelRate;

        /// <summary>
        /// Contains the horizontal synchronization frequency.
        /// </summary>
        public DisplayConfigRational HorizontalSyncFrequency;

        /// <summary>
        /// Contains the vertical synchronization frequency.
        /// </summary>
        public DisplayConfigRational VerticalSyncFrequency;

        /// <summary>
        /// Contains the active image size.
        /// </summary>
        public DisplayConfig2DRegion ActiveSize;

        /// <summary>
        /// Contains the total image size.
        /// </summary>
        public DisplayConfig2DRegion TotalSize;

        /// <summary>
        /// Identifies the video standard.
        /// </summary>
        public uint VideoStandard;

        /// <summary>
        /// Identifies the scan-line ordering.
        /// </summary>
        public uint ScanLineOrdering;

        #endregion
    }
}
