/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Threading;
using BlackSharp.UI.Avalonia.Windows.Dialogs.Enums;

namespace BlackSharp.UI.Avalonia.Windows.Dialogs
{
    /// <summary>
    /// Represents a message box dialog that automatically closes after a specified timeout period if no user interaction occurs.
    /// </summary>
    /// <remarks>If the user does not interact with the dialog before the timeout elapses, the dialog closes automatically.<br/>
    /// In this case <see cref="Timeouted"/> is set to <see langword="true"/> to indicate this condition.</remarks>
    public class TimeoutMessageBox : MessageBox
    {
        #region Constructor

        /// <summary>
        /// <inheritdoc cref="MessageBox()"/>
        /// </summary>
        public TimeoutMessageBox(TimeSpan? timeout)
            : this(timeout, DialogType.None)
        {
        }

        /// <summary>
        /// <inheritdoc cref="MessageBox(DialogType)"/>
        /// </summary>
        /// <param name="timeout">Specifies the timeout until the window closes itself if no user interaction has ocurred.</param>
        /// <param name="type"><inheritdoc/></param>
        public TimeoutMessageBox(TimeSpan? timeout, DialogType type)
            : this(timeout, type, DialogSize.Medium)
        {
        }

        /// <summary>
        /// <inheritdoc cref="MessageBox(DialogSize)"/>
        /// </summary>
        /// <param name="timeout">Specifies the timeout until the window closes itself if no user interaction has ocurred.</param>
        /// <param name="size"><inheritdoc/></param>
        public TimeoutMessageBox(TimeSpan? timeout, DialogSize size)
            : this(timeout, DialogType.None, size)
        {
        }

        /// <summary>
        /// <inheritdoc cref="MessageBox(DialogType, DialogSize)"/>
        /// </summary>
        /// <param name="timeout">Specifies the timeout until the window closes itself if no user interaction has ocurred.</param>
        /// <param name="type"><inheritdoc/></param>
        /// <param name="size"><inheritdoc/></param>
        public TimeoutMessageBox(TimeSpan? timeout, DialogType type, DialogSize size)
            : base(type, size)
        {
            InitializeComponent();

            Timeout = timeout;

            _Thread = new Thread(TimeoutThread);
            _Thread.IsBackground = true;
            _Thread.Start();
        }

        #endregion

        #region Fields

        Thread _Thread;

        CancellationTokenSource _CancellationTokenSource = new();

        #endregion

        #region Properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override Type StyleKeyOverride => typeof(MessageBox);

        /// <summary>
        /// Gets the timeout interval for the window to close itself, if no user interaction occurred.
        /// </summary>
        public TimeSpan? Timeout { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation has timed out.<br/>
        /// This means the user has not interacted with the dialog before the timeout interval elapsed.
        /// </summary>
        public bool Timeouted { get; private set; }

        #endregion

        #region Public

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="e"><inheritdoc/></param>
        protected override void OnClosed(EventArgs e)
        {
            if (_Thread != null)
            {
                //If automatic close occurred, Close was already called and this does not modify user interaction state anymore
                _CancellationTokenSource.Cancel();
                _Thread = null;
            }

            base.OnClosed(e);
        }

        #endregion

        #region Private

        void TimeoutThread()
        {
            //Milliseconds
            const int CheckInterval = 250;

            if (Timeout.HasValue)
            {
                var startTime = DateTime.Now;

                //Save original content for later
                Dispatcher.UIThread.Invoke(() =>
                {
                    foreach (var button in DialogButtons)
                    {
                        button.UserData = button.Content;
                    }
                });

                while ((DateTime.Now - startTime) < Timeout.Value
                    && !_CancellationTokenSource.IsCancellationRequested)
                {
                    var elapsed = DateTime.Now - startTime;
                    var remaining = Timeout.Value - elapsed;

                    int sleepTime = (int)Math.Min(remaining.TotalMilliseconds, CheckInterval);
                    if (sleepTime > 0)
                    {
                        Thread.Sleep(sleepTime);
                    }

                    var updateTime = DateTime.Now;

                    Dispatcher.UIThread.Invoke(() =>
                    {
                        var leftTime = Timeout.Value - (updateTime - startTime);
                        if (leftTime < TimeSpan.Zero)
                        {
                            leftTime = TimeSpan.Zero;
                        }

                        foreach (var button in DialogButtons)
                        {
                            button.Content = $"({(int)leftTime.TotalSeconds}) {button.UserData}";
                        }
                    });
                }
            }

            //User has interacted (= cancelled)
            Timeouted = !_CancellationTokenSource.IsCancellationRequested;

            Dispatcher.UIThread.Invoke(Close);
        }

        #endregion
    }
}
