/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using System.ComponentModel;

namespace BlackSharp.Core.Asynchronous
{
    /// <summary>
    /// Asynchronous executor class.
    /// </summary>
    public sealed class Executor
    {
        #region Public API

        public static void Run(Func<Action> task, Action<AsyncCompletedEventArgs> onError = null)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, e) => e.Result = task();
            worker.RunWorkerCompleted += (o, e) =>
            {
                if (e.Cancelled || e.Error != null)
                {
                    if (onError == null)
                        throw e.Error;
                    else
                        onError(e);
                }
                else if (e.Result != null)
                    (e.Result as Action)();
            };
            worker.RunWorkerAsync();
        }

        #endregion
    }
}
