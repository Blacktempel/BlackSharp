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
/// Runs native serial-port close operations on a rotating background worker.
/// </summary>
/// <remarks>
/// Some USB/VCP drivers can block forever in abort/close paths.<br/>
/// This worker therefore processes close requests serially until one request times out.<br/>
/// The timed-out worker is then abandoned and a fresh worker is created for future close requests.<br/>
/// Already queued requests are moved to the fresh worker; the timed-out request is allowed to keep running
/// on the abandoned background thread until it returns or the process exits.
/// </remarks>
internal static class SerialPortCloseWorker
{
    #region Fields

    private static readonly object _sync = new object();
    private static CloseWorker _currentWorker;
    private static int _nextWorkerID;

    #endregion

    #region Public

    public static bool Close(ISerialPortBackend backend, TimeSpan timeout, out Exception exception)
    {
        if (backend == null)
        {
            throw new ArgumentNullException(nameof(backend));
        }

        var request = new CloseRequest(backend);
        CloseWorker worker = Enqueue(request);

        bool completed = timeout == Timeout.InfiniteTimeSpan
            ? request.Completed.Wait(Timeout.Infinite)
            : request.Completed.Wait(timeout);

        if (!completed)
        {
            request.MarkTimedOut();
            RotateWorker(worker);

            exception = null;
            return false;
        }

        exception = request.Exception;
        request.Dispose();

        return true;
    }

    #endregion

    #region Private

    private static CloseWorker Enqueue(CloseRequest request)
    {
        lock (_sync)
        {
            CloseWorker worker = EnsureCurrentWorkerLocked();

            worker.Enqueue(request);

            return worker;
        }
    }

    private static CloseWorker EnsureCurrentWorkerLocked()
    {
        if (_currentWorker == null || !_currentWorker.AcceptsRequests)
        {
            _currentWorker = new CloseWorker(++_nextWorkerID);
            _currentWorker.Start();
        }

        return _currentWorker;
    }

    private static void RotateWorker(CloseWorker worker)
    {
        lock (_sync)
        {
            List<CloseRequest> pendingRequests = worker.AbandonAndTakePendingRequests();

            if (ReferenceEquals(_currentWorker, worker))
            {
                _currentWorker = new CloseWorker(++_nextWorkerID);
                _currentWorker.Start();
            }

            if (pendingRequests.Count == 0)
            {
                return;
            }

            CloseWorker currentWorker = EnsureCurrentWorkerLocked();

            foreach (CloseRequest pendingRequest in pendingRequests)
            {
                currentWorker.Enqueue(pendingRequest);
            }
        }
    }

    #endregion

    #region Nested types

    private sealed class CloseWorker
    {
        #region Constructor

        public CloseWorker(int id)
        {
            _thread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = $"{typeof(SerialPortCloseWorker).FullName} #{id}"
            };
        }

        #endregion

        #region Fields

        private readonly object _sync = new object();
        private readonly Queue<CloseRequest> _queue = new Queue<CloseRequest>();
        private readonly Thread _thread;
        private bool _abandoned;

        #endregion

        #region Properties

        public bool AcceptsRequests
        {
            get
            {
                lock (_sync)
                    return !_abandoned;
            }
        }

        #endregion

        #region Public

        public void Start()
        {
            _thread.Start();
        }

        public void Enqueue(CloseRequest request)
        {
            lock (_sync)
            {
                if (_abandoned)
                {
                    throw new InvalidOperationException("This serial port close worker has been abandoned.");
                }

                _queue.Enqueue(request);
                Monitor.Pulse(_sync);
            }
        }

        public List<CloseRequest> AbandonAndTakePendingRequests()
        {
            lock (_sync)
            {
                if (_abandoned)
                {
                    return new List<CloseRequest>();
                }

                _abandoned = true;

                var pendingRequests = new List<CloseRequest>(_queue.Count);
                while (_queue.Count > 0)
                {
                    pendingRequests.Add(_queue.Dequeue());
                }

                Monitor.PulseAll(_sync);
                return pendingRequests;
            }
        }

        #endregion

        #region Private

        private void WorkerLoop()
        {
            while (true)
            {
                CloseRequest request = TakeNextRequest();
                if (request == null)
                {
                    return;
                }

                request.Execute();

                if (request.IsTimedOut)
                {
                    request.Dispose();
                }

                lock (_sync)
                {
                    if (_abandoned)
                    {
                        return;
                    }
                }
            }
        }

        private CloseRequest TakeNextRequest()
        {
            lock (_sync)
            {
                while (_queue.Count == 0 && !_abandoned)
                {
                    Monitor.Wait(_sync);
                }

                if (_queue.Count == 0)
                {
                    return null;
                }

                return _queue.Dequeue();
            }
        }

        #endregion
    }

    private sealed class CloseRequest : IDisposable
    {
        #region Constructor

        public CloseRequest(ISerialPortBackend backend)
        {
            _backend = backend;
        }

        #endregion

        #region Fields

        private readonly ISerialPortBackend _backend;
        private int _timedOut;

        #endregion

        #region Properties

        public ManualResetEventSlim Completed { get; } = new ManualResetEventSlim(false);

        public Exception Exception { get; private set; }

        public bool IsTimedOut => Volatile.Read(ref _timedOut) != 0;

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

        public void MarkTimedOut()
        {
            Volatile.Write(ref _timedOut, 1);
        }

        public void Dispose()
        {
            Completed.Dispose();
        }

        #endregion
    }

    #endregion
}
