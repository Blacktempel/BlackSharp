/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BlackSharp.UI.Avalonia.Windows.Media.Enums;

namespace BlackSharp.UI.Avalonia.Windows.Media
{
    /// <summary>
    /// Provides access to geometry resources mapped to specific icon types.
    /// </summary>
    public class GeometryManager
    {
        #region Constructor

        static GeometryManager()
        {
            _Geometries = AvaloniaXamlLoader.Load(_GeometryLocation) as ResourceDictionary;
        }

        #endregion

        #region Fields

        static readonly ResourceDictionary _Geometries;
        static readonly Uri _GeometryLocation = new("avares://BlackSharp.UI.Avalonia/Themes/Geometries.axaml", UriKind.Absolute);

        static readonly Dictionary<GeometryIcon, string> _Mapping = new()
        {
            { GeometryIcon.Error      , "ErrorIcon"       },
            { GeometryIcon.Exclamation, "ExclamationIcon" },
            { GeometryIcon.Information, "InformationIcon" },
            { GeometryIcon.Question   , "QuestionIcon"    },
        };

        #endregion

        #region Public

        /// <summary>
        /// Retrieves the geometry associated with the specified icon.
        /// </summary>
        /// <param name="icon">The icon for which to obtain the corresponding geometry.</param>
        /// <returns>The geometry object mapped to the specified icon or null if no mapping exists.</returns>
        public static Geometry GetGeometry(GeometryIcon icon)
        {
            if (_Mapping.TryGetValue(icon, out var key))
            {
                if (_Geometries[key] is Geometry geometry)
                {
                    return geometry;
                }
            }
            return null;
        }

        #endregion
    }
}
