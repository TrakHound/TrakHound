// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Security
{
    public sealed class TrakHoundAuthenticatedSession : ITrakHoundAuthenticatedSession
    {
        public string SessionId { get; set; }

        public string ProviderId { get; set; }

        public ITrakHoundIdentityUser User { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public ITrakHoundIdentityParameters Parameters { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }
    }
}
