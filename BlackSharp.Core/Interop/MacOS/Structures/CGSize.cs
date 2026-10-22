/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.MacOS.Structures;

/// <summary>
/// Represents CG size data exchanged with the underlying platform.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CGSize
{
    #region Fields

    /// <summary>
    /// Carries the width value exchanged with the native API.
    /// </summary>
    public double Width;

    /// <summary>
    /// Carries the height value exchanged with the native API.
    /// </summary>
    public double Height;

    #endregion
}
