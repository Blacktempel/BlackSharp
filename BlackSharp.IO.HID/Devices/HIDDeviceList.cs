/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.IO.HID;

/// <summary>
/// Provides HID enumeration and change notifications for the local computer.
/// </summary>
public sealed class HIDDeviceList
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="HIDDeviceList"/> class.
    /// </summary>
    private HIDDeviceList()
    {
        _knownPaths = TryGetCurrentPaths(out var paths)
            ? paths
            : Array.Empty<string>();

        _pollTimer = new Timer(Poll, null, Timeout.Infinite, Timeout.Infinite);
    }

    #endregion

    #region Fields

    /// <summary>
    /// Defines the fixed poll interval milliseconds value used by this component.
    /// </summary>
    private const int PollIntervalMilliseconds = 1000;

    /// <summary>
    /// Stores the known local entries used by this component.
    /// </summary>
    private static readonly Lazy<HIDDeviceList> s_local = new(() => new HIDDeviceList());

    /// <summary>
    /// Stores the poll timer state associated with this instance.
    /// </summary>
    private readonly Timer _pollTimer;

    /// <summary>
    /// Synchronizes access to state shared by concurrent operations.
    /// </summary>
    private readonly object _sync = new();

    /// <summary>
    /// Stores the changed state associated with this instance.
    /// </summary>
    private EventHandler _changed;

    /// <summary>
    /// Stores the known known paths entries used by this component.
    /// </summary>
    private string[] _knownPaths;

    /// <summary>
    /// Stores the polling state associated with this instance.
    /// </summary>
    private int _polling;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the HID device list for the local computer.
    /// </summary>
    public static HIDDeviceList Local => s_local.Value;

    #endregion

    #region Public

    /// <summary>
    /// Gets the currently present HID devices.
    /// </summary>
    public IReadOnlyList<HIDDevice> GetDevices()
    {
        return HIDDeviceEnumerator.GetDevices();
    }

    /// <summary>
    /// Occurs when a HID interface is added or removed.
    /// </summary>
    public event EventHandler Changed
    {
        add
        {
            lock (_sync)
            {
                var startPolling = _changed == null;

                _changed += value;

                if (startPolling && _changed != null)
                {
                    if (TryGetCurrentPaths(out var paths))
                    {
                        _knownPaths = paths;
                    }

                    _pollTimer.Change(PollIntervalMilliseconds, PollIntervalMilliseconds);
                }
            }
        }
        remove
        {
            lock (_sync)
            {
                _changed -= value;

                if (_changed == null)
                {
                    _pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// Attempts to get current paths.
    /// </summary>
    /// <param name="paths">When this method returns, contains the paths result.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetCurrentPaths(out string[] paths)
    {
        try
        {
            paths = HIDDeviceEnumerator.GetDevicePaths()
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return true;
        }
        catch
        {
            paths = null;

            return false;
        }
    }

    /// <summary>
    /// Processes poll for the HID device list component.
    /// </summary>
    /// <param name="state">The state used by the operation.</param>
    private void Poll(object state)
    {
        // Reconcile the latest enumeration with the retained device set, emitting additions and removals outside the collection lock.
        if (Interlocked.Exchange(ref _polling, 1) != 0)
        {
            return;
        }

        try
        {
            if (!TryGetCurrentPaths(out var currentPaths))
            {
                return;
            }

            var changed = false;

            lock (_sync)
            {
                if (!_knownPaths.SequenceEqual(currentPaths, StringComparer.OrdinalIgnoreCase))
                {
                    _knownPaths = currentPaths;
                    changed = true;
                }
            }

            if (changed)
            {
                RaiseChanged();
            }
        }
        finally
        {
            Volatile.Write(ref _polling, 0);
        }
    }

    /// <summary>
    /// Processes raise changed for the HID device list component.
    /// </summary>
    private void RaiseChanged()
    {
        EventHandler handlers;

        lock (_sync)
        {
            handlers = _changed;
        }

        if (handlers == null)
        {
            return;
        }

        foreach (EventHandler handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch
            {
                // A subscriber must not terminate the process-wide device watcher.
            }
        }
    }

    #endregion
}
