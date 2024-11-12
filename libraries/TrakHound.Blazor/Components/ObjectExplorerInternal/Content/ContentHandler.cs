// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public delegate void ContentHandler(string objectUuid, ITrakHoundEntity entity, string value, long timestamp = 0);
}
