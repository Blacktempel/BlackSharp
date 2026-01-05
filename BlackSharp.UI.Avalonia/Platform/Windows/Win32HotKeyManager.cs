/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Win32.Input;
using BlackSharp.MVVM.Platform.Interfaces;
using BlackSharp.UI.Avalonia.Platform.Interfaces;
using BlackSharp.UI.Avalonia.Platform.Windows.Interop;
using System.Windows.Input;

namespace BlackSharp.UI.Avalonia.Platform.Windows
{
    /// <summary>
    /// <inheritdoc cref="IHotKeyManager{TIdentifier}"/>
    /// </summary>
    /// <typeparam name="TIdentifier"><inheritdoc cref="IHotKeyManager{TIdentifier}"/></typeparam>
    public sealed class Win32HotKeyManager<TIdentifier> : IHotKeyManager<TIdentifier>
        where TIdentifier : Enum
    {
        #region Fields

        IntPtr _Hwnd;

        bool _AreHotKeysRegistered = false;

        readonly Dictionary<TIdentifier, Win32HotKey> _HotKeys = new();

        #endregion

        #region Public

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="topLevel"><inheritdoc/></param>
        public void EnableHotKeyHandling(TopLevel topLevel)
        {
            if (_AreHotKeysRegistered)
            {
                return;
            }

            Win32Properties.AddWndProcHookCallback(topLevel, WndProc);
            _Hwnd = topLevel.TryGetPlatformHandle().Handle;

            _AreHotKeysRegistered = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="topLevel"><inheritdoc/></param>
        public void DisableHotKeyHandling(TopLevel topLevel)
        {
            if (_AreHotKeysRegistered)
            {
                Win32Properties.RemoveWndProcHookCallback(topLevel, WndProc);
                _Hwnd = IntPtr.Zero;

                _AreHotKeysRegistered = false;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="hotKey"><inheritdoc/></param>
        /// <param name="command"><inheritdoc/></param>
        public void RegisterHotKey(IHotKey<TIdentifier> hotKey, ICommand command)
        {
            //Already exists, update Hotkey if necessary
            if (_HotKeys.TryGetValue(hotKey.ID, out var val))
            {
                //Correct ID, check if something has changed
                if (!val.IsHotKeySame(hotKey))
                {
                    //HotKey has changed
                    //We must unregister the old HotKey first, according to MSDN docs
                    User32.UnregisterHotKey(_Hwnd, TIdentifierToInt32(hotKey.ID));

                    //Create Win32 translation
                    var win32HotKey = new Win32HotKey(hotKey, command);

                    //Register HotKey again, with the changes
                    if (User32.RegisterHotKey(_Hwnd, TIdentifierToInt32(hotKey.ID), win32HotKey.Modifiers, win32HotKey.Key))
                    {
                        _HotKeys[hotKey.ID] = win32HotKey;
                    }
                }
            }
            else //Register new HotKey
            {
                //Create Win32 translation
                var win32HotKey = new Win32HotKey(hotKey, command);

                //Register HotKey
                if (User32.RegisterHotKey(_Hwnd, TIdentifierToInt32(hotKey.ID), win32HotKey.Modifiers, win32HotKey.Key))
                {
                    _HotKeys.Add(hotKey.ID, win32HotKey);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="hotKey"><inheritdoc/></param>
        public void UnregisterHotKey(IHotKey<TIdentifier> hotKey)
        {
            //Check if HotKey is registered
            if (_HotKeys.TryGetValue(hotKey.ID, out var val))
            {
                //HotKey exists, remove it
                User32.UnregisterHotKey(_Hwnd, TIdentifierToInt32(hotKey.ID));

                //Also remove from internal list
                _HotKeys.Remove(hotKey.ID);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ClearHotKeys()
        {
            //Unregister all HotKeys
            foreach (var hotKey in _HotKeys.Values)
            {
                User32.UnregisterHotKey(_Hwnd, TIdentifierToInt32(hotKey.HotKeyID));
            }
        }

        #endregion

        #region Private

        IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Shell32.WM_HOTKEY)
            {
                var hotKeyID = Int32ToTIdentifier(wParam.ToInt32());

                if (_HotKeys.TryGetValue(hotKeyID, out var hotKey))
                {
                    if (hotKey.Command.CanExecute(null))
                    {
                        hotKey.Command.Execute(null);
                    }

                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        int TIdentifierToInt32(TIdentifier id)
        {
            return (int)(object)id;
        }

        TIdentifier Int32ToTIdentifier(int id)
        {
            return (TIdentifier)(object)id;
        }

        #endregion

        #region Private Classes

        class Win32HotKey
        {
            #region Constructor

            public Win32HotKey(IHotKey<TIdentifier> hotKey, ICommand command)
            {
                HotKeyID = hotKey.ID;

                //Modifiers are same in both Avalonia and Win32
                Modifiers = (uint)hotKey.Modifiers;
                Key = (uint)KeyInterop.VirtualKeyFromKey((Key)hotKey.Key);

                Command = command;
            }

            #endregion

            #region Properties

            public TIdentifier HotKeyID { get; }

            public uint Modifiers { get; }

            public uint Key { get; }

            public ICommand Command { get; }

            #endregion

            #region Public

            public bool IsHotKeySame(IHotKey<TIdentifier> hotKey)
            {
                return HotKeyID.Equals(hotKey.ID)
                    && Modifiers == (uint)hotKey.Modifiers
                    && Key == (uint)KeyInterop.VirtualKeyFromKey((Key)hotKey.Key);
            }

            #endregion
        }

        #endregion
    }
}
