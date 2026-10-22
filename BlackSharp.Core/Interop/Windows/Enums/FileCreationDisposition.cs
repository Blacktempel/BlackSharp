// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Interop.Windows.Enums
{
    /// <summary>
    /// Defines how a Windows file-opening operation handles existing and missing files.
    /// </summary>
    public enum FileCreationDisposition : uint
    {
        /// <summary>Creates a new file and fails if it already exists.</summary>
        CreateNew        = 1,
        /// <summary>Creates a file and overwrites an existing file.</summary>
        CreateAlways     = 2,
        /// <summary>Opens an existing file and fails if it does not exist.</summary>
        OpenExisting     = 3,
        /// <summary>Opens an existing file or creates a new one.</summary>
        OpenAlways       = 4,
        /// <summary>Opens and truncates an existing file.</summary>
        TruncateExisting = 5
    }
}
