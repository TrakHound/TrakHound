// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.ExplorerInternal.Components.Entities.Objects
{
    public class QueryResponseInformation
    {
        public bool Success { get; set; }

        public int Count { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
