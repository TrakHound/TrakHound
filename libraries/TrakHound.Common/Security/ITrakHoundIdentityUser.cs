// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Security
{
    public interface ITrakHoundIdentityUser
    {
        string Id { get; }

        TrakHoundIdentityUserType Type { get; }

        string Source { get; }

        IReadOnlyDictionary<string, string> Properties { get; }
    }
}
