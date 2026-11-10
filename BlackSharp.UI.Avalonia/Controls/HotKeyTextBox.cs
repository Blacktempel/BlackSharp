// SPDX-License-Identifier: MPL-2.0

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace BlackSharp.UI.Avalonia.Controls
{
    /// <summary>
    /// Captures key presses as shortcut text in a read-only text box.
    /// </summary>
    public sealed class HotKeyTextBox : TextBox
    {
        #region Constructor

        /// <summary>
        /// Sets the control's focusable and read-only defaults.
        /// </summary>
        static HotKeyTextBox()
        {
            FocusableProperty .OverrideDefaultValue<HotKeyTextBox>(true);
            IsReadOnlyProperty.OverrideDefaultValue<HotKeyTextBox>(true);
        }

        #endregion

        #region Avalonia Properties

        /// <summary>
        /// Defines the two-way bindable <see cref="HotKey"/> property.
        /// </summary>
        public static readonly StyledProperty<string> HotKeyProperty =
            AvaloniaProperty.Register<HotKeyTextBox, string>(
                nameof(HotKey),
                string.Empty,
                defaultBindingMode: BindingMode.TwoWay);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the shortcut shown in the text box. An empty or null value displays no shortcut.
        /// </summary>
        /// <remarks>Captured shortcuts use the format <c>Ctrl + Shift + 7</c>.</remarks>
        public string HotKey
        {
            get => GetValue(HotKeyProperty);
            set => SetValue(HotKeyProperty, value);
        }

        #endregion

        #region Protected

        /// <inheritdoc/>
        protected override Type StyleKeyOverride => typeof(TextBox);

        /// <summary>
        /// Records the pressed key and its modifiers, clears the shortcut on Backspace or Delete,
        /// and ignores F12 on Windows.
        /// </summary>
        /// <param name="eventArgs">The key-down event marked as handled by the control.</param>
        protected override void OnKeyDown(KeyEventArgs eventArgs)
        {
            // Windows reserves F12 for the debugger.
            if (OperatingSystem.IsWindows() && eventArgs.Key == Key.F12)
            {
                eventArgs.Handled = true;

                return;
            }

            if (eventArgs.Key is Key.Back or Key.Delete)
            {
                SetCurrentValue(HotKeyProperty, string.Empty);
                eventArgs.Handled = true;

                return;
            }

            var values = new List<string>();

            AddModifier(values, eventArgs.KeyModifiers, KeyModifiers.Control, "Ctrl" );
            AddModifier(values, eventArgs.KeyModifiers, KeyModifiers.Alt    , "Alt"  );
            AddModifier(values, eventArgs.KeyModifiers, KeyModifiers.Meta   , "Win"  );
            AddModifier(values, eventArgs.KeyModifiers, KeyModifiers.Shift  , "Shift");

            // A modifier key is already represented by its entry in the modifier list.
            if (!IsModifierKey(eventArgs.Key) && eventArgs.Key != Key.None)
            {
                values.Add(GetKeyName(eventArgs.Key));
            }

            SetCurrentValue(HotKeyProperty, string.Join(" + ", values));
            eventArgs.Handled = true;
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs eventArgs)
        {
            base.OnPointerPressed(eventArgs);
            Focus();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == HotKeyProperty)
            {
                SetCurrentValue(TextProperty, HotKey ?? string.Empty);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Appends the label of an active modifier to the shortcut parts.
        /// </summary>
        /// <param name="values">The shortcut parts to append to.</param>
        /// <param name="modifiers">The modifiers reported by the key event.</param>
        /// <param name="modifier">The modifier flag to check.</param>
        /// <param name="name">The label to append when the flag is set.</param>
        private static void AddModifier(
            ICollection<string> values,
            KeyModifiers modifiers,
            KeyModifiers modifier,
            string name)
        {
            if (modifiers.HasFlag(modifier))
            {
                values.Add(name);
            }
        }

        /// <summary>
        /// Formats the key name, removing the enum prefix from top-row digit keys.
        /// </summary>
        /// <param name="key">The key to format.</param>
        /// <returns>The key name used in the shortcut text.</returns>
        private static string GetKeyName(Key key)
        {
            var name = key.ToString();

            return name.Length == 2 && name[0] == 'D' && char.IsDigit(name[1])
                 ? name[1].ToString()
                 : name;
        }

        /// <summary>
        /// Checks whether the key represents a Control, Alt, Shift, or Windows modifier.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><see langword="true"/> for the left or right variant of these modifier keys.</returns>
        private static bool IsModifierKey(Key key)
        {
            return key is Key.LeftCtrl
                or Key.RightCtrl
                or Key.LeftAlt
                or Key.RightAlt
                or Key.LeftShift
                or Key.RightShift
                or Key.LWin
                or Key.RWin;
        }

        #endregion
    }
}
