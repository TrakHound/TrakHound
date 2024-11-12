// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundObjectStateRouter : TrakHoundEntityRouter<ITrakHoundObjectStateEntity>
    {
        public TrakHoundObjectStateRouter(TrakHoundRouter router) : base(router) { }


        protected override long FilterQueryRange(TrakHoundResult<ITrakHoundObjectStateEntity> result)
        {
            return result.Content.Timestamp;
        }


        #region "Latest"

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> LatestByObject(IEnumerable<string> paths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await LatestByObjectUuid(objectUuids, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> LatestByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStateLatestDriver, ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundObjectStateEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Latest(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundObjectStateEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.States.LatestByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStateEntity>("Latest", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStateLatestDriver>(TrakHoundObjectRoutes.StatesLatest),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> QueryByObject(IEnumerable<string> paths, long start, long stop, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, start, stop, skip, take, sortOrder, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await QueryByRange(queries, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStateQueryDriver, ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundObjectStateEntity>>> serviceFunction = async (serviceRequest) =>
            {
                var querySkip = serviceRequest.Request.GetParameter<long>("skip");
                var queryTake = serviceRequest.Request.GetParameter<long>("take");

                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.Query(rangeQueries, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundObjectStateEntity>>> routerFunction = async (routerRequest) =>
            {
                var querySkip = routerRequest.Request.GetParameter<long>("skip");
                var queryTake = routerRequest.Request.GetParameter<long>("take");

                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.States.QueryByRange(rangeQueries, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStateEntity>("Query", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStateQueryDriver>(TrakHoundObjectRoutes.StatesQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Last"

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> LastByObject(IEnumerable<string> paths, long timestamp, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await LastByObjectUuid(objectUuids, timestamp, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> LastByObjectUuid(IEnumerable<string> objectUuids, long timestamp, string requestId = null)
        {
            var queries = new List<TrakHoundTimeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundTimeQuery(objectUuid, timestamp));
            }

            return await LastByRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> LastByRange(IEnumerable<TrakHoundTimeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStateQueryDriver, ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundObjectStateEntity>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryTimestamp = query.GetParameter<long>("timestamp");
                    rangeQueries.Add(new TrakHoundTimeQuery(query.Query, queryTimestamp));
                }

                return await serviceRequest.Driver.Last(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundObjectStateEntity>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryTimestamp = query.GetParameter<long>("timestamp");
                    rangeQueries.Add(new TrakHoundTimeQuery(query.Query, queryTimestamp));
                }

                return await routerRequest.Router.Entities.Objects.States.LastByRange(rangeQueries, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStateEntity>("Last", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStateQueryDriver>(TrakHoundObjectRoutes.StatesQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryTime
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Count"

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByObject(IEnumerable<string> paths, long start, long stop, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await CountByObjectUuid(objectUuids, start, stop, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await CountByRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByRange(IEnumerable<TrakHoundRangeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStateQueryDriver, ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.Count(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.States.CountByRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStateEntity>("Count", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStateQueryDriver>(TrakHoundObjectRoutes.StatesQuery),
                serviceFunction,
                routerFunction,
                ProcessCountRange
                );

            return new TrakHoundResponse<TrakHoundCount>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectStateEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectStateEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStateSubscribeDriver, ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStateEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.States.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectStateEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStateSubscribeDriver>(TrakHoundObjectRoutes.StatesSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectStateEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

    }
}
