// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.Filters
{
    public class TrakHoundEntityMatchResult
    {
        public string Query { get; }

        public ITrakHoundEntity Entity { get; }


        public TrakHoundEntityMatchResult(string query, ITrakHoundEntity entity)
        {
            Query = query;
            Entity = entity;
        }
    }
}
