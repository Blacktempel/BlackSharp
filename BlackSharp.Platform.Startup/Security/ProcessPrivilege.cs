// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Linux.Native;
using System.Security.Principal;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

#pragma warning disable CA1416 // Platform compatibility warning

namespace BlackSharp.Platform.Startup.Security
{
    /// <summary>
    /// Checks whether the current process has elevated operating-system privileges.
    /// </summary>
    public static class ProcessPrivilege
    {
        #region Properties

        /// <summary>
        /// Gets whether the current process is elevated on Windows or running as root on Linux.
        /// </summary>
        public static bool IsElevated
        {
            get
            {
                if (PlatformOperatingSystem.IsWindows())
                {
                    using var identity = WindowsIdentity.GetCurrent();

                    var principal = new WindowsPrincipal(identity);

                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                if (PlatformOperatingSystem.IsLinux())
                {
                    try
                    {
                        return LibC.geteuid() == 0;
                    }
                    catch (DllNotFoundException)
                    {
                        return false;
                    }
                    catch (EntryPointNotFoundException)
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        #endregion
    }
}

#pragma warning restore CA1416 // Platform compatibility warning
