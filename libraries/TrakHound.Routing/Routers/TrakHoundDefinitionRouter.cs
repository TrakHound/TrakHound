// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundDefinitionRouter : TrakHoundEntityRouter<ITrakHoundDefinitionEntity>
    {
        private const int _storeQueueInterval = 1000;
        private const int _storeQueueTake = 10000;


        private readonly ItemIntervalQueue<ITrakHoundDefinitionQueryResult> _queryResultsQueue;


        public TrakHoundDefinitionRouter(TrakHoundRouter router) : base(router) 
        {
            _queryResultsQueue = new ItemIntervalQueue<ITrakHoundDefinitionQueryResult>(_storeQueueInterval, _storeQueueTake);
            _queryResultsQueue.ItemsReceived += QueryResultsQueueItemsReceived;
        }


        internal async Task ProcessQueryOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundDefinitionQueryResult>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var queryResults = new List<ITrakHoundDefinitionQueryResult>();

                    if (optionType == RouteRedirectOptions.Publish)
                    {
                        var arguments = options.Select(o => o.Argument);
                        if (!arguments.IsNullOrEmpty())
                        {
                            foreach (var argument in arguments)
                            {
                                if (argument != null) queryResults.Add(argument);
                            }
                        }
                    }

                    if (optionType == RouteRedirectOptions.Empty)
                    {
                        var queries = options.Select(o => o.Request);
                        if (!queries.IsNullOrEmpty())
                        {
                            foreach (var query in queries)
                            {
                                // Add Empty Query Result
                                var queryResult = new TrakHoundDefinitionQueryResult(query, null);
                                queryResults.Add(queryResult);
                            }
                        }
                    }

                    if (!queryResults.IsNullOrEmpty())
                    {
                        _queryResultsQueue.Add(queryResults);
                        //await StoreResults(queryResults);
                    }
                }
            }
        }

        private async void QueryResultsQueueItemsReceived(object sender, IEnumerable<ITrakHoundDefinitionQueryResult> queryResults)
        {
            if (!queryResults.IsNullOrEmpty())
            {
                await StoreResults(queryResults);
            }
        }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundDefinitionEntity>> Query(string pattern, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryIds(pattern, skip, take, sortOrder, requestId);
            var ids = idResponse.Content.Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, entityResponse.Duration);
        }

        public async Task<TrakHoundResponse<string>> QueryIds(string pattern, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();

            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IDefinitionQueryDriver, ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<string>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(pattern, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<string>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Definitions.Definitions.QueryIds(pattern, skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundDefinitionEntity>("Query", requestId, pattern);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IDefinitionQueryDriver>(TrakHoundDefinitionRoutes.DefinitionsQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query by Type"

        public async Task<TrakHoundResponse<ITrakHoundDefinitionEntity>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryIdsByType(types, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid);

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, entityResponse.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryIdsByType(IEnumerable<string> types, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();

            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IDefinitionQueryDriver, ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByType(serviceRequest.Request.Queries.Select(o => o.Query), skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Definitions.Definitions.QueryIdsByType(routerRequest.Request.Queries.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundDefinitionEntity>("Query by Type", requestId, types);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IDefinitionQueryDriver>(TrakHoundDefinitionRoutes.DefinitionsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessQueryOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<bool>> StoreResults(IEnumerable<ITrakHoundDefinitionQueryResult> queryResults, string requestId = null)
        {
            // Set Driver Function
            Func<ParameterRouteTargetDriverRequest<IDefinitionQueryStoreDriver, ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreResults(queryResults);
            };

            // Set Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Definitions.Definitions.StoreResults(queryResults, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundDefinitionEntity>("StoreResults", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IDefinitionQueryStoreDriver>(TrakHoundDefinitionRoutes.DefinitionsQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query by Parent"

        public async Task<TrakHoundResponse<ITrakHoundDefinitionEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryUuidsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IDefinitionQueryDriver, ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByParentUuid(parentUuids, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Definitions.Definitions.QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundDefinitionEntity>("Query by Parent Uuid", requestId, parentUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IDefinitionQueryDriver>(TrakHoundDefinitionRoutes.DefinitionsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            //await ProcessQueryByParentUuidOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query by Child"

        public async Task<TrakHoundResponse<ITrakHoundDefinitionEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryUuidsByChildUuid(childUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IDefinitionQueryDriver, ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByChildUuid(childUuids, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundDefinitionEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Definitions.Definitions.QueryUuidsByChildUuid(childUuids, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundDefinitionEntity>("QueryByChildUuid", requestId, childUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IDefinitionQueryDriver>(TrakHoundDefinitionRoutes.DefinitionsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            //await ProcessQueryByChildUuidOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

    }
}
