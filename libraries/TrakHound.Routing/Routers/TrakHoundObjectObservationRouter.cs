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
    public class TrakHoundObjectObservationRouter : TrakHoundEntityRouter<ITrakHoundObjectObservationEntity>
    {
        public TrakHoundObjectObservationRouter(TrakHoundRouter router) : base(router) { }


        protected override long FilterQueryRange(TrakHoundResult<ITrakHoundObjectObservationEntity> result)
        {
            return result.Content.Timestamp;
        }


        #region "Latest"

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> LatestByObject(IEnumerable<string> paths, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await LatestByObjectUuid(objectUuids, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationLatestDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Latest(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Observations.LatestByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Latest", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationLatestDriver>(TrakHoundObjectRoutes.ObservationsLatest),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> QueryByObject(IEnumerable<string> paths, long start, long stop, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await QueryByObjectUuid(objectUuids, start, stop, skip, take, sortOrder, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids) 
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await QueryByRange(queries, skip, take, sortOrder, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationQueryDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> serviceFunction = async (serviceRequest) =>
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

                return await serviceRequest.Driver.Query(rangeQueries, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> routerFunction = async (routerRequest) =>
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

                return await routerRequest.Router.Entities.Objects.Observations.QueryByRange(rangeQueries, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Query", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationQueryDriver>(TrakHoundObjectRoutes.ObservationsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionGroups, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationQueryDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> serviceFunction = async (serviceRequest) =>
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

                return await serviceRequest.Driver.Query(rangeQueries, conditionGroups, querySkip, queryTake, sortOrder);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> routerFunction = async (routerRequest) =>
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

                return await routerRequest.Router.Entities.Objects.Observations.QueryByRange(rangeQueries, conditionGroups, querySkip, queryTake, sortOrder, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Query", requestId, queries, skip, take);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationQueryDriver>(TrakHoundObjectRoutes.ObservationsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryRange
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Last"

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> LastByObject(IEnumerable<string> paths, long timestamp, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await LastByObjectUuid(objectUuids, timestamp, requestId);

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> LastByObjectUuid(IEnumerable<string> objectUuids, long timestamp, string requestId = null)
        {
            var queries = new List<TrakHoundTimeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundTimeQuery(objectUuid, timestamp));
            }

            return await LastByRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> LastByRange(IEnumerable<TrakHoundTimeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationQueryDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryTimestamp = query.GetParameter<long>("timestamp");
                    rangeQueries.Add(new TrakHoundTimeQuery(query.Query, queryTimestamp));
                }

                return await serviceRequest.Driver.Last(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryTimestamp = query.GetParameter<long>("timestamp");
                    rangeQueries.Add(new TrakHoundTimeQuery(query.Query, queryTimestamp));
                }

                return await routerRequest.Router.Entities.Objects.Observations.LastByRange(rangeQueries, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Last", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationQueryDriver>(TrakHoundObjectRoutes.ObservationsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryTime
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> LastByRange(IEnumerable<TrakHoundTimeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionGroups, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationQueryDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryTimestamp = query.GetParameter<long>("timestamp");
                    rangeQueries.Add(new TrakHoundTimeQuery(query.Query, queryTimestamp));
                }

                return await serviceRequest.Driver.Last(rangeQueries, conditionGroups);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundTimeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryTimestamp = query.GetParameter<long>("timestamp");
                    rangeQueries.Add(new TrakHoundTimeQuery(query.Query, queryTimestamp));
                }

                return await routerRequest.Router.Entities.Objects.Observations.LastByRange(rangeQueries, conditionGroups, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Last", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationQueryDriver>(TrakHoundObjectRoutes.ObservationsQuery),
                serviceFunction,
                routerFunction,
                ProcessQueryTime
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(response.Results, response.Duration);
        }

        #endregion

        #region "Aggregate"

        public async Task<TrakHoundResponse<TrakHoundAggregate>> AggregateByObject(IEnumerable<string> paths, TrakHoundAggregateType aggregateType, long start, long stop, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await AggregateByObjectUuid(objectUuids, aggregateType, start, stop, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundAggregate>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundAggregate>> AggregateByObjectUuid(IEnumerable<string> objectUuids, TrakHoundAggregateType aggregateType, long start, long stop, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await AggregateByRange(queries, aggregateType, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundAggregate>> AggregateByRange(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundAggregateType aggregateType, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationAggregateDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<TrakHoundAggregate>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.Aggregate(rangeQueries, aggregateType);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<TrakHoundAggregate>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Observations.AggregateByRange(rangeQueries, aggregateType, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Aggregate", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationAggregateDriver>(TrakHoundObjectRoutes.ObservationsAggregate),
                serviceFunction,
                routerFunction,
                ProcessAggregateRange
                );

            return new TrakHoundResponse<TrakHoundAggregate>(response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<TrakHoundAggregateWindow>> AggregateWindowByObject(IEnumerable<string> paths, TrakHoundAggregateType aggregateType, long start, long stop, long window, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await AggregateWindowByObjectUuid(objectUuids, aggregateType, start, stop, window, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundAggregateWindow>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(IEnumerable<string> objectUuids, TrakHoundAggregateType aggregateType, long start, long stop, long window, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await AggregateWindowByRange(queries, aggregateType, window, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundAggregateWindow>> AggregateWindowByRange(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundAggregateType aggregateType, long window, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationAggregateDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<TrakHoundAggregateWindow>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.AggregateWindow(rangeQueries, aggregateType, window);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<TrakHoundAggregateWindow>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Observations.AggregateWindowByRange(rangeQueries, aggregateType, window, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("AggregateWindow", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationAggregateDriver>(TrakHoundObjectRoutes.ObservationsAggregate),
                serviceFunction,
                routerFunction,
                ProcessAggregateWindowRange
                );

            return new TrakHoundResponse<TrakHoundAggregateWindow>(response.Results, response.Duration);
        }

        #endregion

        #region "Count"

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByObject(IEnumerable<string> paths, long start, long stop, long objectSkip = 0, long objectTake = 1000, SortOrder objectSortOrder = SortOrder.Ascending, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var objectQueryResponse = await Router.Entities.Objects.Objects.QueryUuids(paths, objectSkip, objectTake, objectSortOrder, requestId);
            var objectUuids = objectQueryResponse.Content?.Select(o => o.Uuid);

            var queryResponse = await CountByObjectUuid(objectUuids, start, stop, requestId);

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(queryResponse.Results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByObjectUuid(IEnumerable<string> objectUuids, long start, long stop, string requestId = null)
        {
            var queries = new List<TrakHoundRangeQuery>();
            foreach (var objectUuid in objectUuids)
            {
                queries.Add(new TrakHoundRangeQuery(objectUuid, start, stop));
            }

            return await CountByRange(queries, requestId);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByRange(IEnumerable<TrakHoundRangeQuery> queries, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationQueryDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<TrakHoundCount>>> serviceFunction = async (serviceRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in serviceRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await serviceRequest.Driver.Count(rangeQueries);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<TrakHoundCount>>> routerFunction = async (routerRequest) =>
            {
                var rangeQueries = new List<TrakHoundRangeQuery>();
                foreach (var query in routerRequest.Request.Queries)
                {
                    var queryStart = query.GetParameter<long>("start");
                    var queryStop = query.GetParameter<long>("stop");
                    rangeQueries.Add(new TrakHoundRangeQuery(query.Query, queryStart, queryStop));
                }

                return await routerRequest.Router.Entities.Objects.Observations.CountByRange(rangeQueries, requestId: routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Count", requestId, queries);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationQueryDriver>(TrakHoundObjectRoutes.ObservationsQuery),
                serviceFunction,
                routerFunction,
                ProcessCountRange
                );

            return new TrakHoundResponse<TrakHoundCount>(response.Results, response.Duration);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> SubscribeByObject(IEnumerable<string> paths, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>>();

            var response = await Subscribe(requestId);
            if (response.Content != null)
            {
                var filter = new TrakHoundEntityPatternFilter(_router.Client.System.Entities, 100);
                filter.Allow(paths);

                var entityConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>(response.Content);
                entityConsumer.OnReceivedAsync = (entity) =>
                {
                    filter.MatchAsync(entity);
                    return null;
                };

                var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>();
                resultConsumer.OnDisposed = () =>
                {
                    entityConsumer.Dispose();
                    filter.Dispose();
                };

                filter.MatchesReceived += (s, matchResults) =>
                {
                    var entities = new List<ITrakHoundObjectObservationEntity>();
                    foreach (var matchResult in matchResults)
                    {
                        entities.Add((ITrakHoundObjectObservationEntity)matchResult.Entity);
                    }
                    resultConsumer.Push(entities);
                };

                foreach (var path in paths)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(Router.Id, path, TrakHoundResultType.Ok, resultConsumer));
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> SubscribeByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationSubscribeDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Observations.SubscribeByObjectUuid(objectUuids, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectObservationEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationSubscribeDriver>(TrakHoundObjectRoutes.ObservationsSubscribe),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<ITrakHoundObjectObservationEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectObservationDeleteDriver, ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectObservationEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Observations.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectMetadataEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectObservationDeleteDriver>(TrakHoundObjectRoutes.ObservationsDelete),
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
