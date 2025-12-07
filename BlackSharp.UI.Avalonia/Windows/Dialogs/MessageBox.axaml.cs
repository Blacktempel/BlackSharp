/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.UI.Avalonia.Windows.Dialogs.Enums;

namespace BlackSharp.UI.Avalonia.Windows.Dialogs;

/// <summary>
/// Represents a dialog box that displays a message to the user and optionally provides buttons for user interaction.
/// </summary>
public partial class MessageBox : DialogBase
{
    #region Constructor

    /// <summary>
    /// <inheritdoc cref="DialogBase()"/>
    /// </summary>
    public MessageBox()
        : this(DialogType.None)
    {
    }

    /// <summary>
    /// <inheritdoc cref="DialogBase(DialogType)"/>
    /// </summary>
    /// <param name="type"><inheritdoc/></param>
    public MessageBox(DialogType type)
        : this(type, DialogSize.Medium)
    {
    }

    /// <summary>
    /// <inheritdoc cref="DialogBase(DialogSize)"/>
    /// </summary>
    /// <param name="size"><inheritdoc/></param>
    public MessageBox(DialogSize size)
        : this(DialogType.None, size)
    {
    }

    /// <summary>
    /// <inheritdoc cref="DialogBase(DialogType, DialogSize)"/>
    /// </summary>
    /// <param name="type"><inheritdoc/></param>
    /// <param name="size"><inheritdoc/></param>
    public MessageBox(DialogType type, DialogSize size)
        : base(type, size)
    {
        InitializeComponent();
    }

    #endregion
}
