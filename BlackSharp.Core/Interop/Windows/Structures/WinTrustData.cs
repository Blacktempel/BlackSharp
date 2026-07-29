/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using System.Runtime.InteropServices;

namespace BlackSharp.Core.Interop.Windows.Structures;

/// <summary>
/// Defines the policy and subject data supplied to a Windows trust verification request.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct WinTrustData
{
    /// <summary>
    /// The size of this structure in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// Optional policy-provider callback data.
    /// </summary>
    public IntPtr PolicyCallbackData;

    /// <summary>
    /// Optional subject-interface-package client data.
    /// </summary>
    public IntPtr SipClientData;

    /// <summary>
    /// The user-interface behavior for the trust operation.
    /// </summary>
    public uint UiChoice;

    /// <summary>
    /// The requested certificate-revocation checks.
    /// </summary>
    public uint RevocationChecks;

    /// <summary>
    /// Identifies the kind of subject information supplied.
    /// </summary>
    public uint UnionChoice;

    /// <summary>
    /// A pointer to the selected subject information.
    /// </summary>
    public IntPtr File;

    /// <summary>
    /// The action to perform on trust-provider state.
    /// </summary>
    public uint StateAction;

    /// <summary>
    /// The trust-provider state handle.
    /// </summary>
    public IntPtr StateData;

    /// <summary>
    /// A pointer to an optional URL reference.
    /// </summary>
    public IntPtr UrlReference;

    /// <summary>
    /// Flags controlling trust-provider behavior.
    /// </summary>
    public uint ProviderFlags;

    /// <summary>
    /// Identifies the context in which the user interface is displayed.
    /// </summary>
    public uint UiContext;

    /// <summary>
    /// A pointer to optional signature settings.
    /// </summary>
    public IntPtr SignatureSettings;
}
