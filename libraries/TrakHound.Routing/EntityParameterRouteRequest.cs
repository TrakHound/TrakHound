// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Entities;

namespace TrakHound.Routing
{
    class EntityParameterRouteRequest<TEntity> : ParameterRouteRequest where TEntity : ITrakHoundEntity
    {
        public EntityParameterRouteRequest()
        {
            Id = null;
            Name = null;
            Parameters = null;
        }

        public EntityParameterRouteRequest(string name, string id, string query = null, IEnumerable<RouteQueryParameter> parameters = null)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            Parameters = parameters;

            AddQuery(query);
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<string> queries, IEnumerable<RouteQueryParameter> parameters = null)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            Parameters = parameters;

            AddQueries(queries);
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<RouteQuery> queries, IEnumerable<RouteQueryParameter> parameters = null)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            Parameters = parameters;

            if (queries != null)
            {
                AddQueries(queries);
            }
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundTimeQuery> timeQueries)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
            Parameters = null;

            if (!timeQueries.IsNullOrEmpty())
            {
                foreach (var timeQuery in timeQueries)
                {
                    var routeQuery = new RouteQuery();
                    routeQuery.Query = timeQuery.Target;

                    var queryParameters = new List<RouteQueryParameter>();
                    queryParameters.Add(new RouteQueryParameter("timestamp", timeQuery.Timestamp));
                    routeQuery.Parameters = queryParameters;

                    AddQuery(routeQuery);
                }
            }
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundRangeQuery> rangeQueries)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
            Parameters = null;

            if (!rangeQueries.IsNullOrEmpty())
            {
                foreach (var rangeQuery in rangeQueries)
                {
                    var routeQuery = new RouteQuery();
                    routeQuery.Query = rangeQuery.Target;

                    var queryParameters = new List<RouteQueryParameter>();
                    queryParameters.Add(new RouteQueryParameter("start", rangeQuery.From));
                    queryParameters.Add(new RouteQueryParameter("stop", rangeQuery.To));
                    routeQuery.Parameters = queryParameters;

                    AddQuery(routeQuery);
                }
            }
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundRangeQuery> rangeQueries, long skip, long take)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            var requestParameters = new List<RouteQueryParameter>();
            requestParameters.Add(new RouteQueryParameter("skip", skip));
            requestParameters.Add(new RouteQueryParameter("take", take));
            Parameters = requestParameters;

            if (!rangeQueries.IsNullOrEmpty())
            {
                foreach (var rangeQuery in rangeQueries)
                {
                    var routeQuery = new RouteQuery();
                    routeQuery.Query = rangeQuery.Target;

                    var queryParameters = new List<RouteQueryParameter>();
                    queryParameters.Add(new RouteQueryParameter("start", rangeQuery.From));
                    queryParameters.Add(new RouteQueryParameter("stop", rangeQuery.To));
                    routeQuery.Parameters = queryParameters;

                    AddQuery(routeQuery);
                }
            }
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundTimeRangeQuery> rangeQueries)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
            Parameters = null;

            if (!rangeQueries.IsNullOrEmpty())
            {
                foreach (var rangeQuery in rangeQueries)
                {
                    var routeQuery = new RouteQuery();
                    routeQuery.Query = rangeQuery.Target;

                    var queryParameters = new List<RouteQueryParameter>();
                    queryParameters.Add(new RouteQueryParameter("start", rangeQuery.From));
                    queryParameters.Add(new RouteQueryParameter("stop", rangeQuery.To));
                    queryParameters.Add(new RouteQueryParameter("span", rangeQuery.Span));
                    routeQuery.Parameters = queryParameters;

                    AddQuery(routeQuery);
                }
            }
        }

        public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundTimeRangeQuery> rangeQueries, long skip, long take)
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            Name = $"[{entityCategory}] {name} {entityClass}";
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            var requestParameters = new List<RouteQueryParameter>();
            requestParameters.Add(new RouteQueryParameter("skip", skip));
            requestParameters.Add(new RouteQueryParameter("take", take));
            Parameters = requestParameters;

            if (!rangeQueries.IsNullOrEmpty())
            {
                foreach (var rangeQuery in rangeQueries)
                {
                    var routeQuery = new RouteQuery();
                    routeQuery.Query = rangeQuery.Target;

                    var queryParameters = new List<RouteQueryParameter>();
                    queryParameters.Add(new RouteQueryParameter("start", rangeQuery.From));
                    queryParameters.Add(new RouteQueryParameter("stop", rangeQuery.To));
                    queryParameters.Add(new RouteQueryParameter("span", rangeQuery.Span));
                    routeQuery.Parameters = queryParameters;

                    AddQuery(routeQuery);
                }
            }
        }

        //public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundTimeRangeIdQuery> rangeQueries)
        //{
        //    var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
        //    var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

        //    Name = $"[{entityCategory}] {name} {entityClass}";
        //    Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
        //    Parameters = null;

        //    if (!rangeQueries.IsNullOrEmpty())
        //    {
        //        foreach (var rangeQuery in rangeQueries)
        //        {
        //            var routeQuery = new RouteQuery();
        //            routeQuery.Query = rangeQuery.Target;

        //            var queryParameters = new List<RouteQueryParameter>();
        //            if (!rangeQuery.TimeRangeIds.IsNullOrEmpty())
        //            {
        //                foreach (var timeRangeId in rangeQuery.TimeRangeIds)
        //                {
        //                    queryParameters.Add(new RouteQueryParameter("timeRangeId", timeRangeId));
        //                }
        //            }

        //            routeQuery.Parameters = queryParameters;

        //            AddQuery(routeQuery);
        //        }
        //    }
        //}

        //public EntityParameterRouteRequest(string name, string id, IEnumerable<TrakHoundTimeRangeIdQuery> rangeQueries, long skip, long take)
        //{
        //    var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
        //    var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

        //    Name = $"[{entityCategory}] {name} {entityClass}";
        //    Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

        //    var requestParameters = new List<RouteQueryParameter>();
        //    requestParameters.Add(new RouteQueryParameter("skip", skip));
        //    requestParameters.Add(new RouteQueryParameter("take", take));
        //    Parameters = requestParameters;

        //    if (!rangeQueries.IsNullOrEmpty())
        //    {
        //        foreach (var rangeQuery in rangeQueries)
        //        {
        //            var routeQuery = new RouteQuery();
        //            routeQuery.Query = rangeQuery.Target;

        //            var queryParameters = new List<RouteQueryParameter>();
        //            if (!rangeQuery.TimeRangeIds.IsNullOrEmpty())
        //            {
        //                foreach (var timeRangeId in rangeQuery.TimeRangeIds)
        //                {
        //                    queryParameters.Add(new RouteQueryParameter("timeRangeId", timeRangeId));
        //                }
        //            }

        //            routeQuery.Parameters = queryParameters;

        //            AddQuery(routeQuery);
        //        }
        //    }
        //}
    }
}
