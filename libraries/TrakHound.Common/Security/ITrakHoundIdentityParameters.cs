// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Security
{
    public interface ITrakHoundIdentityParameters
    {
        ITrakHoundIdentityParameterCollection Request { get; }
        ITrakHoundIdentityParameterCollection Session { get; }
        ITrakHoundIdentityParameterCollection Persistent { get; }
    }
}
