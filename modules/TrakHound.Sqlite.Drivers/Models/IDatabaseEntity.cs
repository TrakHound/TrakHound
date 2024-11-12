// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public interface IDatabaseEntity<TEntity> where TEntity : ITrakHoundEntity
    {
        string RequestedId { get; set; }

        public TEntity ToEntity();
    }
}
