/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;
using System.Runtime.InteropServices;
using System.Security;

using OS = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.Interop.Windows.Mutexes
{
    /// <summary>
    /// Create / Open a mutex with appropiate protection.
    /// </summary>
    public sealed class WorldMutex : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructs a new global mutex with specified name.
        /// </summary>
        /// <param name="mutexName">Name of mutex.</param>
        public WorldMutex(string mutexName)
        {
            if (string.IsNullOrWhiteSpace(mutexName))
            {
                throw new ArgumentException("A mutex name is required.", nameof(mutexName));
            }

            if (OS.IsWindows())
            {
                _WorldMutex = CreateWorldMutex(mutexName);
            }
            else
            {
                MutexName     = $"Global\\{mutexName}";
                _ManagedMutex = new Mutex(false, MutexName);
            }
        }

        /// <summary>
        /// Destructs the object.
        /// </summary>
        ~WorldMutex()
        {
            Dispose(false);
        }

        #endregion

        #region Constants

        internal const uint TWO_SECONDS = 2000;

        internal const int SECURITY_DESCRIPTOR_REVISION = 1;
        internal const int SECURITY_WORLD_RID = 0x00;

        internal static readonly byte[] SECURITY_WORLD_SID_AUTHORITY = new byte[] { 0, 0, 0, 0, 0, 1 };

        internal const int ACL_REVISION = 2;

        internal const uint MUTANT_QUERY_STATE = 0x0001;
        internal const uint READ_CONTROL = 0x00020000;
        internal const uint SYNCHRONIZE = 0x00100000;
        internal const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        internal const uint MUTANT_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | MUTANT_QUERY_STATE;

        #endregion

        #region Fields

        /// <summary>
        /// Mutex Handle.
        /// </summary>
        IntPtr _WorldMutex;
        Mutex  _ManagedMutex;
        bool   _Disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Name of internal mutex.
        /// </summary>
        public string MutexName { get; private set; }

        #endregion

        #region Public

        /// <summary>
        /// Attempts to lock the mutex with specified timeout (default is 2 seconds).
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds to wait for the mutex.</param>
        /// <returns>Returns true if the mutex was successfully locked within the specified timeout, false otherwise.</returns>
        public bool Lock(uint millisecondsTimeout = TWO_SECONDS)
        {
            if (_Disposed)
            {
                return false;
            }

            if (_ManagedMutex != null)
            {
                try
                {
                    return _ManagedMutex.WaitOne(
                        TimeSpan.FromMilliseconds(millisecondsTimeout));
                }
                catch (AbandonedMutexException)
                {
                    return true;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }

            if (_WorldMutex != IntPtr.Zero)
            {
                return Kernel32.WaitForSingleObject(_WorldMutex, millisecondsTimeout) == 0;
            }

            return false;
        }

        /// <summary>
        /// Unlock the mutex.
        /// </summary>
        public void Unlock()
        {
            if (_Disposed)
            {
                return;
            }

            if (_ManagedMutex != null)
            {
                try
                {
                    _ManagedMutex.ReleaseMutex();
                }
                catch (ApplicationException)
                {
                }
                catch (ObjectDisposedException)
                {
                }

                return;
            }

            if (_WorldMutex != IntPtr.Zero)
            {
                Kernel32.ReleaseMutex(_WorldMutex);
            }
        }

        /// <summary>
        /// Releases the operating-system mutex resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private

        void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }

            _Disposed = true;

            if (disposing)
            {
                _ManagedMutex?.Dispose();
            }

            _ManagedMutex = null;

            var handle = _WorldMutex;

            _WorldMutex = IntPtr.Zero;

            if (!OS.IsWindows() || handle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                Kernel32.CloseHandle(handle);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Create / Open a mutex with appropiate protection.
        /// </summary>
        /// <param name="name">Name of mutex to create.</param>
        [SecurityCritical]
        IntPtr CreateWorldMutex(string name)
        {
            //Setup security descriptor
            AdvApi32.InitializeSecurityDescriptor(out var sdb, SECURITY_DESCRIPTOR_REVISION);

            var swa = new SidIdentifierAuthority();
            swa.Value = SECURITY_WORLD_SID_AUTHORITY; //World access

            int aclSize = Marshal.SizeOf<Acl>() * 32;
            IntPtr acl = Marshal.AllocHGlobal(aclSize);

            if (AdvApi32.AllocateAndInitializeSid
                         (
                             ref swa,                            //SID identifier authority
                             1,                                  //Sub authority count
                             SECURITY_WORLD_RID, //Sub authority 0
                             0,                                  //Sub authority 1
                             0,                                  //Sub authority 2
                             0,                                  //Sub authority 3
                             0,                                  //Sub authority 4
                             0,                                  //Sub authority 5
                             0,                                  //Sub authority 6
                             0,                                  //Sub authority 7
                             out var sid                         //Returned SID
                         )
             &&
                AdvApi32.InitializeAcl(acl, (uint)aclSize, ACL_REVISION) //ACL setup OK and
             &&
                AdvApi32.AddAccessAllowedAce(acl, ACL_REVISION, MUTANT_ALL_ACCESS, sid) //ACE setup OK ?
               )
            {
                AdvApi32.SetSecurityDescriptorDacl(ref sdb, true, acl, false); //Yes, setup world access
            }
            else
            {
                AdvApi32.SetSecurityDescriptorDacl(ref sdb, true, IntPtr.Zero, false); //else setup with default
            }

            IntPtr sdbPtr = Marshal.AllocHGlobal(Marshal.SizeOf(sdb));
            Marshal.StructureToPtr(sdb, sdbPtr, false);

            var sab = new SecurityAttributes(); //Setup security attributes block
            sab.Length = (uint)Marshal.SizeOf(sdb);
            sab.InheritHandle = false;
            sab.SecurityDescriptor = sdbPtr;

            var mutexName = $"Global\\{name}";

            IntPtr mutexHandle = IntPtr.Zero;

            if
            (
                (mutexHandle = Kernel32.CreateMutex(ref sab, false, mutexName)) != IntPtr.Zero || //Create / open with Global\ unprotected or
                (mutexHandle = Kernel32.OpenMutex(READ_CONTROL | //Open with Global\ protected or (probably Aquasuite)
                                                  MUTANT_QUERY_STATE |
                                                  SYNCHRONIZE, false, mutexName)) != IntPtr.Zero ||
                (mutexHandle = Kernel32.CreateMutex(ref sab, false, name)) != IntPtr.Zero //Create / open with no prefix unprotected ?
            )
            {
                MutexName = mutexName;
            }

            Marshal.FreeHGlobal(acl); //Free acl
            Marshal.FreeHGlobal(sdbPtr); //Free sbd

            if (sid != IntPtr.Zero) //Need to free the SID ?
            {
                AdvApi32.FreeSid(sid); //Yes, free it
            }

            return mutexHandle; //Return the handle
        }

        #endregion
    }
}
