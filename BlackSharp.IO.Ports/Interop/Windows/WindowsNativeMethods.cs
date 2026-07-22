/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.IO.Ports.Interop.Windows.Structures;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlackSharp.IO.Ports.Interop.Windows;

internal static class WindowsNativeMethods
{
    #region Fields

    const string DLL_NAME = "kernel32.dll";

    public static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

    public const uint PurgeTxAbort = 0x0001;
    public const uint PurgeRxAbort = 0x0002;
    public const uint PurgeTxClear = 0x0004;
    public const uint PurgeRxClear = 0x0008;

    public const uint Clrdtr = 6;
    public const uint Setdtr = 5;
    public const uint Clrrts = 4;
    public const uint Setrts = 3;

    public const int ErrorOperationAborted =  995;
    public const int ErrorIoIncomplete     =  996;
    public const int ErrorIoPending        =  997;
    public const int ErrorNotFound         = 1168;

    public const uint WaitObject0 = 0x00000000;
    public const uint WaitTimeout = 0x00000102;
    public const uint WaitFailed  = 0xFFFFFFFF;
    public const uint Infinite    = 0xFFFFFFFF;

    public const byte Noparity    = 0;
    public const byte Oddparity   = 1;
    public const byte Evenparity  = 2;
    public const byte Markparity  = 3;
    public const byte Spaceparity = 4;

    public const byte Onestopbit   = 0;
    public const byte One5stopbits = 1;
    public const byte Twostopbits  = 2;

    #endregion

    #region Public

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadFile(
        IntPtr hFile,
        byte[] lpBuffer,
        int nNumberOfBytesToRead,
        out int lpNumberOfBytesRead,
        IntPtr lpOverlapped);

    [DllImport(DLL_NAME, EntryPoint = nameof(ReadFile), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadFile(
        SafeFileHandle hFile,
        IntPtr lpBuffer,
        uint nNumberOfBytesToRead,
        out uint lpNumberOfBytesRead,
        ref NativeOverlappedData lpOverlapped);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteFile(
        IntPtr hFile,
        byte[] lpBuffer,
        int nNumberOfBytesToWrite,
        out int lpNumberOfBytesWritten,
        IntPtr lpOverlapped);

    [DllImport(DLL_NAME, EntryPoint = nameof(WriteFile), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteFile(
        SafeFileHandle hFile,
        IntPtr lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        ref NativeOverlappedData lpOverlapped);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeviceIoControl(
        SafeFileHandle hDevice,
        uint dwIoControlCode,
        IntPtr lpInBuffer,
        uint nInBufferSize,
        IntPtr lpOutBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        ref NativeOverlappedData lpOverlapped);

    [DllImport(DLL_NAME, EntryPoint = nameof(ReadFile), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadFileOverlapped(
        IntPtr hFile,
        IntPtr lpBuffer,
        int nNumberOfBytesToRead,
        IntPtr lpNumberOfBytesRead,
        IntPtr lpOverlapped);

    [DllImport(DLL_NAME, EntryPoint = nameof(WriteFile), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteFileOverlapped(
        IntPtr hFile,
        IntPtr lpBuffer,
        int nNumberOfBytesToWrite,
        IntPtr lpNumberOfBytesWritten,
        IntPtr lpOverlapped);

    [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateEventW(
        IntPtr lpEventAttributes,
        [MarshalAs(UnmanagedType.Bool)] bool bManualReset,
        [MarshalAs(UnmanagedType.Bool)] bool bInitialState,
        string lpName);

    [DllImport(DLL_NAME, SetLastError = true)]
    public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetOverlappedResult(
        IntPtr hFile,
        IntPtr lpOverlapped,
        out uint lpNumberOfBytesTransferred,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    [DllImport(DLL_NAME, EntryPoint = nameof(GetOverlappedResult), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetOverlappedResult(
        SafeFileHandle hFile,
        ref NativeOverlappedData lpOverlapped,
        out uint lpNumberOfBytesTransferred,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupComm(IntPtr hFile, uint dwInQueue, uint dwOutQueue);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCommState(IntPtr hFile, ref Dcb lpDcb);

    [DllImport(DLL_NAME, EntryPoint = nameof(GetCommState), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCommState(IntPtr hFile, ref Dcb lpDcb);

    [DllImport(DLL_NAME, EntryPoint = nameof(SetCommState), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCommTimeouts(IntPtr hFile, out CommTimeouts lpCommTimeouts);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCommTimeouts(IntPtr hFile, ref CommTimeouts lpCommTimeouts);

    [DllImport(DLL_NAME, EntryPoint = nameof(SetCommTimeouts), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCommTimeouts(SafeFileHandle hFile, ref CommTimeouts lpCommTimeouts);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PurgeComm(IntPtr hFile, uint dwFlags);

    [DllImport(DLL_NAME, EntryPoint = nameof(PurgeComm), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PurgeComm(SafeFileHandle hFile, uint dwFlags);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ClearCommError(IntPtr hFile, out uint lpErrors, out ComStat lpStat);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EscapeCommFunction(IntPtr hFile, uint dwFunc);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

    [DllImport(DLL_NAME, EntryPoint = nameof(CancelIoEx), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CancelIoEx(SafeFileHandle hFile, ref NativeOverlappedData lpOverlapped);

    [DllImport(DLL_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCommMask(IntPtr hFile, uint dwEvtMask);

    [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint QueryDosDeviceW(string lpDeviceName, char[] lpTargetPath, uint ucchMax);

    public static Win32Exception LastWin32Exception()
    {
        return new Win32Exception(Marshal.GetLastWin32Error());
    }

    #endregion
}
