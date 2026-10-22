// SPDX-License-Identifier: MPL-2.0

using System.ComponentModel;

namespace BlackSharp.Core.Asynchronous
{
    /// <summary>
    /// Asynchronous executor class.
    /// </summary>
    public sealed class Executor
    {
        #region Public API

        /// <summary>
        /// Executes a specified action via <see cref="BackgroundWorker"/>.
        /// </summary>
        /// <param name="task">Task to execute.</param>
        /// <param name="onError">Action to execute if an error occurs.</param>
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
