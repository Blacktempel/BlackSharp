/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

namespace BlackSharp.Core.Security.Cryptography
{
    public sealed class SHA3_224 : SHA3
    {
        #region Constructor

        public SHA3_224()
            : base(224)
        {
        }

        #endregion
    }

    public sealed class SHA3_256 : SHA3
    {
        #region Constructor

        public SHA3_256()
            : base(256)
        {
        }

        #endregion
    }

    public sealed class SHA3_384 : SHA3
    {
        #region Constructor

        public SHA3_384()
            : base(384)
        {
        }

        #endregion
    }

    public sealed class SHA3_512 : SHA3
    {
        #region Constructor

        public SHA3_512()
            : base(512)
        {
        }

        #endregion
    }
}
