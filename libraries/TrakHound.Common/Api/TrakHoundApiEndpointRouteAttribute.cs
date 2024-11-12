// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public abstract class TrakHoundApiEndpointRouteAttribute : Attribute
    {
        public TrakHoundApiRouteType Type { get; init; }

        public string Route { get; set; }

        public string Description { get; set; }
    }
}
