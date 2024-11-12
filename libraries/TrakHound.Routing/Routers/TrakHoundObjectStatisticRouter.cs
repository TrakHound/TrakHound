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
    public class TrakHoundObjectStatisticRouter : TrakHoundEntityRouter<ITrakHoundObjectStatisticEntity>
    {
        public TrakHoundObjectStatisticRouter(TrakHoundRouter router) : base(router) { }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>> QueryByObject(IEnumerable<string> paths, long start, long stop, long span, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, start, stop, span, skip, take, sortOrder, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectStatisticEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, long span, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var queries = new List<TrakHoundTimeRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundTimeRangeQuery(objectUuid, start, stop, span));
            }

            return await QueryByRange(queries, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>> QueryByRange(IEnumerable<TrakHoundTimeRangeQuery> queries, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStatisticQueryDriver, ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>>> serviceFunction = async (serviceRequest) =>
            {
                var querySkip = serviceRequest.Request.GetParameter<long>("skip");
                var queryTake = serviceRequest.Request.GetParameter<long>("take");

                var rangeQueries = new List<TrakHoundTimeRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    var querySpan = query.GetParameter<long>("span");
                    rangeQueries.Add(new TrakHoundTimeRangeQuery(query.Query, queryStart, queryStop, querySpan));
                }

                return await serviceRequest.Driver.Query(rangeQueries, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>>> routerFunction = async (routerRequest) =>
            {
                var querySkip = routerRequest.Request.GetParameter<long>("skip");
                var queryTake = routerRequest.Request.GetParameter<long>("take");

                var rangeQueries = new List<TrakHoundTimeRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    var querySpan = query.GetParameter<long>("span");
                    rangeQueries.Add(new TrakHoundTimeRangeQuery(query.Query, queryStart, queryStop, querySpan));
                }

                return await routerRequest.Router.Entities.Objects.Statistics.QueryByRange(rangeQueries, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStatisticEntity>("Query", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStatisticQueryDriver>(TrakHoundObjectRoutes.StatisticsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryTimeRange
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return new TrakHoundResponse<ITrakHoundObjectStatisticEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Spans"

        public async Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> SpansByObject(IEnumerable<string> paths, long start, long stop, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await SpansByObjectUuid(objectUuids, start, stop, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundTimeRangeSpan>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> SpansByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await SpansByRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> SpansByRange(IEnumerable<TrakHoundRangeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStatisticQueryDriver, ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<TrakHoundTimeRangeSpan>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.Spans(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<TrakHoundTimeRangeSpan>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Statistics.SpansByRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStatisticEntity>("Spans", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStatisticQueryDriver>(TrakHoundObjectRoutes.StatisticsQuery),
                serviceFunction,
                routerFunction,
                ProcessSpanRange
                );

            return new TrakHoundResponse<TrakHoundTimeRangeSpan>(response.Results, response.Duration);
        }

        #endregion

        #region "Count"

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByObject(IEnumerable<string> paths, long start, long stop, long span, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await CountByObjectUuid(objectUuids, start, stop, span, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, long span, string requestId = null)
        {
            var queries = new List<TrakHoundTimeRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundTimeRangeQuery(objectUuid, start, stop, span));
            }

            return await CountByRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByRange(IEnumerable<TrakHoundTimeRangeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStatisticQueryDriver, ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    var querySpan = query.GetParameter<long>("span");
                    rangeQueries.Add(new TrakHoundTimeRangeQuery(query.Query, queryStart, queryStop, querySpan));
                }

                return await serviceRequest.Driver.Count(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    var querySpan = query.GetParameter<long>("span");
                    rangeQueries.Add(new TrakHoundTimeRangeQuery(query.Query, queryStart, queryStop, querySpan));
                }

                return await routerRequest.Router.Entities.Objects.Statistics.CountByRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStatisticEntity>("Count", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStatisticQueryDriver>(TrakHoundObjectRoutes.StatisticsQuery),
                serviceFunction,
                routerFunction,
                ProcessCountRange
                );

            return new TrakHoundResponse<TrakHoundCount>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectStatisticEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectStatisticEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStatisticSubscribeDriver, ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Statistics.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectStatisticEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStatisticSubscribeDriver>(TrakHoundObjectRoutes.StatisticsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectStatisticEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectStatisticDeleteDriver, ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectStatisticEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Statistics.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectStatisticEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectStatisticDeleteDriver>(TrakHoundObjectRoutes.StatisticsDelete),
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
