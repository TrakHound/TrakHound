// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundObjectRouter : TrakHoundEntityRouter<ITrakHoundObjectEntity>
    {
        private const int _storeQueueInterval = 1000;
        private const int _storeQueueTake = 10000;


        private readonly ITrakHoundClient _client;
        private readonly TrakHoundObjectNotifyHandler _notifyHandler;
        private readonly ItemIntervalQueue<ITrakHoundObjectQueryResult> _queryResultsQueue;


        public TrakHoundObjectRouter(TrakHoundRouter router) : base(router)
        {
            _client = router.Client;
            _notifyHandler = new TrakHoundObjectNotifyHandler(_client);

            _queryResultsQueue = new ItemIntervalQueue<ITrakHoundObjectQueryResult>(_storeQueueInterval, _storeQueueTake);
            _queryResultsQueue.ItemsReceived += QueryResultsQueueItemsReceived;
        }


        protected override Task OnAfterPublish(IEnumerable<TrakHoundPublishResult<ITrakHoundObjectEntity>> results, IEnumerable<ITrakHoundObjectEntity> entities, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            if (!results.IsNullOrEmpty())
            {
                UpdateNotifications(results);
            }

            return Task.CompletedTask;
        }

        protected async override Task OnBeforeDelete(IEnumerable<EntityDeleteRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
		{
			if (!requests.IsNullOrEmpty())
			{
                await UpdateNotifications(requests);
			}
		}


        internal async Task ProcessQueryOptions(string requestId, TrakHoundObjectQueryRequest queryRequest, IEnumerable<RouteRedirectOption<ITrakHoundObjectQueryResult>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var queryResults = new List<ITrakHoundObjectQueryResult>();

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
                                if (!queryRequest.ParentUuids.IsNullOrEmpty())
                                {
                                    foreach (var parentUuid in queryRequest.ParentUuids)
                                    {
                                        var isRoot = parentUuid == null;

                                        // Add Empty Query Result
                                        var queryResult = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, null, queryRequest.ParentLevel, null, parentUuid, isRoot);
                                        queryResults.Add(queryResult);
                                    }
                                }
                                else
                                {
                                    // Add Empty Query Result
                                    var queryResult = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, null);
                                    queryResults.Add(queryResult);
                                }
                            }
                        }
                    }

                    if (!queryResults.IsNullOrEmpty())
                    {
                        _queryResultsQueue.Add(queryResults);
                    }
                }
            }
        }

        private async void QueryResultsQueueItemsReceived(object sender, IEnumerable<ITrakHoundObjectQueryResult> queryResults)
        {
            if (!queryResults.IsNullOrEmpty())
            {
                await StoreResults(queryResults);
            }
        }

        internal async Task ProcessQueryByParentUuidOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectQueryResult>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var queryResults = new List<ITrakHoundObjectQueryResult>();

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
                        var parentUuids = options.Select(o => o.Request);
                        if (!parentUuids.IsNullOrEmpty())
                        {
                            foreach (var parentUuid in parentUuids)
                            {
                                // Add Empty Query Result
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, null);
                                queryResults.Add(queryResult);
                            }
                        }
                    }

                    if (!queryResults.IsNullOrEmpty())
                    {
                        await StoreParents(queryResults);
                    }
                }
            }
        }

        internal async Task ProcessQueryByChildUuidOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectQueryResult>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var queryResults = new List<ITrakHoundObjectQueryResult>();

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
                        var parentUuids = options.Select(o => o.Request);
                        if (!parentUuids.IsNullOrEmpty())
                        {
                            foreach (var parentUuid in parentUuids)
                            {
                                // Add Empty Query Result
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, null);
                                queryResults.Add(queryResult);
                            }
                        }
                    }

                    if (!queryResults.IsNullOrEmpty())
                    {
                        await StoreChildren(queryResults);
                    }
                }
            }
        }

        internal async Task ProcessQueryChildrenByRootUuidOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectQueryResult>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var queryResults = new List<ITrakHoundObjectQueryResult>();

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
                        var parentUuids = options.Select(o => o.Request);
                        if (!parentUuids.IsNullOrEmpty())
                        {
                            foreach (var parentUuid in parentUuids)
                            {
                                // Add Empty Query Result
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, null);
                                queryResults.Add(queryResult);
                            }
                        }
                    }

                    if (!queryResults.IsNullOrEmpty())
                    {
                        await StoreChildrenByRoots(queryResults);
                    }
                }
            }
        }

        internal async Task ProcessQueryRootsByChildUuidOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectQueryResult>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var queryResults = new List<ITrakHoundObjectQueryResult>();

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
                        var parentUuids = options.Select(o => o.Request);
                        if (!parentUuids.IsNullOrEmpty())
                        {
                            foreach (var parentUuid in parentUuids)
                            {
                                // Add Empty Query Result
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, null);
                                queryResults.Add(queryResult);
                            }
                        }
                    }

                    if (!queryResults.IsNullOrEmpty())
                    {
                        await StoreRootsByChildren(queryResults);
                    }
                }
            }
        }

        private void UpdateNotifications(IEnumerable<TrakHoundPublishResult<ITrakHoundObjectEntity>> results)
        {
            if (!results.IsNullOrEmpty())
            {
                foreach (var result in results)
                {
                    if (result.Type != TrakHoundPublishResultType.Queued)
                    {
                        var notificationType = result.Type == TrakHoundPublishResultType.Created ? TrakHoundEntityNotificationType.Created : TrakHoundEntityNotificationType.Changed;

                        _notifyHandler.AddPublishNotification(notificationType, result.Result);
                    }
                }
            }
        }

        private async Task UpdateNotifications(IEnumerable<EntityDeleteRequest> requests)
        {
            //var uuids = requests.Select(o => o.Target).Distinct();
            //var existingEntities = await _client.System.Entities.Objects.ReadPartialModels(uuids);
            //var dExistingEntities = existingEntities.ToDistinct()?.ToDictionary(o => o.Uuid);

            //foreach (var request in requests)
            //{
            //    if (!string.IsNullOrEmpty(request.Target))
            //    {
            //        ITrakHoundObjectEntityModel existingEntity = null;
            //        if (dExistingEntities != null && dExistingEntities.ContainsKey(request.Target))
            //        {
            //            existingEntity = dExistingEntities[request.Target];
            //        }

            //        if (existingEntity != null)
            //        {
            //            _notifyHandler.AddDeleteNotification(existingEntity);
            //        }
            //    }
            //}
        }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> Query(IEnumerable<string> paths, long skip = 0, long take = int.MaxValue, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var uuidResponse = await QueryUuids(paths, skip, take, sortOrder, requestId);
            var uuids = uuidResponse.Content?.Select(o => o.Uuid);

            var entityResponse = await Read(uuids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = int.MaxValue, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var uuidResponse = await QueryUuidsByQueryRequest(queryRequest, skip, take, sortOrder, requestId);
            var uuids = uuidResponse.Content?.Select(o => o.Uuid);

            var entityResponse = await Read(uuids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryUuids(IEnumerable<string> paths, long skip = 0, long take = int.MaxValue, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var expressionQueries = new List<string>();
            var absoluteQueries = new List<string>();

            foreach (var path in paths)
            {
                switch (TrakHoundPath.GetType(path))
                {
                    case TrakHoundPathType.Absolute: absoluteQueries.Add(path); break;
                    case TrakHoundPathType.Expression: expressionQueries.Add(path); break;
                }
            }

            // Expression Paths
            if (!expressionQueries.IsNullOrEmpty())
            {
                var expressionResponse = await QueryUuidsByExpression(expressionQueries, skip, take, sortOrder, requestId);
                results.AddRange(expressionResponse.Results);
            }

            //// Expression Paths
            //if (!expressionQueries.IsNullOrEmpty())
            //{
            //    var namespaces = expressionQueries.Select(o => TrakHoundPath.GetNamespace(o)).Distinct();
            //    foreach (var ns in namespaces)
            //    {
            //        var byNamespace = expressionQueries.Where(o => TrakHoundPath.GetNamespace(o) == ns);
            //        var partialPaths = byNamespace?.Select(o => TrakHoundPath.GetPartialPath(o));

            //        var expressionResponse = await QueryUuidsByExpression(ns, partialPaths, skip, take, sortOrder, requestId);
            //        results.AddRange(expressionResponse.Results);
            //    }
            //}

            // Absolute Paths
            if (!absoluteQueries.IsNullOrEmpty())
            {
                foreach (var pathQuery in absoluteQueries)
                {
                    var uuid = TrakHoundPath.GetUuid(pathQuery);
                    if (uuid != null)
                    {
                        var result = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, pathQuery, uuid);
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(_router.Id, pathQuery, TrakHoundResultType.Ok, result, result.Uuid));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryUuidsByQueryRequest(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = int.MaxValue, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(queryRequest, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryUuidsByQueryRequest(queryRequest, skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Query Uuids by Query Request", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessQueryOptions(request.Id, queryRequest, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryUuidsByExpression(IEnumerable<string> expressions, long skip = 0, long take = int.MaxValue, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var expression in expressions)
            {
                var expressionResults = await TrakHoundExpression.Evaluate(_client.System.Entities, expression, skip, take, sortOrder); // DEBUG!!! NEEDS TO ALLOW expression with Namespace included
                if (!expressionResults.IsNullOrEmpty())
                {
                    foreach (var expressionResult in expressionResults)
                    {
                        var result = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Expression, expressionResult.Namespace, expressionResult.Expression, expressionResult.Uuid);
                        result.Skip = skip;
                        result.Take = take;
                        result.SortOrder = sortOrder;
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(_router.Id, expression, TrakHoundResultType.Ok, result, result.Uuid));
                    }
                }
                else
                {
                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(_router.Id, expression, TrakHoundResultType.NotFound));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        //public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryUuidsByExpression(string ns, IEnumerable<string> expressions, long skip = 0, long take = int.MaxValue, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        //{
        //    var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();
        //    if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
        //    var stpw = System.Diagnostics.Stopwatch.StartNew();

        //    foreach (var expression in expressions)
        //    {
        //        //var expressionResults = await TrakHoundExpression.Evaluate(_client.System.Entities, ns, expression, skip, take, sortOrder);
        //        var expressionResults = await TrakHoundExpression.Evaluate(_client.System.Entities, expression, skip, take, sortOrder); // DEBUG!!! NEEDS TO ALLOW expression with Namespace included
        //        if (!expressionResults.IsNullOrEmpty())
        //        {
        //            foreach (var expressionResult in expressionResults)
        //            {
        //                var result = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Expression, expressionResult.Expression, expressionResult.Uuid);
        //                result.Skip = skip;
        //                result.Take = take;
        //                result.SortOrder = sortOrder;
        //                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(_router.Id, expression, TrakHoundResultType.Ok, result, result.Uuid));
        //            }
        //        }
        //        else
        //        {
        //            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(_router.Id, expression, TrakHoundResultType.NotFound));
        //        }
        //    }

        //    stpw.Stop();
        //    return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        //}




        public async Task<TrakHoundResponse<bool>> StoreResults(IEnumerable<ITrakHoundObjectQueryResult> queryResults, string requestId = null)
        {
            // Set Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryStoreDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreResults(queryResults);
            };

            // Set Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.StoreResults(queryResults, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("StoreResults", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryStoreDriver>(TrakHoundObjectRoutes.ObjectsStore),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<bool>> StoreQuery(IEnumerable<ITrakHoundQueryRequestResult> queryResults, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryStoreDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreQuery(queryResults);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.StoreQuery(queryResults, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("StoreQuery", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryStoreDriver>(TrakHoundObjectRoutes.ObjectsStore),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query Root"

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> QueryRoot(long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryRootUuids(skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootUuids(long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryRoot(skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryRootUuids(skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Query Root", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> QueryRoot(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryRootUuids(parentUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootUuids(IEnumerable<string> namespaces, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryRoot(namespaces, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryRootUuids(namespaces, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Query Root", requestId, namespaces);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query by Parent"

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByParentUuid(parentUuids, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Query by Parent Uuid", requestId, parentUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessQueryByParentUuidOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> StoreParents(IEnumerable<ITrakHoundObjectQueryResult> queryResults, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryStoreDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreParents(queryResults);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.StoreParents(queryResults, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("StoreParents", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryStoreDriver>(TrakHoundObjectRoutes.ObjectsStore),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query by Child"

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryUuidsByChildUuid(childUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByChildUuid(childUuids, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryUuidsByChildUuid(childUuids, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("QueryByChildUuid", requestId, childUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessQueryByChildUuidOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> StoreChildren(IEnumerable<ITrakHoundObjectQueryResult> queryResults, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryStoreDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreChildren(queryResults);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.StoreChildren(queryResults, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("StoreChildren", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryStoreDriver>(TrakHoundObjectRoutes.ObjectsStore),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query Children by Root"

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryChildUuidsByRootUuid(parentUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryChildrenByRootUuid(parentUuids, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryChildUuidsByRootUuid(parentUuids, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("QueryChildrenByRootUuid", requestId, parentUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessQueryChildrenByRootUuidOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> StoreChildrenByRoots(IEnumerable<ITrakHoundObjectQueryResult> queryResults, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryStoreDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreChildrenByRoots(queryResults);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.StoreChildrenByRoots(queryResults, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("StoreChildrenByRoots", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryStoreDriver>(TrakHoundObjectRoutes.ObjectsStore),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Query Roots by Child"

        public async Task<TrakHoundResponse<ITrakHoundObjectEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var idResponse = await QueryRootUuidsByChildUuid(childUuids, skip, take, sortOrder, requestId);
            var ids = idResponse.Content?.Select(o => o.Uuid).Distinct();

            var entityResponse = await Read(ids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryRootByChildUuid(childUuids, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryRootUuidsByChildUuid(childUuids, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Query by Child Uuid", requestId, childUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessQueryRootsByChildUuidOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> StoreRootsByChildren(IEnumerable<ITrakHoundObjectQueryResult> queryResults, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryStoreDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.StoreRootsByChildren(queryResults);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.StoreRootsByChildren(queryResults, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("StoreRootsByChildren", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryStoreDriver>(TrakHoundObjectRoutes.ObjectsStore),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "List Namespaces"

        public async Task<TrakHoundResponse<string>> ListNamespaces(long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, string>, Task<TrakHoundResponse<string>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.ListNamespaces(skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<string>, Task<TrakHoundResponse<string>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.ListNamespaces(skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("List Namespaces", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>> Subscribe(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>> SubscribeByUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectSubscribeDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.SubscribeByUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectSubscribeDriver>(TrakHoundObjectRoutes.ObjectsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Count"

        public async Task<TrakHoundResponse<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueryDriver, TrakHoundCount>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryChildCount(uuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<TrakHoundCount>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryChildCount(uuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Query Child Count", requestId, uuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueryDriver>(TrakHoundObjectRoutes.ObjectsQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Notify"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>>> Notify(string objectQuery, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>>>();

            var subscription = _notifyHandler.CreateSubscription(objectQuery);
            if (subscription != null)
            {
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>>(Router.Id, objectQuery, TrakHoundResultType.Ok, subscription.Consumer));
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>>(Router.Id, objectQuery, TrakHoundResultType.InternalError));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Index"

        public async Task<TrakHoundResponse<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityIndexQueryDriver<ITrakHoundObjectEntity>, ITrakHoundObjectEntity>, Task<TrakHoundResponse<string>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryIndex(requests, skip, take, sortOrder);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<string>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.QueryIndex(requests, skip, take, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("IndexQuery", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IEntityIndexQueryDriver<ITrakHoundObjectEntity>>(TrakHoundObjectRoutes.ObjectsIndexQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByRootUuid(IEnumerable<string> rootUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectDeleteDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByRootUuid(rootUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.DeleteByRootUuid(rootUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("DeleteByRootUuid", requestId, rootUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectDeleteDriver>(TrakHoundObjectRoutes.ObjectsDelete),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Expire"

        public async Task<TrakHoundResponse<EntityDeleteResult>> Expire(IEnumerable<string> patterns, long created, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectExpirationDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Expire(patterns, created);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.Expire(patterns, created, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("Expire", requestId, patterns);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectExpirationDriver>(TrakHoundObjectRoutes.ObjectsExpire),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(IEnumerable<string> patterns, long lastUpdated, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectExpirationUpdateDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.ExpireByUpdate(patterns, lastUpdated);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.ExpireByUpdate(patterns, lastUpdated, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("ExpireByUpdate", requestId, patterns);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectExpirationUpdateDriver>(TrakHoundObjectRoutes.ObjectsExpireUpdate),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<string> patterns, long lastAccessed, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectExpirationAccessDriver, ITrakHoundObjectEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.ExpireByAccess(patterns, lastAccessed);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Objects.ExpireByAccess(patterns, lastAccessed, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectEntity>("ExpireByAccess", requestId, patterns);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectExpirationAccessDriver>(TrakHoundObjectRoutes.ObjectsExpireAccess),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

    }
}
