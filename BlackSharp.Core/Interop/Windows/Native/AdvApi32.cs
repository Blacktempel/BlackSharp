/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;
using System.Text;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected Windows security, service-control, and registry functions.
    /// </summary>
    public static class AdvApi32
    {
        #region Fields

        const string DLL_NAME = "advapi32.dll";

        /// <summary>
        /// Grants read access to a registry key.
        /// </summary>
        public const uint KeyRead    = 0x00020019;

        /// <summary>
        /// Identifies a null-terminated registry string.
        /// </summary>
        public const uint RegSz      = 1;

        /// <summary>
        /// Identifies arbitrary binary registry data.
        /// </summary>
        public const uint RegBinary  = 3;

        /// <summary>
        /// Identifies a 32-bit registry value.
        /// </summary>
        public const uint RegDword   = 4;

        /// <summary>
        /// Identifies a sequence of null-terminated registry strings.
        /// </summary>
        public const uint RegMultiSz = 7;

        /// <summary>
        /// Identifies a 64-bit registry value.
        /// </summary>
        public const uint RegQword   = 11;

        /// <summary>
        /// Represents the predefined local-machine registry key.
        /// </summary>
        public static readonly IntPtr HkeyLocalMachine =
            new(unchecked((int)0x80000002));

        /// <summary>
        /// Grants access to connect to the service control manager.
        /// </summary>
        public const uint SC_MANAGER_CONNECT        = 0x0001;

        /// <summary>
        /// Grants access to create services.
        /// </summary>
        public const uint SC_MANAGER_CREATE_SERVICE = 0x0002;

        /// <summary>
        /// Grants access to query a service configuration.
        /// </summary>
        public const uint SERVICE_QUERY_CONFIG  = 0x0001;

        /// <summary>
        /// Grants access to change a service configuration.
        /// </summary>
        public const uint SERVICE_CHANGE_CONFIG = 0x0002;

        /// <summary>
        /// Grants access to query service status.
        /// </summary>
        public const uint SERVICE_QUERY_STATUS  = 0x0004;

        /// <summary>
        /// Grants access to start a service.
        /// </summary>
        public const uint SERVICE_START         = 0x0010;

        /// <summary>
        /// Grants access to stop a service.
        /// </summary>
        public const uint SERVICE_STOP          = 0x0020;

        /// <summary>
        /// Grants access to delete a service.
        /// </summary>
        public const uint SERVICE_DELETE        = 0x00010000;

        /// <summary>
        /// Identifies a kernel-mode driver service.
        /// </summary>
        public const uint SERVICE_KERNEL_DRIVER = 0x00000001;

        /// <summary>
        /// Configures a service for manual start.
        /// </summary>
        public const uint SERVICE_DEMAND_START  = 0x00000003;

        /// <summary>
        /// Records the error and continues system startup.
        /// </summary>
        public const uint SERVICE_ERROR_NORMAL  = 0x00000001;

        /// <summary>
        /// Leaves an existing service configuration value unchanged.
        /// </summary>
        public const uint SERVICE_NO_CHANGE     = 0xFFFFFFFF;

        /// <summary>
        /// Requests that a service stop.
        /// </summary>
        public const uint SERVICE_CONTROL_STOP  = 0x00000001;

        #endregion

        #region Imports

        /// <summary>
        /// Adds an allowed-access entry to an access-control list.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern bool AddAccessAllowedAce(IntPtr acl, uint dwAceRevision, uint AccessMask, IntPtr sid);

        /// <summary>
        /// Allocates and initializes a security identifier.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern bool AllocateAndInitializeSid(ref SidIdentifierAuthority pIdentifierAuthority, byte nSubAuthorityCount, int dwSubAuthority0, int dwSubAuthority1, int dwSubAuthority2, int dwSubAuthority3, int dwSubAuthority4, int dwSubAuthority5, int dwSubAuthority6, int dwSubAuthority7, out IntPtr pSid);

        /// <summary>
        /// Changes the configuration of an installed service.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig(IntPtr service, uint serviceType, uint startType, uint errorControl, string binaryPathName, string loadOrderGroup, IntPtr tagId, string dependencies, string serviceStartName, string password, string displayName);

        /// <summary>
        /// Closes a service-control handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr serviceControlObject);

        /// <summary>
        /// Sends a control code to a service.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(IntPtr service, uint control, IntPtr serviceStatus);

        /// <summary>
        /// Creates a service in the service-control database.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateService(IntPtr serviceControlManager, string serviceName, string displayName, uint desiredAccess, uint serviceType, uint startType, uint errorControl, string binaryPathName, string loadOrderGroup, IntPtr tagId, string dependencies, string serviceStartName, string password);

        /// <summary>
        /// Marks a service for deletion.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteService(IntPtr service);

        /// <summary>
        /// Releases a security identifier allocated by the system.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern IntPtr FreeSid(IntPtr pSid);

        /// <summary>
        /// Initializes an access-control list.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern bool InitializeAcl(IntPtr acl, uint aclLength, uint aclRevision);

        /// <summary>
        /// Initializes a security descriptor.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(out SecurityDescriptor securityDescriptor, uint dwRevision);

        /// <summary>
        /// Opens the service control manager.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint desiredAccess);

        /// <summary>
        /// Opens an installed service.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr serviceControlManager, string serviceName, uint desiredAccess);

        /// <summary>
        /// Closes an open registry key.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern int RegCloseKey(IntPtr key);

        /// <summary>
        /// Enumerates a registry key's subkeys.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int RegEnumKeyEx(IntPtr key, uint index, StringBuilder name, ref uint nameLength, IntPtr reserved, IntPtr keyClass, IntPtr keyClassLength, IntPtr lastWriteTime);

        /// <summary>
        /// Opens a registry key.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int RegOpenKeyEx(IntPtr key, string subKey, uint options, uint desiredAccess, out IntPtr result);

        /// <summary>
        /// Retrieves registry value data into a byte buffer.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int RegQueryValueEx(IntPtr key, string valueName, IntPtr reserved, out uint type, [Out] byte[] data, ref uint dataSize);

        /// <summary>
        /// Retrieves registry value data into unmanaged memory.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int RegQueryValueEx(IntPtr key, string valueName, IntPtr reserved, out uint type, IntPtr data, ref uint dataSize);

        /// <summary>
        /// Sets the discretionary access-control list of a security descriptor.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(ref SecurityDescriptor sdb, bool bDaclPresent, IntPtr acl, bool bDaclDefaulted);

        /// <summary>
        /// Starts a service.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(IntPtr service, uint serviceArgumentCount, string[] serviceArgumentVectors);

        #endregion
    }
}
