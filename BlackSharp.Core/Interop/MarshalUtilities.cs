/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop
{
    /// <summary>
    /// Provides reusable helpers for invoking pointer-based native APIs with managed structures.
    /// </summary>
    public static class MarshalUtilities
    {
        #region Public

        /// <summary>
        /// Marshals a structure to unmanaged memory, invokes an action, and copies the updated structure back.
        /// </summary>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <typeparam name="TResult">The native action result type.</typeparam>
        /// <param name="value">The structure passed to and updated by the native action.</param>
        /// <param name="action">The action invoked with a pointer to the structure.</param>
        /// <returns>The result returned by <paramref name="action"/>.</returns>
        public static TResult Invoke<T, TResult>(ref T value, Func<IntPtr, TResult> action)
            where T : struct
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var pointer     = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            var initialized = false;

            try
            {
                Marshal.StructureToPtr(value, pointer, false);
                initialized = true;

                var result = action(pointer);

                value = Marshal.PtrToStructure<T>(pointer);

                return result;
            }
            finally
            {
                if (initialized)
                {
                    Marshal.DestroyStructure<T>(pointer);
                }

                Marshal.FreeHGlobal(pointer);
            }
        }

        /// <summary>
        /// Marshals a structure array to unmanaged memory, invokes an action, and copies updated values back.
        /// </summary>
        /// <typeparam name="T">The structure element type.</typeparam>
        /// <typeparam name="TResult">The native action result type.</typeparam>
        /// <param name="values">The structures passed to and updated by the native action.</param>
        /// <param name="action">The action invoked with a pointer to the first structure.</param>
        /// <returns>The result returned by <paramref name="action"/>.</returns>
        public static TResult Invoke<T, TResult>(T[] values, Func<IntPtr, TResult> action)
            where T : struct
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var elementSize      = Marshal.SizeOf<T>();
            var bufferSize       = checked(elementSize * values.Length);
            var buffer           = Marshal.AllocHGlobal(bufferSize);
            var initializedCount = 0;

            try
            {
                for (var index = 0; index < values.Length; ++index)
                {
                    Marshal.StructureToPtr(values[index], IntPtr.Add(buffer, index * elementSize), false);
                    ++initializedCount;
                }

                var result = action(buffer);

                for (var index = 0; index < initializedCount; ++index)
                {
                    values[index] = Marshal.PtrToStructure<T>(IntPtr.Add(buffer, index * elementSize));
                }

                return result;
            }
            finally
            {
                for (var index = 0; index < initializedCount; ++index)
                {
                    Marshal.DestroyStructure<T>(IntPtr.Add(buffer, index * elementSize));
                }

                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Pins two managed arrays for the duration of a native call.
        /// </summary>
        /// <typeparam name="TFirst">The first array element type.</typeparam>
        /// <typeparam name="TSecond">The second array element type.</typeparam>
        /// <typeparam name="TResult">The native action result type.</typeparam>
        /// <param name="first">The first array to pin.</param>
        /// <param name="second">The second array to pin.</param>
        /// <param name="action">The action invoked with pointers to both arrays.</param>
        /// <returns>The result returned by <paramref name="action"/>.</returns>
        public static TResult InvokePinned<TFirst, TSecond, TResult>(
            TFirst[] first,
            TSecond[] second,
            Func<IntPtr, IntPtr, TResult> action)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var firstHandle  = GCHandle.Alloc(first , GCHandleType.Pinned);
            var secondHandle = GCHandle.Alloc(second, GCHandleType.Pinned);

            try
            {
                return action(firstHandle.AddrOfPinnedObject(), secondHandle.AddrOfPinnedObject());
            }
            finally
            {
                secondHandle.Free();
                firstHandle.Free();
            }
        }

        #endregion
    }
}
