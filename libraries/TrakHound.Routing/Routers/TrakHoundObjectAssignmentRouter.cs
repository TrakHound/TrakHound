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
    public class TrakHoundObjectAssignmentRouter : TrakHoundEntityRouter<ITrakHoundObjectAssignmentEntity>
    {
        public TrakHoundObjectAssignmentRouter(TrakHoundRouter router) : base(router) { }


        protected override long FilterQueryRange(TrakHoundResult<ITrakHoundObjectAssignmentEntity> result)
        {
            return result.Content.AddTimestamp;
        }


        internal async Task ProcessCurrentByAssigneeOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectAssignmentEntity>> options)
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
                        var objs = new List<ITrakHoundObjectAssignmentEntity>();
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
                        await EmptyByAssignee(emptyRequests, requestId);
                    }

                    // Process Options from a derived class
                    await OnProcessOptions(requestId, optionType, options);
                }
            }
        }

        internal async Task ProcessCurrentByMemberOptions(string requestId, IEnumerable<RouteRedirectOption<ITrakHoundObjectAssignmentEntity>> options)
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
                        var objs = new List<ITrakHoundObjectAssignmentEntity>();
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


        #region "Subscribe By Assignee"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByAssignee(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectAssignmentEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectAssignmentEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByAssigneeUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentSubscribeDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.SubscribeByAssignee(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.SubscribeByAssigneeUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentSubscribeDriver>(TrakHoundObjectRoutes.AssignmentsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectAssignmentEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe By Member"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByMember(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectAssignmentEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectAssignmentEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByMemberUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentSubscribeDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.SubscribeByMember(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.SubscribeByMemberUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentSubscribeDriver>(TrakHoundObjectRoutes.AssignmentsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectAssignmentEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion


        #region "Current By Assignee"

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(IEnumerable<string> assigneePaths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(assigneePaths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid).Distinct();

            var queryResponse = await CurrentByAssigneeUuid(objectUuids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(IEnumerable<string> assigneeUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentCurrentDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.CurrentByAssignee(assigneeUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.CurrentByAssigneeUuid(assigneeUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Current by Assignee", requestId, assigneeUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentCurrentDriver>(TrakHoundObjectRoutes.AssignmentsCurrent),
                serviceFunction,
                routerFunction
                );

            // Process Options      
            await ProcessCurrentByAssigneeOptions(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion

        #region "Current By Member"

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByMember(IEnumerable<string> memberPaths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(memberPaths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid).Distinct();

            var queryResponse = await CurrentByMemberUuid(objectUuids, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(IEnumerable<string> memberUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentCurrentDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.CurrentByMember(memberUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.CurrentByMemberUuid(memberUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Current by Member", requestId, memberUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentCurrentDriver>(TrakHoundObjectRoutes.AssignmentsCurrent),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessCurrentByMemberOptions(requestId, response.Options);

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion


        #region "Query by Assignee"

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(IEnumerable<string> assigneePaths, long start, long stop, long skip, long take, SortOrder sortOrder, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(assigneePaths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid).Distinct();

            var queryResponse = await QueryByAssigneeUuid(objectUuids, start, stop, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(IEnumerable<string> assigneeUuids, long start, long stop, long skip, long take, SortOrder sortOrder, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var assigneeUuid in assigneeUuids)
            {
                queries.Add(new TrakHoundRangeQuery(assigneeUuid, start, stop));
            }

            return await QueryByAssigneeRange(queries, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeRange(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentQueryDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> serviceFunction = async (serviceRequest) =>
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

                return await serviceRequest.Driver.QueryByAssignee(rangeQueries, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> routerFunction = async (routerRequest) =>
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

                return await routerRequest.Router.Entities.Objects.Assignments.QueryByAssigneeRange(rangeQueries, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Query by Assignee", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentQueryDriver>(TrakHoundObjectRoutes.AssignmentsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Count by Assignee"

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByAssignee(IEnumerable<string> assigneePaths, long start, long stop, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(assigneePaths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await CountByAssigneeUuid(objectUuids, start, stop, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByAssigneeUuid(IEnumerable<string> objectUuids, long start, long stop, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await CountByAssigneeRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByAssigneeRange(IEnumerable<TrakHoundRangeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentQueryDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.CountByAssignee(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Assignments.CountByAssigneeRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Count", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentQueryDriver>(TrakHoundObjectRoutes.AssignmentsQuery),
                serviceFunction,
                routerFunction,
                ProcessCountRange
                );

            return new TrakHoundResponse<TrakHoundCount>(response.Results, response.Duration);
        }

        #endregion


        #region "Query by Member"

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByMember(IEnumerable<string> memberPaths, long start, long stop, long skip, long take, SortOrder sortOrder, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(memberPaths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid).Distinct();

            var queryResponse = await QueryByMemberUuid(objectUuids, start, stop, skip, take, sortOrder, requestId);

            stpw.Stop();
            return HandleResponse(requestId, queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(IEnumerable<string> memberUuids, long start, long stop, long skip, long take, SortOrder sortOrder, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var memberUuid in memberUuids)
            {
                queries.Add(new TrakHoundRangeQuery(memberUuid, start, stop));
            }

            return await QueryByMemberRange(queries, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByMemberRange(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentQueryDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> serviceFunction = async (serviceRequest) =>
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

                return await serviceRequest.Driver.QueryByMember(rangeQueries, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>>> routerFunction = async (routerRequest) =>
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

                return await routerRequest.Router.Entities.Objects.Assignments.QueryByMemberRange(rangeQueries, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Query by Member", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentQueryDriver>(TrakHoundObjectRoutes.AssignmentsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Count by Member"

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByMember(IEnumerable<string> memberPaths, long start, long stop, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(memberPaths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await CountByMemberUuid(objectUuids, start, stop, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByMemberUuid(IEnumerable<string> objectUuids, long start, long stop, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await CountByMemberRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByMemberRange(IEnumerable<TrakHoundRangeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentQueryDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.CountByMember(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Assignments.CountByMemberRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Count", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentQueryDriver>(TrakHoundObjectRoutes.AssignmentsQuery),
                serviceFunction,
                routerFunction,
                ProcessCountRange
                );

            return new TrakHoundResponse<TrakHoundCount>(response.Results, response.Duration);
        }

        #endregion


        #region "Empty"

        public async Task<TrakHoundResponse<bool>> EmptyByAssignee(IEnumerable<EntityEmptyRequest> emptyRequests, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentEmptyDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.EmptyAssignee(emptyRequests);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.EmptyByAssignee(emptyRequests, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Empty by Assignee", requestId, emptyRequests?.Select(o => o.EntityUuid));

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentEmptyDriver>(TrakHoundObjectRoutes.AssignmentsEmpty),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> EmptyByMember(IEnumerable<EntityEmptyRequest> emptyRequests, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentEmptyDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.EmptyMember(emptyRequests);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.EmptyByMember(emptyRequests, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("Empty by Member", requestId, emptyRequests?.Select(o => o.EntityUuid));

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentEmptyDriver>(TrakHoundObjectRoutes.AssignmentsEmpty),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(requestId, response.Results, response.Duration);
        }

        #endregion


        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByAssigneeUuid(IEnumerable<string> assigneeUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentDeleteDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByAssigneeUuid(assigneeUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.DeleteByAssigneeUuid(assigneeUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("DeleteByAssigneeUuid", requestId, assigneeUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentDeleteDriver>(TrakHoundObjectRoutes.AssignmentsDelete),
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
            Func<ParameterRouteTargetDriverRequest<IObjectAssignmentDeleteDriver, ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByMemberUuid(memberUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectAssignmentEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Assignments.DeleteByMemberUuid(memberUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectAssignmentEntity>("DeleteByMemberUuid", requestId, memberUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectAssignmentDeleteDriver>(TrakHoundObjectRoutes.AssignmentsDelete),
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
