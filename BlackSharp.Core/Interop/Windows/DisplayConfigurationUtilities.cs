/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;

namespace BlackSharp.Core.Interop.Windows
{
    /// <summary>
    /// Provides typed access to the Windows display-configuration API.
    /// </summary>
    public static class DisplayConfigurationUtilities
    {
        #region Public

        /// <summary>
        /// Retrieves display paths and mode information for the requested topology.
        /// </summary>
        /// <param name="flags">The Windows display configuration query flags.</param>
        /// <param name="pathCount">The path array capacity and resulting path count.</param>
        /// <param name="paths">The array that receives display paths.</param>
        /// <param name="modeCount">The mode array capacity and resulting mode count.</param>
        /// <param name="modes">The array that receives display modes.</param>
        /// <param name="currentTopologyID">An optional pointer that receives the current topology identifier.</param>
        /// <returns>The native Windows result code.</returns>
        public static int QueryDisplayConfig(
            uint flags,
            ref uint pathCount,
            DisplayConfigPathInfo[] paths,
            ref uint modeCount,
            DisplayConfigModeInfo[] modes,
            IntPtr currentTopologyID)
        {
            var actualPathCount = pathCount;
            var actualModeCount = modeCount;
            var result = MarshalUtilities.InvokePinned(
                paths,
                modes,
                (pathPointer, modePointer) => User32.QueryDisplayConfig(
                    flags,
                    ref actualPathCount,
                    pathPointer,
                    ref actualModeCount,
                    modePointer,
                    currentTopologyID));

            pathCount = actualPathCount;
            modeCount = actualModeCount;

            return result;
        }

        /// <summary>
        /// Retrieves device information for a display source or target structure.
        /// </summary>
        /// <typeparam name="T">The display device information structure type.</typeparam>
        /// <param name="deviceInfo">The initialized request structure that receives the result.</param>
        /// <returns>The native Windows result code.</returns>
        public static int GetDeviceInfo<T>(ref T deviceInfo)
            where T : struct
        {
            return MarshalUtilities.Invoke(ref deviceInfo, User32.DisplayConfigGetDeviceInfo);
        }

        #endregion
    }
}
