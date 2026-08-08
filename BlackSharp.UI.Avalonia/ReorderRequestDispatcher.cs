/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.MVVM.Events;
using System.Windows.Input;

namespace BlackSharp.UI.Avalonia
{
    internal static class ReorderRequestDispatcher
    {
        #region Internal

        internal static void Dispatch(
            object sender,
            ItemReorderRequestedEventArgs e,
            EventHandler<ItemReorderRequestedEventArgs> eventHandler,
            ICommand command,
            Func<int, int, bool> fallback)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            eventHandler?.Invoke(sender, e);

            if (!e.Handled && command?.CanExecute(e) == true)
            {
                command.Execute(e);
                e.Handled = true;
            }

            if (!e.Handled && fallback != null)
            {
                e.Handled = fallback(e.OldIndex, e.NewIndex);
            }
        }

        #endregion
    }
}
