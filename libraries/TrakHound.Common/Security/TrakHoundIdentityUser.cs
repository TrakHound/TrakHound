// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Security
{
    public class TrakHoundIdentityUser : ITrakHoundIdentityUser
    {
        public string Id { get; set; }

        public TrakHoundIdentityUserType Type { get; set; }

        public string Source { get; set; }

        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}
