// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Security
{
    public sealed class TrakHoundIdentityResource
    {
        public string Id { get; }

        public TrakHoundIdentityResourceType Type { get; }

        public IEnumerable<string> Permissions { get; }


        public TrakHoundIdentityResource(TrakHoundIdentityResourceType type, string id, IEnumerable<string> permissions = null)
        {
            Id = id;
            Type = type;
            Permissions = permissions;
        }
    }
}
