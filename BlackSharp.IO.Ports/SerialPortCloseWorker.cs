/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

namespace BlackSharp.IO.Ports;

/// <summary>
/// Single, process-wide close worker for serial backends.
/// </summary>
internal static class SerialPortCloseWorker
{
    #region Fields

    private static readonly object SyncRoot = new object();
    private static readonly Queue<CloseRequest> Queue = new Queue<CloseRequest>();
    private static Thread _thread;

    #endregion

    #region Public

    public static bool Close(ISerialPortBackend backend, TimeSpan timeout, out Exception exception)
    {
        if (backend == null)
        {
            throw new ArgumentNullException(nameof(backend));
        }

        var request = new CloseRequest(backend);

        lock (SyncRoot)
        {
            EnsureThreadStartedNoLock();
            Queue.Enqueue(request);
            Monitor.Pulse(SyncRoot);
        }

        bool completed = timeout == Timeout.InfiniteTimeSpan
            ? request.Completed.Wait(Timeout.Infinite)
            : request.Completed.Wait(timeout);

        if (!completed)
        {
            exception = null;
            return false;
        }

        exception = request.Exception;
        return true;
    }

    #endregion

    #region Private

    private static void EnsureThreadStartedNoLock()
    {
        if (_thread != null)
        {
            return;
        }

        _thread = new Thread(WorkerLoop)
        {
            IsBackground = true,
            Name = typeof(SerialPortCloseWorker).FullName
        };

        _thread.Start();
    }

    private static void WorkerLoop()
    {
        while (true)
        {
            CloseRequest request;

            lock (SyncRoot)
            {
                while (Queue.Count == 0)
                {
                    Monitor.Wait(SyncRoot);
                }

                request = Queue.Dequeue();
            }

            request.Execute();
        }
    }

    #endregion

    #region Nested types

    private sealed class CloseRequest
    {
        #region Constructor

        public CloseRequest(ISerialPortBackend backend)
        {
            _backend = backend;
        }

        #endregion

        #region Fields

        private readonly ISerialPortBackend _backend;

        #endregion

        #region Properties

        public ManualResetEventSlim Completed { get; } = new ManualResetEventSlim(false);

        public Exception Exception { get; private set; }

        #endregion

        #region Public

        public void Execute()
        {
            try
            {
                _backend.RequestAbort();
                _backend.CloseCore();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                Completed.Set();
            }
        }

        #endregion
    }

    #endregion
}
