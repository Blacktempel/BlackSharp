// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Acl
    {
        byte AclRevision;
        byte Sbz1;
        short AclSize;
        short AceCount;
        short Sbz2;
    }
}
