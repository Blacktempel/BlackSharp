/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Provides guarded event invocation for independent external subscribers.
    /// </summary>
    public static class EventHandlerExtensions
    {
        #region Public

        /// <summary>
        /// Invokes each subscriber independently and suppresses exceptions thrown by individual handlers.
        /// </summary>
        /// <param name="handlers">The subscribed handlers.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event data.</param>
        public static void InvokeAllSafely(
            this EventHandler handlers,
            object sender,
            EventArgs eventArgs)
        {
            if (handlers == null)
            {
                return;
            }

            foreach (EventHandler handler in handlers.GetInvocationList())
            {
                try
                {
                    handler(sender, eventArgs);
                }
                catch
                {
                    //Independent subscribers must not prevent later subscribers from receiving the event.
                }
            }
        }

        /// <summary>
        /// Invokes each subscriber independently and suppresses exceptions thrown by individual handlers.
        /// </summary>
        /// <typeparam name="TEventArgs">The event-data type.</typeparam>
        /// <param name="handlers">The subscribed handlers.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event data.</param>
        public static void InvokeAllSafely<TEventArgs>(
            this EventHandler<TEventArgs> handlers,
            object sender,
            TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            if (handlers == null)
            {
                return;
            }

            foreach (EventHandler<TEventArgs> handler in handlers.GetInvocationList())
            {
                try
                {
                    handler(sender, eventArgs);
                }
                catch
                {
                    //Independent subscribers must not prevent later subscribers from receiving the event.
                }
            }
        }

        #endregion
    }
}
