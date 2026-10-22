// SPDX-License-Identifier: MPL-2.0

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
