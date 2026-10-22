// SPDX-License-Identifier: MPL-2.0

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
