// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public static partial class TrakHoundHttp
    {
        public static Dictionary<string, string> GetApiParameters(HttpResponse httpResponse)
        {
            Dictionary<string, string> parameters = null;

            if (httpResponse.Headers != null)
            {
                foreach (var header in httpResponse.Headers)
                {
                    if (header.Key != null && header.Key.StartsWith(HttpConstants.ApiParameterHeaderPrefix) && header.Key.Length > HttpConstants.ApiParameterHeaderPrefix.Length + 1)
                    {
                        var parameterName = header.Key.Substring(HttpConstants.ApiParameterHeaderPrefix.Length + 1);
                        parameterName = parameterName.ToPascalCase();
                        if (!string.IsNullOrEmpty(parameterName))
                        {
                            if (parameters == null) parameters = new Dictionary<string, string>();
                            parameters.Remove(parameterName);
                            parameters.Add(parameterName, header.Value?.FirstOrDefault());
                        }
                    }
                }
            }

            return parameters;
        }

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
