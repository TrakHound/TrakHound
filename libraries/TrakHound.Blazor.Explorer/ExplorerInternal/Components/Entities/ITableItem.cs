// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Blazor.ExplorerInternal.Components.Entities
{
    public interface ITableItem<TEntity> where TEntity : ITrakHoundEntity
    {
        string Uuid { get; set; }

        TEntity ToEntity();
    }
}
