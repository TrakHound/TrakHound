// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Security
{
    public interface ITrakHoundSession
    {
        public string SessionId { get; }

        public string ProviderId { get; }

        IEnumerable<string> Roles { get; }

        public ITrakHoundIdentityParameters Parameters { get; }
    }
}
