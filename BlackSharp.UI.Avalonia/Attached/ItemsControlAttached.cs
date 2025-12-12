/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia;
using Avalonia.Controls;

namespace BlackSharp.UI.Avalonia.Attached
{
    /// <summary>
    /// Provides attached properties and helper methods for customizing the layout and behavior of the last child
    /// element in an <see cref="ItemsControl"/>.
    /// </summary>
    public static class ItemsControlAttached
    {
        #region Constructor

        static ItemsControlAttached()
        {
            LastChildMarginProperty  .Changed.AddClassHandler<ItemsControl>(OnLastChildMarginChanged);
            LastChildDataTypeProperty.Changed.AddClassHandler<ItemsControl>(OnLastChildMarginChanged);
        }

        #endregion

        #region XAML Properties

        /// <summary>
        /// Identifies the LastChildMargin attached property, which specifies the margin to apply to the last child
        /// element of an <see cref="ItemsControl"/>.
        /// </summary>
        /// <remarks>This attached property can be set on an <see cref="ItemsControl"/> to provide custom spacing for
        /// the last item in the control.<br/>
        /// It is typically used in XAML to adjust layout or visual appearance.<br/>
        /// The property value is a <see cref="Thickness"/> structure, or null to indicate no additional margin.</remarks>
        public static readonly AttachedProperty<Thickness?> LastChildMarginProperty =
            AvaloniaProperty.RegisterAttached<ItemsControl, Thickness?>(
                "LastChildMargin", typeof(ItemsControlAttached));

        /// <summary>
        /// Identifies the LastChildDataType attached property, which specifies the data type of the last child item in
        /// an <see cref="ItemsControl"/>.
        /// </summary>
        /// <remarks>This property is intended for use with <see cref="ItemsControl"/> elements to associate a <see cref="Type"/>
        /// value representing the data type of the last child.<br/>
        /// The value should match the <see cref="StyledElement.DataContext"/> type expected by the container.<br/>
        /// This property can be used in data templates or custom controls to enable type-specific
        /// behavior for the last item in a collection.</remarks>
        public static readonly AttachedProperty<Type> LastChildDataTypeProperty =
            AvaloniaProperty.RegisterAttached<ItemsControl, Type>(
                "LastChildDataType", typeof(ItemsControlAttached));

        #endregion

        #region Properties

        /// <summary>
        /// Sets the margin to apply to the last child element of a container.
        /// </summary>
        /// <param name="element">The object on which to set the last child margin property.</param>
        /// <param name="value">The margin value to set. Specify null to clear the value.</param>
        public static void SetLastChildMargin(AvaloniaObject element, Thickness? value) =>
            element.SetValue(LastChildMarginProperty, value);

        /// <summary>
        /// Gets the margin value applied to the last child element of the specified object.
        /// </summary>
        /// <param name="element">The object from which to retrieve the last child margin. Cannot be null.</param>
        /// <returns>A <see cref="Thickness"/> representing the margin applied to the last child, or <see langword="null"/> if no
        /// margin is set.</returns>
        public static Thickness? GetLastChildMargin(AvaloniaObject element) =>
            element.GetValue(LastChildMarginProperty);

        /// <summary>
        /// Sets the data type of the last child element for the specified object.
        /// </summary>
        /// <param name="element">The object on which to set the last child data type. Cannot be null.</param>
        /// <param name="value">The data type to assign to the last child element. Can be null to clear the value.</param>
        public static void SetLastChildDataType(AvaloniaObject element, Type value) =>
            element.SetValue(LastChildDataTypeProperty, value);

        /// <summary>
        /// Gets the data type of the last child element associated with the specified object.
        /// </summary>
        /// <param name="element">The object from which to retrieve the last child data type.</param>
        /// <returns>A Type representing the data type of the last child element, or null if no data type is set.</returns>
        public static Type GetLastChildDataType(AvaloniaObject element) =>
            element.GetValue(LastChildDataTypeProperty);

        /// <summary>
        /// Identifies the OriginalMargin attached property, which stores the original margin value for a control before
        /// any modifications are applied.
        /// </summary>
        /// <remarks>This property can be used to preserve and restore a controls initial margin, for
        /// example when applying temporary layout changes.<br/>
        /// The property is intended for use with controls that may have their Margin property altered dynamically.</remarks>
        static readonly AttachedProperty<Thickness?> OriginalMarginProperty =
            AvaloniaProperty.RegisterAttached<Control, Thickness?>(
                "OriginalMargin", typeof(ItemsControlAttached));

        #endregion

        #region Private

        static void OnLastChildMarginChanged(ItemsControl itemsControl, AvaloniaPropertyChangedEventArgs e)
        {
            itemsControl.ContainerPrepared -= ItemsControlContainerPrepared;
            itemsControl.ContainerPrepared += ItemsControlContainerPrepared;

            itemsControl.ContainerClearing -= ItemsControlContainerClearing;
            itemsControl.ContainerClearing += ItemsControlContainerClearing;

            Apply(itemsControl);
        }

        static void ItemsControlContainerClearing(object sender, ContainerClearingEventArgs e)
        {
            if (e.Container is Control c)
            {
                //Remove original margin when container is cleared
                c.ClearValue(OriginalMarginProperty);
            }

            if (sender is ItemsControl ic)
            {
                Apply(ic);
            }
        }

        static void ItemsControlContainerPrepared(object sender, ContainerPreparedEventArgs e)
        {
            if (sender is ItemsControl ic)
            {
                var container = e.Container;

                //Set original margin, if not already done
                if (container.GetValue(OriginalMarginProperty) == null)
                {
                    container.SetValue(OriginalMarginProperty, container.Margin);
                }

                Apply(ic);
            }
        }

        static void Apply(ItemsControl itemsControl)
        {
            var desiredMargin = GetLastChildMargin(itemsControl);
            var requiredType = GetLastChildDataType(itemsControl);

            var containers = itemsControl.GetRealizedContainers()
                .OfType<Control>()
                .ToList();

            if (containers.Count == 0)
            {
                return;
            }

            //Set target index: either last container in general or last whose DataContext matches the type
            int targetIndex = -1;
            if (requiredType == null)
            {
                targetIndex = containers.Count - 1;
            }
            else
            {
                for (int i = containers.Count - 1; i >= 0; --i)
                {
                    var dc = containers[i].DataContext;
                    if (dc != null && requiredType.IsInstanceOfType(dc))
                    {
                        targetIndex = i;
                        break;
                    }
                }
            }

            //Reset all to original value (except possibly target container, which is set below)
            for (int i = 0; i < containers.Count; ++i)
            {
                if (i == targetIndex)
                {
                    continue;
                }

                var c = containers[i];

                var orig = c.GetValue(OriginalMarginProperty);
                if (orig != null && c.Margin != orig.Value)
                {
                    c.Margin = orig.Value;
                }
            }

            //Set desired margin if target exists
            if (targetIndex >= 0)
            {
                var last = containers[targetIndex];

                //Save original if not stored
                var stored = last.GetValue(OriginalMarginProperty);
                if (stored == null)
                {
                    last.SetValue(OriginalMarginProperty, last.Margin);
                }

                if (last.Margin != desiredMargin)
                {
                    last.Margin = desiredMargin ?? default;
                }
            }
        }

        #endregion
    }
}
