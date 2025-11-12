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
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BlackSharp.UI.Avalonia.Global
{
    /// <summary>
    /// Manager for global resources.
    /// </summary>
    public class GlobalResourceManager
    {
        #region Constructor

        static GlobalResourceManager()
        {
            UpdateGlobalResources();
        }

        #endregion

        #region Fields

        static List<Style> _Styles = new();

        #endregion

        #region Properties

        #region GlobalFontFamily

        static object _LockObject_GFF = new object();
        static FontFamily _GlobalFontFamily = FontManager.Current.DefaultFontFamily;

        /// <summary>
        /// Gets or sets the global <see cref="FontFamily"/>.
        /// </summary>
        public static FontFamily GlobalFontFamily
        {
            get
            {
                lock (_LockObject_GFF)
                {
                    return _GlobalFontFamily;
                }
            }
            set
            {
                lock (_LockObject_GFF)
                {
                    if (_GlobalFontFamily != value)
                    {
                        _GlobalFontFamily = value;
                        UpdateGlobalResources();
                        NotifyStaticPropertyChanged(() => GlobalFontFamily);
                    }
                }
            }
        }

        #endregion

        #region GlobalFontSize

        static object _LockObject_GFS = new object();
        static double _GlobalFontSize = 12;

        /// <summary>
        /// Gets or sets the global font size.
        /// </summary>
        public static double GlobalFontSize
        {
            get
            {
                lock (_LockObject_GFS)
                {
                    return _GlobalFontSize;
                }
            }
            set
            {
                lock (_LockObject_GFS)
                {
                    if (_GlobalFontSize != value)
                    {
                        _GlobalFontSize = value;
                        UpdateGlobalResources();
                        NotifyStaticPropertyChanged(() => GlobalFontSize);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Private

        static void UpdateGlobalResources()
        {
            var styles = Application.Current.Styles;

            //Remove old styles
            foreach (var style in _Styles)
            {
                styles.Remove(style);
            }

            //Create new styles
            _Styles.AddRange
            ([
                new Style(s => s.OfType<TemplatedControl>())
                {
                    Setters =
                    {
                        new Setter(TemplatedControl.FontFamilyProperty, GlobalFontFamily),
                        new Setter(TemplatedControl.FontSizeProperty  , GlobalFontSize  ),
                    }
                },
                new Style(s => s.OfType<TextBlock>())
                {
                    Setters =
                    {
                        new Setter(TextBlock.FontFamilyProperty, GlobalFontFamily),
                        new Setter(TextBlock.FontSizeProperty  , GlobalFontSize  ),
                    }
                },
            ]);

            styles.AddRange(_Styles);
        }

        #endregion

        #region Static NotifyPropertyChanged

        /// <summary>
        /// Notifies that a static property has changed.
        /// </summary>
        /// <param name="propertyName">Name of static property, which has changed.</param>
        public static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies that all static properies have changed.
        /// </summary>
        public void NotifyAllStaticPropertyChanged()
        {
            NotifyStaticPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Notifies that a static property has changed.
        /// </summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="property">Expression of property, which has changed.</param>
        /// <exception cref="ArgumentException">Throws if given expression is used improperly.</exception>
        public static void NotifyStaticPropertyChanged<T>(Expression<Func<T>> property)
        {
            var expr = property.Body as MemberExpression;
            if (expr == null)
                throw new ArgumentException("Lambda does not contain member expression. Usage: () => MyClassOrObject.Property");
            NotifyStaticPropertyChanged(expr.Member.Name);
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        #endregion
    }
}
