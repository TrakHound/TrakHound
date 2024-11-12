// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Http
{
    public static partial class TrakHoundHttp
    {
        public static string GetEntityPath<TEntity>() where TEntity : ITrakHoundEntity
        {
            var path = HttpConstants.EntitiesPrefix;

            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) path = Url.Combine(path, GetSourceEntityPath<TEntity>());
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) path = Url.Combine(path, GetDefinitionEntityPath<TEntity>());
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) path = Url.Combine(path, GetObjectEntityPath<TEntity>());

            return path;
        }
    }
}
