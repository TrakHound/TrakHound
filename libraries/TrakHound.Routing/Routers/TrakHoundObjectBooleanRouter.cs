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
    public class TrakHoundObjectBooleanRouter : TrakHoundEntityRouter<ITrakHoundObjectBooleanEntity>
    {
        public TrakHoundObjectBooleanRouter(TrakHoundRouter router) : base(router) { }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectBooleanEntity>> QueryByObject(IEnumerable<string> paths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectBooleanEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectBooleanQueryDriver, ITrakHoundObjectBooleanEntity>, Task<TrakHoundResponse<ITrakHoundObjectBooleanEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectBooleanEntity>, Task<TrakHoundResponse<ITrakHoundObjectBooleanEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Booleans.QueryByObjectUuid(routerRequest.Request.Queries.Select(o => o.Query), routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectBooleanEntity>("Query", requestId, objectUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectBooleanQueryDriver>(TrakHoundObjectRoutes.BooleansQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectBooleanEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectBooleanEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectBooleanSubscribeDriver, ITrakHoundObjectBooleanEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectBooleanEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Booleans.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectBooleanEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectBooleanSubscribeDriver>(TrakHoundObjectRoutes.BooleansSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectBooleanEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

    }
}
