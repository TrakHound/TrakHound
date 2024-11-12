// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Security
{
    public class TrakHoundIdentityParameters : ITrakHoundIdentityParameters
    {
        public ITrakHoundIdentityParameterCollection Request { get; set; }
        public ITrakHoundIdentityParameterCollection Session { get; set; }
        public ITrakHoundIdentityParameterCollection Persistent { get; set; }
    }
}
