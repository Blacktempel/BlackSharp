// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    /// <summary>
    /// Represents a native Windows window procedure.
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr WindowProcedure(IntPtr window, uint message, UIntPtr wParam, IntPtr lParam);
}
