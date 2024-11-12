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
    public partial class TrakHoundObjectGroupRouter : TrakHoundEntityRouter<ITrakHoundObjectGroupEntity>
    {
        public TrakHoundObjectGroupRouter(TrakHoundRouter router) : base(router) { }


        internal async Task ProcessOptionsByGroup(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectGroupEntity>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var typeOptions = options.Where(o => o.Option == optionType);

                    // Publish Option
                    if (optionType == RouteRedirectOptions.Publish)
                    {
                        var objs = new List<ITrakHoundObjectGroupEntity>();
                        foreach (var option in typeOptions)
                        {
                            if (option.Argument != null) objs.Add(option.Argument);
                        }

                        // Publish to Router
                        await Publish(objs, TrakHoundOperationMode.Async, requestId);
                    }

                    // Empty Option
                    if (optionType == RouteRedirectOptions.Empty)
                    {
                        var emptyRequests = new List<EntityEmptyRequest>();
                        foreach (var option in typeOptions)
                        {
                            if (!string.IsNullOrEmpty(option.Request))
                            {
                                emptyRequests.Add(new EntityEmptyRequest(option.Request));
                            }
                        }

                        // Empty to Router
                        await EmptyByGroup(emptyRequests, requestId);
                    }

                    // Process Options from a derived class
                    await OnProcessOptions(requestId, optionType, options);
                }
            }
        }

        internal async Task ProcessOptionsByMember(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectGroupEntity>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var typeOptions = options.Where(o => o.Option == optionType);

                    // Publish Option
                    if (optionType == RouteRedirectOptions.Publish)
                    {
                        var objs = new List<ITrakHoundObjectGroupEntity>();
                        foreach (var option in typeOptions)
                        {
                            if (option.Argument != null) objs.Add(option.Argument);
                        }

                        // Publish to Router
                        await Publish(objs, TrakHoundOperationMode.Async, requestId);
                    }

                    // Empty Option
                    if (optionType == RouteRedirectOptions.Empty)
                    {
                        var emptyRequests = new List<EntityEmptyRequest>();
                        foreach (var option in typeOptions)
                        {
                            if (!string.IsNullOrEmpty(option.Request))
                            {
                                emptyRequests.Add(new EntityEmptyRequest(option.Request));
                            }
                        }

                        // Empty to Router
                        await EmptyByMember(emptyRequests, requestId);
                    }

                    // Process Options from a derived class
                    await OnProcessOptions(requestId, optionType, options);
                }
            }
        }


        #region "Subscribe By Group"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByGroup(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectGroupEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectGroupEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByGroupUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupSubscribeDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.SubscribeByGroup(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.SubscribeByGroupUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupSubscribeDriver>(TrakHoundObjectRoutes.GroupsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectGroupEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe By Member"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByMember(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectGroupEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectGroupEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByMemberUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupSubscribeDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.SubscribeByMember(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.SubscribeByMemberUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupSubscribeDriver>(TrakHoundObjectRoutes.GroupsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectGroupEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion


        #region "Query by Group"

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByGroup(IEnumerable<string> groupPaths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var groupQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(groupPaths, requestId: requestId);
            var groupUuids = groupQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByGroupUuid(groupUuids, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByGroupUuid(IEnumerable<string> groupUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupQueryDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByGroup(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.QueryByGroupUuid(routerRequest.Request.Queries.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("Query by Group", requestId, groupUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupQueryDriver>(TrakHoundObjectRoutes.GroupsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptionsByGroup(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion

        #region "Query by Member"

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByMember(IEnumerable<string> memberPaths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var memberQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(memberPaths, requestId: requestId);
            var memberUuids = memberQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByMemberUuid(memberUuids, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByMemberUuid(IEnumerable<string> memberUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupQueryDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryByMember(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.QueryByMemberUuid(routerRequest.Request.Queries.Select(o => o.Query), skip, take, sortOrder, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("Query by Member", requestId, memberUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupQueryDriver>(TrakHoundObjectRoutes.GroupsQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptionsByMember(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion


        #region "Empty"

        public async Task<TrakHoundResponse<bool>> EmptyByGroup(IEnumerable<EntityEmptyRequest> emptyRequests, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupEmptyDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.EmptyByGroup(emptyRequests);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.EmptyByGroup(emptyRequests, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("Empty by Group", requestId, emptyRequests?.Select(o => o.EntityUuid));

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupEmptyDriver>(TrakHoundObjectRoutes.GroupsEmpty),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> EmptyByMember(IEnumerable<EntityEmptyRequest> emptyRequests, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupEmptyDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.EmptyByMember(emptyRequests);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.EmptyByMember(emptyRequests, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("Empty by Member", requestId, emptyRequests?.Select(o => o.EntityUuid));

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupEmptyDriver>(TrakHoundObjectRoutes.GroupsEmpty),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion


        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByGroupUuid(IEnumerable<string> assigneeUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupDeleteDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByGroupUuid(assigneeUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Groups.DeleteByGroupUuid(assigneeUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("DeleteByGroupUuid", requestId, assigneeUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupDeleteDriver>(TrakHoundObjectRoutes.GroupsDelete),
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

        public async Task<TrakHoundResponse<bool>> DeleteByMemberUuid(IEnumerable<string> memberUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectGroupDeleteDriver, ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByMemberUuid(memberUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectGroupEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.DeleteByMemberUuid(memberUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectGroupEntity>("DeleteByMemberUuid", requestId, memberUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectGroupDeleteDriver>(TrakHoundObjectRoutes.GroupsDelete),
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
