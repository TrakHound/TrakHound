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
    public partial class TrakHoundObjectVocabularySetRouter : TrakHoundEntityRouter<ITrakHoundObjectVocabularySetEntity>
    {
        public TrakHoundObjectVocabularySetRouter(TrakHoundRouter router) : base(router) { }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>> QueryByObject(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectVocabularySetQueryDriver, ITrakHoundObjectVocabularySetEntity>, Task<TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByObjectUuid(serviceRequest.Request.Queries?.Select(o => o.Query), skip, take, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectVocabularySetEntity>, Task<TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.VocabularySets.QueryByObjectUuid(routerRequest.Request.Queries?.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectVocabularySetEntity>("Query by ObjectUuid", requestId, objectUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectVocabularySetQueryDriver>(TrakHoundObjectRoutes.VocabularySetsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectVocabularySetEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectVocabularySetEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectVocabularySetSubscribeDriver, ITrakHoundObjectVocabularySetEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectVocabularySetEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.VocabularySets.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectVocabularySetEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectVocabularySetSubscribeDriver>(TrakHoundObjectRoutes.VocabularySetsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectVocabularySetEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularySetEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectVocabularySetDeleteDriver, ITrakHoundObjectVocabularySetEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectVocabularySetEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.VocabularySets.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectVocabularySetEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectVocabularySetDeleteDriver>(TrakHoundObjectRoutes.VocabularySetsDelete),
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
