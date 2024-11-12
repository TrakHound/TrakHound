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
    public class TrakHoundObjectQueueRouter : TrakHoundEntityRouter<ITrakHoundObjectQueueEntity>
    {
        private readonly ITrakHoundClient _client;
        private readonly TrakHoundObjectNotifyHandler _notifyHandler;


        public TrakHoundObjectQueueRouter(TrakHoundRouter router) : base(router) 
        {
            _client = router.Client;
            _notifyHandler = new TrakHoundObjectNotifyHandler(_client);
        }


        protected async override Task OnAfterPublish(IEnumerable<TrakHoundPublishResult<ITrakHoundObjectQueueEntity>> results, IEnumerable<ITrakHoundObjectQueueEntity> entities, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                await UpdateNotifications(entities);
            }
        }

        protected async override Task OnBeforeDelete(IEnumerable<EntityDeleteRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                await UpdateNotifications(requests);
            }
        }

        private async Task UpdateNotifications(string queueUuid)
        {
            if (!string.IsNullOrEmpty(queueUuid))
            {
                var objectEntity = await _client.System.Entities.Objects.ReadByUuid(queueUuid);
                if (objectEntity != null)
                {
                    _notifyHandler.AddPublishNotification(TrakHoundEntityNotificationType.ComponentChanged, objectEntity);
                }
            }
        }

        private async Task UpdateNotifications(IEnumerable<ITrakHoundObjectQueueEntity> entities)
        {
            var objectUuids = entities.Select(o => o.QueueUuid).Distinct();
            var objectEntities = await _client.System.Entities.Objects.ReadByUuid(objectUuids);

            foreach (var objectEntity in objectEntities)
            {
                _notifyHandler.AddPublishNotification(TrakHoundEntityNotificationType.ComponentChanged, objectEntity);
            }
        }

        private async Task UpdateNotifications(IEnumerable<EntityDeleteRequest> requests)
        {
            var uuids = requests.Select(o => o.Target).Distinct();
            var entities = await _client.System.Entities.Objects.Queue.ReadByUuid(uuids);
            if (!entities.IsNullOrEmpty())
            {
                var objectUuids = entities.Select(o => o.QueueUuid).Distinct();
                var objectEntities = await _client.System.Entities.Objects.ReadByUuid(objectUuids);

                foreach (var objectEntity in objectEntities)
                {
                    _notifyHandler.AddPublishNotification(TrakHoundEntityNotificationType.ComponentChanged, objectEntity);
                }
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> Pull(string queueUuid, int count = 1, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueuePullDriver, ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Pull(queueUuid, count);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Queues.Pull(queueUuid, count, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectQueueEntity>("Pull", requestId, queueUuid);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueuePullDriver>(TrakHoundObjectRoutes.QueuesPull),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);

                // Notify that Queue has Changed
                await UpdateNotifications(queueUuid);
            }

            return HandleResponse(requestId, response.Results, response.Duration);
        }


        #region "Query By Queue"

        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByQueue(IEnumerable<string> queuePaths, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, long queueSkip = 0, long queueTake = 1000, SortOrder queueSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var queueQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(queuePaths, queueSkip, queueTake, queueSortOrder, requestId);
            var queueUuids = queueQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByQueueUuid(queueUuids, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByQueueUuid(IEnumerable<string> queueUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueueQueryDriver, ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByQueue(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Queues.QueryByQueueUuid(routerRequest.Request.Queries.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectQueueEntity>("Query By Queue", requestId, queueUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueueQueryDriver>(TrakHoundObjectRoutes.QueuesQuery),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion

        #region "Query By Member"

        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByMember(IEnumerable<string> memberPaths, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, long memberSkip = 0, long memberTake = 1000, SortOrder memberSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var memberQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(memberPaths, memberSkip, memberTake, memberSortOrder, requestId);
            var memberUuids = memberQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByMemberUuid(memberUuids, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByMemberUuid(IEnumerable<string> memberUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueueQueryDriver, ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByMember(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Queues.QueryByMemberUuid(routerRequest.Request.Queries.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectQueueEntity>("Query By Member", requestId, memberUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueueQueryDriver>(TrakHoundObjectRoutes.QueuesQuery),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion


        #region "Subscribe By Queue"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>> SubscribeByQueue(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectQueueEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectQueueEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>> SubscribeByQueueUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueueSubscribeDriver, ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.SubscribeByQueue(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Queues.SubscribeByQueueUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectQueueEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueueSubscribeDriver>(TrakHoundObjectRoutes.QueuesSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectQueueEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe By Member"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>> SubscribeByMember(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectQueueEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectQueueEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>> SubscribeByMemberUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectQueueSubscribeDriver, ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.SubscribeByMember(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectQueueEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Queues.SubscribeByMemberUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectQueueEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectQueueSubscribeDriver>(TrakHoundObjectRoutes.QueuesSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectQueueEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>>(request.Id, response.Results, response.Duration);
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

    }
}
