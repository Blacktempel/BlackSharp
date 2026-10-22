// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Interop.Windows.Native;
using BlackSharp.Core.Interop.Windows.Structures;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.Security.Windows
{
    /// <summary>
    /// Protects UTF-8 text with Windows DPAPI for the current user account.
    /// </summary>
    public static class WindowsDataProtector
    {
        #region Fields

        private const int CryptProtectUiForbidden = 0x1;

        #endregion

        #region Public

        /// <summary>
        /// Protects text for the current Windows user and returns a Base64 payload.
        /// </summary>
        /// <param name="plaintext">The text to protect.</param>
        /// <returns>A Base64 payload, or an empty string for empty input.</returns>
        public static string Protect(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return string.Empty;
            }

            EnsureWindows();

            var bytes = Encoding.UTF8.GetBytes(plaintext);

            try
            {
                return Convert.ToBase64String(Transform(bytes, true));
            }
            finally
            {
                Array.Clear(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Unprotects a Base64 payload for the current Windows user.
        /// </summary>
        /// <param name="protectedValue">The protected Base64 payload.</param>
        /// <returns>The unprotected text, or an empty string for empty input.</returns>
        public static string Unprotect(string protectedValue)
        {
            if (string.IsNullOrEmpty(protectedValue))
            {
                return string.Empty;
            }

            EnsureWindows();

            var protectedBytes = Convert.FromBase64String(protectedValue);
            byte[] plaintextBytes = null;

            try
            {
                plaintextBytes = Transform(protectedBytes, false);

                return Encoding.UTF8.GetString(plaintextBytes);
            }
            finally
            {
                Array.Clear(protectedBytes, 0, protectedBytes.Length);

                if (plaintextBytes != null)
                {
                    Array.Clear(plaintextBytes, 0, plaintextBytes.Length);
                }
            }
        }

        #endregion

        #region Private

        private static void EnsureWindows()
        {
            if (!PlatformOperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("Windows DPAPI is available only on Windows.");
            }
        }

        private static unsafe byte[] Transform(byte[] input, bool protect)
        {
            var inputPointer = Marshal.AllocHGlobal(input.Length);

            var inputBlob  = new DataBlob { DataLength = input.Length, Data = inputPointer };
            var outputBlob = new DataBlob();

            var description = IntPtr.Zero;

            try
            {
                Marshal.Copy(input, 0, inputPointer, input.Length);

                var success = protect
                    ? Crypt32.CryptProtectData  (ref inputBlob, null           , IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CryptProtectUiForbidden, ref outputBlob)
                    : Crypt32.CryptUnprotectData(ref inputBlob, out description, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, CryptProtectUiForbidden, ref outputBlob);

                if (!success)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var result = new byte[outputBlob.DataLength];
                Marshal.Copy(outputBlob.Data, result, 0, result.Length);

                return result;
            }
            finally
            {
                new Span<byte>((void*)inputPointer, input.Length).Clear();

                Marshal.FreeHGlobal(inputPointer);

                if (outputBlob.Data != IntPtr.Zero)
                {
                    Kernel32.LocalFree(outputBlob.Data);
                }

                if (description != IntPtr.Zero)
                {
                    Kernel32.LocalFree(description);
                }
            }
        }

        #endregion
    }
}
