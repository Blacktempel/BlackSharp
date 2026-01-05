/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Controls;
using BlackSharp.MVVM.Platform.Interfaces;
using System.Windows.Input;

namespace BlackSharp.UI.Avalonia.Platform.Interfaces
{
    /// <summary>
    /// Manages the registration and handling of hot keys within a top-level application window.
    /// </summary>
    /// <typeparam name="TIdentifier">The enumeration type used to uniquely identify each hot key.</typeparam>
    public interface IHotKeyManager<TIdentifier>
        where TIdentifier : Enum
    {
        /// <summary>
        /// Enables global hot key handling for the specified top-level window.
        /// </summary>
        /// <param name="topLevel">The top-level window for which to enable hot key handling.</param>
        void EnableHotKeyHandling(TopLevel topLevel);

        /// <summary>
        /// Disables hot key handling for the specified top-level window.
        /// </summary>
        /// <param name="topLevel">The top-level window for which hot key handling will be disabled.</param>
        void DisableHotKeyHandling(TopLevel topLevel);

        /// <summary>
        /// Registers a hot key and associates it with the specified command to be executed when the hot key is triggered.
        /// </summary>
        /// <param name="hotKey">The hot key to register. Defines the key combination that will trigger the command. Cannot be null.</param>
        /// <param name="command">The command to execute when the hot key is activated. Cannot be null.</param>
        void RegisterHotKey(IHotKey<TIdentifier> hotKey, ICommand command);

        /// <summary>
        /// Unregisters a hot key.
        /// </summary>
        /// <param name="hotKey">The hot key to unregister. Cannot be null.</param>
        void UnregisterHotKey(IHotKey<TIdentifier> hotKey);

        /// <summary>
        /// Clears all registered hot keys.
        /// </summary>
        void ClearHotKeys();
    }
}
