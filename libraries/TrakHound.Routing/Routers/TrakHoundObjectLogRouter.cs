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
    public class TrakHoundObjectLogRouter : TrakHoundEntityRouter<ITrakHoundObjectLogEntity>
    {
        public TrakHoundObjectLogRouter(TrakHoundRouter router) : base(router) { }


        protected override long FilterQueryRange(TrakHoundResult<ITrakHoundObjectLogEntity> result)
        {
            return result.Content.Timestamp;
        }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> QueryByObject(IEnumerable<string> paths, TrakHoundLogLevel minimumLogLevel, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, minimumLogLevel, skip, take, sortOrder, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectLogEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLogLevel, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectLogQueryDriver, ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundObjectLogEntity>>> serviceFunction = async (serviceRequest) =>
            {
                var querySkip = serviceRequest.Request.GetParameter<long>("skip");
                var queryTake = serviceRequest.Request.GetParameter<long>("take");

                return await serviceRequest.Driver.Query(objectUuids, minimumLogLevel, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundObjectLogEntity>>> routerFunction = async (routerRequest) =>
            {
                var querySkip = routerRequest.Request.GetParameter<long>("skip");
                var queryTake = routerRequest.Request.GetParameter<long>("take");

                return await routerRequest.Router.Entities.Objects.Logs.QueryByObjectUuid(objectUuids, minimumLogLevel, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectLogEntity>("Query", requestId, objectUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectLogQueryDriver>(TrakHoundObjectRoutes.LogsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return new TrakHoundResponse<ITrakHoundObjectLogEntity>(response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> QueryByObject(IEnumerable<string> paths, TrakHoundLogLevel minimumLogLevel, long start, long stop, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, minimumLogLevel, start, stop, skip, take, sortOrder, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectLogEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLogLevel, long start, long stop, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await QueryByRange(queries, minimumLogLevel, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundLogLevel minimumLogLevel, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectLogQueryDriver, ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundObjectLogEntity>>> serviceFunction = async (serviceRequest) =>
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

                return await serviceRequest.Driver.Query(rangeQueries, minimumLogLevel, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundObjectLogEntity>>> routerFunction = async (routerRequest) =>
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

                return await routerRequest.Router.Entities.Objects.Logs.QueryByRange(rangeQueries, minimumLogLevel, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectLogEntity>("Query", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectLogQueryDriver>(TrakHoundObjectRoutes.LogsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return new TrakHoundResponse<ITrakHoundObjectLogEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> SubscribeByObject(IEnumerable<string> paths, TrakHoundLogLevel minimumLogLevel, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>();

            var response = await Subscribe(minimumLogLevel, requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectLogEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectLogEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLogLevel, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectLogSubscribeDriver, ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids, minimumLogLevel);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Logs.SubscribeByObjectUuid(objectUuids, minimumLogLevel, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectLogEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectLogSubscribeDriver>(TrakHoundObjectRoutes.LogsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectLogEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(request.Id, response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(TrakHoundLogLevel minimumLogLevel, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectLogSubscribeDriver, ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(minimumLogLevel);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Logs.Subscribe(minimumLogLevel, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectLogEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectLogSubscribeDriver>(TrakHoundObjectRoutes.LogsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectLogEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectLogDeleteDriver, ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectLogEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Logs.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectLogEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectLogDeleteDriver>(TrakHoundObjectRoutes.LogsDelete),
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
