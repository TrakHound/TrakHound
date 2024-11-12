// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Security
{
    public sealed class TrakHoundAnnonymousSession : ITrakHoundAnnonymousSession
    {
        public string SessionId { get; set; }

        public string ProviderId { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public ITrakHoundIdentityParameters Parameters { get; set; }
    }
}
