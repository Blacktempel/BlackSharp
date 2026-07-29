/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 */

using BlackSharp.Core.Interop.Windows.Enums;
using BlackSharp.Core.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace BlackSharp.Core.Interop.Windows.Native
{
    /// <summary>
    /// Provides selected Windows kernel functions, flags, and status values.
    /// </summary>
    public static class Kernel32
    {
        #region Fields

        const string DLL_NAME = "kernel32.dll";

        /// <summary>
        /// Requests generic read access.
        /// </summary>
        public const uint GENERIC_READ = 0x80000000;

        /// <summary>
        /// Requests generic write access.
        /// </summary>
        public const uint GENERIC_WRITE = 0x40000000;

        /// <summary>
        /// Allows subsequent handles to request read access.
        /// </summary>
        public const uint FILE_SHARE_READ = 0x00000001;

        /// <summary>
        /// Allows subsequent handles to request write access.
        /// </summary>
        public const uint FILE_SHARE_WRITE = 0x00000002;

        /// <summary>
        /// Opens an existing file or device.
        /// </summary>
        public const uint OPEN_EXISTING = 3;

        /// <summary>
        /// Terminates pending transmit operations.
        /// </summary>
        public const uint PurgeTxAbort = 0x0001;

        /// <summary>
        /// Terminates pending receive operations.
        /// </summary>
        public const uint PurgeRxAbort = 0x0002;

        /// <summary>
        /// Clears the transmit queue.
        /// </summary>
        public const uint PurgeTxClear = 0x0004;

        /// <summary>
        /// Clears the receive queue.
        /// </summary>
        public const uint PurgeRxClear = 0x0008;

        /// <summary>
        /// Clears the data-terminal-ready signal.
        /// </summary>
        public const uint Clrdtr = 6;

        /// <summary>
        /// Sets the data-terminal-ready signal.
        /// </summary>
        public const uint Setdtr = 5;

        /// <summary>
        /// Clears the request-to-send signal.
        /// </summary>
        public const uint Clrrts = 4;

        /// <summary>
        /// Sets the request-to-send signal.
        /// </summary>
        public const uint Setrts = 3;

        /// <summary>
        /// Indicates that an I/O operation was canceled.
        /// </summary>
        public const int ErrorOperationAborted =  995;

        /// <summary>
        /// Indicates that an overlapped I/O operation has not completed.
        /// </summary>
        public const int ErrorIoIncomplete     =  996;

        /// <summary>
        /// Indicates that an overlapped I/O operation is pending.
        /// </summary>
        public const int ErrorIoPending        =  997;

        /// <summary>
        /// Indicates that the requested item was not found.
        /// </summary>
        public const int ErrorNotFound         = 1168;

        /// <summary>
        /// Indicates that a wait object was signaled.
        /// </summary>
        public const uint WaitObject0 = 0x00000000;

        /// <summary>
        /// Indicates that a wait interval elapsed.
        /// </summary>
        public const uint WaitTimeout = 0x00000102;

        /// <summary>
        /// Indicates that a wait operation failed.
        /// </summary>
        public const uint WaitFailed  = 0xFFFFFFFF;

        /// <summary>
        /// Requests an infinite wait interval.
        /// </summary>
        public const uint Infinite    = 0xFFFFFFFF;

        /// <summary>
        /// Disables parity checking.
        /// </summary>
        public const byte Noparity    = 0;

        /// <summary>
        /// Selects odd parity.
        /// </summary>
        public const byte Oddparity   = 1;

        /// <summary>
        /// Selects even parity.
        /// </summary>
        public const byte Evenparity  = 2;

        /// <summary>
        /// Selects mark parity.
        /// </summary>
        public const byte Markparity  = 3;

        /// <summary>
        /// Selects space parity.
        /// </summary>
        public const byte Spaceparity = 4;

        /// <summary>
        /// Selects one stop bit.
        /// </summary>
        public const byte Onestopbit   = 0;

        /// <summary>
        /// Selects one and a half stop bits.
        /// </summary>
        public const byte One5stopbits = 1;

        /// <summary>
        /// Selects two stop bits.
        /// </summary>
        public const byte Twostopbits  = 2;

        /// <summary>
        /// Represents the invalid native handle value.
        /// </summary>
        public static readonly IntPtr InvalidHandle = new IntPtr(-1);

        #endregion

        #region Imports

        /// <summary>
        /// Cancels pending asynchronous I/O associated with a handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

        /// <summary>
        /// Cancels the specified asynchronous I/O operation.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CancelIoEx(SafeFileHandle hFile, ref NativeOverlappedData lpOverlapped);

        /// <summary>
        /// Retrieves and clears communications errors and status.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClearCommError(IntPtr hFile, out uint lpErrors, out ComStat lpStat);

        /// <summary>
        /// Closes an open native object handle.
        /// </summary>
        [DllImport(DLL_NAME)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Creates or opens an event object.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, [MarshalAs(UnmanagedType.Bool)] bool bManualReset, [MarshalAs(UnmanagedType.Bool)] bool bInitialState, string lpName);

        /// <summary>
        /// Opens a file or device and returns a native handle.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, DesiredAccess dwDesiredAccess, FileShareMode dwShareMode, IntPtr lpSecurityAttributes, FileCreationDisposition dwCreationDisposition, FileFlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// Opens a file or device and returns a safe handle.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", SetLastError = true)]
        public static extern SafeFileHandle CreateFileS(string lpFileName, DesiredAccess dwDesiredAccess, FileShareMode dwShareMode, IntPtr lpSecurityAttributes, FileCreationDisposition dwCreationDisposition, FileFlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// Opens a file or device with explicit security attributes and returns a safe handle.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", SetLastError = true)]
        public static extern SafeFileHandle CreateFileS(string lpFileName, DesiredAccess dwDesiredAccess, FileShareMode dwShareMode, ref SecurityAttributes lpSecurityAttributes, FileCreationDisposition dwCreationDisposition, FileFlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// Creates or opens a mutex without explicit security attributes.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        /// <summary>
        /// Creates or opens a mutex with explicit security attributes.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateMutex(ref SecurityAttributes lpMutexAttributes, bool bInitialOwner, string lpName);

        /// <summary>
        /// Sends a control code to a device using unmanaged buffers.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        /// <summary>
        /// Sends a control code to a device using managed byte buffers.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(SafeFileHandle device, uint ioControlCode, byte[] inBuffer, int inBufferSize, [Out] byte[] outBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

        /// <summary>
        /// Sends an overlapped control request to a device.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, ref NativeOverlappedData lpOverlapped);

        /// <summary>
        /// Performs an extended function on a communications device.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EscapeCommFunction(IntPtr hFile, uint dwFunc);

        /// <summary>
        /// Releases a loaded dynamic-link library.
        /// </summary>
        [DllImport(DLL_NAME)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr module);

        /// <summary>
        /// Retrieves the number of active processors in a processor group.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern uint GetActiveProcessorCount(ushort groupNumber);

        /// <summary>
        /// Retrieves the number of active processor groups.
        /// </summary>
        [DllImport(DLL_NAME)]
        public static extern ushort GetActiveProcessorGroupCount();

        /// <summary>
        /// Retrieves the configuration of a communications device by native handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCommState(IntPtr hFile, ref Dcb lpDcb);

        /// <summary>
        /// Retrieves the configuration of a communications device by safe handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

        /// <summary>
        /// Retrieves timeout parameters for a communications device.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCommTimeouts(IntPtr hFile, out CommTimeouts lpCommTimeouts);

        /// <summary>
        /// Retrieves whether a device is currently powered on.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDevicePowerState(SafeFileHandle device, [MarshalAs(UnmanagedType.Bool)] out bool isPoweredOn);

        /// <summary>
        /// Retrieves free and total disk space for a path.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailableToCaller, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);

        /// <summary>
        /// Retrieves a module handle from the current process.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        /// <summary>
        /// Retrieves the result of an overlapped operation by native handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetOverlappedResult(IntPtr hFile, IntPtr lpOverlapped, out uint lpNumberOfBytesTransferred, [MarshalAs(UnmanagedType.Bool)] bool bWait);

        /// <summary>
        /// Retrieves the result of an overlapped operation by safe handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetOverlappedResult(SafeFileHandle hFile, ref NativeOverlappedData lpOverlapped, out uint lpNumberOfBytesTransferred, [MarshalAs(UnmanagedType.Bool)] bool bWait);

        /// <summary>
        /// Resolves an exported function from a loaded module.
        /// </summary>
        [DllImport(DLL_NAME, ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr GetProcAddress(IntPtr module, string methodName);

        /// <summary>
        /// Retrieves firmware table data into unmanaged memory.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern uint GetSystemFirmwareTable(uint firmwareTableProviderSignature, uint firmwareTableId, IntPtr firmwareTableBuffer, uint bufferSize);

        /// <summary>
        /// Retrieves firmware table data into a managed byte buffer.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern uint GetSystemFirmwareTable(uint firmwareTableProviderSignature, uint firmwareTableId, [Out] byte[] firmwareTableBuffer, uint bufferSize);

        /// <summary>
        /// Retrieves current physical and virtual memory statistics.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx status);

        /// <summary>
        /// Loads a dynamic-link library into the current process.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Opens an existing named mutex.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenMutex(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        /// <summary>
        /// Discards communications operations or buffers by native handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PurgeComm(IntPtr hFile, uint dwFlags);

        /// <summary>
        /// Discards communications operations or buffers by safe handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PurgeComm(SafeFileHandle hFile, uint dwFlags);

        /// <summary>
        /// Retrieves the target paths for a DOS device name.
        /// </summary>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint QueryDosDevice(string deviceName, char[] targetPath, int maxCharacters);

        /// <summary>
        /// Reads bytes from a native file handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);

        /// <summary>
        /// Reads bytes from a safe file handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(SafeFileHandle device, byte[] buffer, uint numberOfBytesToRead, out uint numberOfBytesRead, IntPtr overlapped);

        /// <summary>
        /// Starts an overlapped read into unmanaged memory.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, ref NativeOverlappedData lpOverlapped);

        /// <summary>
        /// Starts an overlapped read using unmanaged operation state.
        /// </summary>
        [DllImport(DLL_NAME, EntryPoint = "ReadFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFileOverlapped(IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, IntPtr lpNumberOfBytesRead, IntPtr lpOverlapped);

        /// <summary>
        /// Releases ownership of a mutex.
        /// </summary>
        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseMutex(IntPtr hMutex);

        /// <summary>
        /// Sets the event mask for a communications device.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCommMask(IntPtr hFile, uint dwEvtMask);

        /// <summary>
        /// Applies a communications-device configuration by native handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCommState(IntPtr hFile, ref Dcb lpDcb);

        /// <summary>
        /// Applies a communications-device configuration by safe handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

        /// <summary>
        /// Applies communications timeouts by native handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCommTimeouts(IntPtr hFile, ref CommTimeouts lpCommTimeouts);

        /// <summary>
        /// Applies communications timeouts by safe handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCommTimeouts(SafeFileHandle hFile, ref CommTimeouts lpCommTimeouts);

        /// <summary>
        /// Moves the file pointer of a safe file handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetFilePointerEx(SafeFileHandle device, long distanceToMove, IntPtr newFilePointer, uint moveMethod);

        /// <summary>
        /// Configures the input and output buffer sizes of a communications device.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupComm(IntPtr hFile, uint dwInQueue, uint dwOutQueue);

        /// <summary>
        /// Waits until an object is signaled or the timeout elapses.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        /// <summary>
        /// Writes bytes to a native file handle.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);

        /// <summary>
        /// Starts an overlapped write from unmanaged memory.
        /// </summary>
        [DllImport(DLL_NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(SafeFileHandle hFile, IntPtr lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, ref NativeOverlappedData lpOverlapped);

        /// <summary>
        /// Starts an overlapped write using unmanaged operation state.
        /// </summary>
        [DllImport(DLL_NAME, EntryPoint = "WriteFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFileOverlapped(IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToWrite, IntPtr lpNumberOfBytesWritten, IntPtr lpOverlapped);

        #endregion

        #region Public

        /// <summary>
        /// Sends a strongly typed control request to a device.
        /// </summary>
        /// <typeparam name="TInput">The unmanaged input structure type.</typeparam>
        /// <typeparam name="TOutput">The unmanaged output structure type.</typeparam>
        /// <param name="device">The target device handle.</param>
        /// <param name="ioControlCode">The control code to send.</param>
        /// <param name="input">The input structure.</param>
        /// <param name="output">Receives the output structure.</param>
        /// <param name="bytesReturned">Receives the number of bytes written to the output buffer.</param>
        /// <param name="errorCode">Receives the native error code when the operation fails.</param>
        /// <returns><see langword="true"/> when the control request succeeds; otherwise, <see langword="false"/>.</returns>
        public static bool DeviceIoControl<TInput, TOutput>(
            SafeFileHandle device,
            uint ioControlCode,
            TInput input,
            out TOutput output,
            out uint bytesReturned,
            out int errorCode)
            where TInput : unmanaged
            where TOutput : unmanaged
        {
            var inputSize    = Marshal.SizeOf<TInput>();
            var outputSize   = Marshal.SizeOf<TOutput>();
            var inputBuffer  = Marshal.AllocHGlobal(inputSize);
            var outputBuffer = Marshal.AllocHGlobal(outputSize);

            try
            {
                Marshal.StructureToPtr(input, inputBuffer, false);

                var succeeded = DeviceIoControl(
                    device,
                    ioControlCode,
                    inputBuffer,
                    (uint)inputSize,
                    outputBuffer,
                    (uint)outputSize,
                    out bytesReturned,
                    IntPtr.Zero);

                errorCode = succeeded
                    ? 0
                    : Marshal.GetLastWin32Error();
                output = succeeded
                    ? Marshal.PtrToStructure<TOutput>(outputBuffer)
                    : default;

                return succeeded;
            }
            finally
            {
                Marshal.FreeHGlobal(outputBuffer);
                Marshal.FreeHGlobal(inputBuffer);
            }
        }

        /// <summary>
        /// Creates an exception for the error captured by the most recent Windows API call.
        /// </summary>
        /// <returns>An exception containing the native error code and message.</returns>
        public static Win32Exception LastWin32Exception()
        {
            return new Win32Exception(Marshal.GetLastWin32Error());
        }

        #endregion
    }
}
