// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Entities;

namespace TrakHound.Routing
{
    class EntityRouteRequest<TEntity> : RouteRequest where TEntity : ITrakHoundEntity
    {
        public EntityRouteRequest()
        {
            Id = null;
            Name = null;
        }

        public EntityRouteRequest(string name, string id, string query = null)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            AddQuery(query);
        }

        public EntityRouteRequest(string name, string id, IEnumerable<string> queries)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            AddQueries(queries);
        }
    }
}
