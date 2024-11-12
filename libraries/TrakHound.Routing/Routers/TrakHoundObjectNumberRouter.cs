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
    public class TrakHoundObjectNumberRouter : TrakHoundEntityRouter<ITrakHoundObjectNumberEntity>
    {
        public TrakHoundObjectNumberRouter(TrakHoundRouter router) : base(router) { }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> QueryByObject(IEnumerable<string> paths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectNumberQueryDriver, ITrakHoundObjectNumberEntity>, Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectNumberEntity>, Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Numbers.QueryByObjectUuid(routerRequest.Request.Queries.Select(o => o.Query), routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectNumberEntity>("Query", requestId, objectUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectNumberQueryDriver>(TrakHoundObjectRoutes.NumbersQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> QueryByObject(IEnumerable<string> paths, double min, double max, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, min, max, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, double min, double max, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectNumberQueryDriver, ITrakHoundObjectNumberEntity>, Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query), min, max);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectNumberEntity>, Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Numbers.QueryByObjectUuid(routerRequest.Request.Queries.Select(o => o.Query), min, max, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectNumberEntity>("Query", requestId, objectUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectNumberQueryDriver>(TrakHoundObjectRoutes.NumbersQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectNumberEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectNumberEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectNumberSubscribeDriver, ITrakHoundObjectNumberEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectNumberEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Numbers.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectNumberEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectNumberSubscribeDriver>(TrakHoundObjectRoutes.NumbersSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectNumberEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

    }
}
