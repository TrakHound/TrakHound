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
    public class TrakHoundObjectEventRouter : TrakHoundEntityRouter<ITrakHoundObjectEventEntity>
    {
        public TrakHoundObjectEventRouter(TrakHoundRouter router) : base(router) { }


        protected override long FilterQueryRange(TrakHoundResult<ITrakHoundObjectEventEntity> result)
        {
            return result.Content.Timestamp;
        }


        #region "Latest"

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> LatestByObject(IEnumerable<string> paths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await LatestByObjectUuid(objectUuids, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectEventEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> LatestByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectEventLatestDriver, ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<ITrakHoundObjectEventEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Latest(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<ITrakHoundObjectEventEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Events.LatestByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEventEntity>("Latest", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectEventLatestDriver>(TrakHoundObjectRoutes.EventsLatest),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return new TrakHoundResponse<ITrakHoundObjectEventEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> QueryByObject(IEnumerable<string> paths, long start, long stop, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, start, stop, skip, take, sortOrder, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectEventEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await QueryByRange(queries, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectEventQueryDriver, ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<ITrakHoundObjectEventEntity>>> serviceFunction = async (serviceRequest) =>
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
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<ITrakHoundObjectEventEntity>>> routerFunction = async (routerRequest) =>
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

                return await routerRequest.Router.Entities.Objects.Events.QueryByRange(rangeQueries, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEventEntity>("Query", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectEventQueryDriver>(TrakHoundObjectRoutes.EventsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return new TrakHoundResponse<ITrakHoundObjectEventEntity>(response.Results, response.Duration);
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
            Func<ParameterRouteTargetDriverRequest<IObjectEventQueryDriver, ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
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
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Events.CountByRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEventEntity>("Count", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectEventQueryDriver>(TrakHoundObjectRoutes.EventsQuery),
                serviceFunction,
                routerFunction,
                ProcessCountRange
                );

            return new TrakHoundResponse<TrakHoundCount>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectEventEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectEventEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectEventSubscribeDriver, ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Events.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectEventEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectEventSubscribeDriver>(TrakHoundObjectRoutes.EventsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectEventEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectEventDeleteDriver, ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEventEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Events.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEventEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectEventDeleteDriver>(TrakHoundObjectRoutes.EventsDelete),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }

        #endregion

    }
}
