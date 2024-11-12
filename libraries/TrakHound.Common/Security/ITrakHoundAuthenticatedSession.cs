// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Security
{
    public interface ITrakHoundAuthenticatedSession : ITrakHoundSession
    {
        ITrakHoundIdentityUser User { get; }

        DateTime ValidFrom { get; }

        DateTime ValidTo { get; }
    }
}
