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
    public partial class TrakHoundObjectSetRouter : TrakHoundEntityRouter<ITrakHoundObjectSetEntity>
    {
        public TrakHoundObjectSetRouter(TrakHoundRouter router) : base(router) { }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectSetEntity>> QueryByObject(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, requestId: requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectSetEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectSetQueryDriver, ITrakHoundObjectSetEntity>, Task<TrakHoundResponse<ITrakHoundObjectSetEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByObjectUuid(serviceRequest.Request.Queries?.Select(o => o.Query), skip, take, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectSetEntity>, Task<TrakHoundResponse<ITrakHoundObjectSetEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Sets.QueryByObjectUuid(routerRequest.Request.Queries?.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectSetEntity>("Query by ObjectUuid", requestId, objectUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectSetQueryDriver>(TrakHoundSourceRoutes.MetadataQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectSetEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectSetEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectSetEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectSetSubscribeDriver, ITrakHoundObjectSetEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectSetEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Sets.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectSetEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectSetSubscribeDriver>(TrakHoundObjectRoutes.SetsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectSetEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectSetDeleteDriver, ITrakHoundObjectSetEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectSetEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Sets.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectSetEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectSetDeleteDriver>(TrakHoundObjectRoutes.SetsDelete),
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
