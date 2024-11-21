// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Logging;

namespace TrakHound.Routing.Routers
{
    public abstract class TrakHoundEntityRouter<TEntity> : ITrakHoundEntityRouter<TEntity> where TEntity : ITrakHoundEntity
    {
        private static readonly ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundEntityRouter<TEntity>>();

        internal TrakHoundRouter _router;


        public TrakHoundRouter Router => _router;

        internal ITrakHoundLogger Logger => _logger;


        public TrakHoundEntityRouter(TrakHoundRouter router)
        {
            if (router != null && router.Configuration != null)
            {
                _router = router;
            }
        }


        protected virtual Task OnBeforePublish(IEnumerable<TEntity> entities, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            return Task.CompletedTask;
        }

		protected virtual Task OnAfterPublish(IEnumerable<TrakHoundPublishResult<TEntity>> results, IEnumerable<TEntity> entities, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
		{
			return Task.CompletedTask;
		}

		protected virtual Task OnBeforeDelete(IEnumerable<EntityDeleteRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
		{
			return Task.CompletedTask;
		}

		protected virtual Task OnAfterDelete(IEnumerable<EntityDeleteRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
		{
			return Task.CompletedTask;
		}

        protected virtual Task OnBeforeIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnAfterIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnBeforeExpire(IEnumerable<EntityDeleteRequest> requests, string requestId = null)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnAfterExpire(IEnumerable<EntityDeleteRequest> requests, string requestId = null)
        {
            return Task.CompletedTask;
        }


        #region "Internal"

        internal static TrakHoundResponse<TReturn> HandleResponse<TReturn>(
            string requestId,
            IEnumerable<TrakHoundResult<TReturn>> results,
            long duration = 0
            )
        {
            var response = new TrakHoundResponse<TReturn>(results, duration);

            _logger.LogTrace($"{requestId} : [Request] Completed in {response.Duration / 10000}ms");

            return response;
        }


        internal virtual Task OnProcessOptions(string requestId, RouteRedirectOptions optionType, IEnumerable<RouteRedirectOption<TEntity>> options) => Task.CompletedTask;

        internal virtual Task OnProcessOptions(string requestId, RouteRedirectOptions optionType, IEnumerable<RouteRedirectOption<bool>> options) => Task.CompletedTask;


        internal async Task ProcessOptions(string requestId, IEnumerable<RouteRedirectOption<TEntity>> options)
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
                        var objs = new List<TEntity>();
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
                        var entityIds = new List<EntityEmptyRequest>();
                        foreach (var option in typeOptions)
                        {
                            if (!string.IsNullOrEmpty(option.Request))
                            {
                                entityIds.Add(new EntityEmptyRequest(option.Request));
                            }
                        }

                        // Empty to Router
                        await Router.Entities.Empty<TEntity>(TrakHoundRoutes.Get(typeof(IEntityEmptyDriver<TEntity>)), entityIds, TrakHoundOperationMode.Async, requestId);
                    }

                    // Process Options from a derived class
                    await OnProcessOptions(requestId, optionType, options);
                }
            }
        }

        internal async Task ProcessOptions(string requestId, IEnumerable<RouteRedirectOption<bool>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    // Process Options from a derived class
                    await OnProcessOptions(requestId, optionType, options);
                }
            }
        }


        protected virtual long FilterQueryRange(TrakHoundResult<TEntity> result)
        {
            return 0;
        }

        protected virtual long FilterSpanRange(TrakHoundResult<TrakHoundTimeRangeSpan> result)
        {
            return 0;
        }

        protected virtual long FilterCountRange(TrakHoundResult<TrakHoundCount> result)
        {
            return 0;
        }

        protected virtual long FilterAggregateRange(TrakHoundResult<TrakHoundAggregate> result)
        {
            return 0;
        }

        protected virtual long FilterAggregateWindowRange(TrakHoundResult<TrakHoundAggregateWindow> result)
        {
            return 0;
        }


        internal IParameterRouteRequest ProcessQueryTime(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TEntity>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();
                long count = 0;

                foreach (var query in request.Queries)
                {
                    long queryTimestamp = query.GetParameter<long>("timestamp");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterQueryRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterQueryRange(o));
                            count += okResults.Count();
                        }
                    }

                    long redirectTimestamp = maxFound > queryTimestamp ? maxFound + 1 : queryTimestamp;

                    if (redirectTimestamp > 0 || count == 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Query Redirect = {query.Query} ::: Timestamp = {redirectTimestamp}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("timestamp", redirectTimestamp));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Add Queries
                if (!redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessQueryRange(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TEntity>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();
                long skip = request.GetParameter<long>("skip");
                long take = request.GetParameter<long>("take");

                long count = 0;

                foreach (var query in request.Queries)
                {
                    long queryStart = query.GetParameter<long>("start");
                    long queryStop = query.GetParameter<long>("stop");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterQueryRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterQueryRange(o));
                            count += okResults.Count();
                        }
                    }

                    long redirectStart = maxFound > queryStart ? maxFound + 1 : queryStart;
                    long redirectStop = queryStop;

                    if (redirectStop - redirectStart > 0 || count == 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Query Redirect = {query.Query} ::: Start = {redirectStart} :: Stop = {redirectStop}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("start", redirectStart));
                        redirectParameters.Add(new RouteQueryParameter("stop", redirectStop));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Adjust Skip & Take based on read results
                skip = count > 0 ? Math.Max(0, skip - count) : skip;
                take = take - count;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                requestParameters.Add(new RouteQueryParameter("skip", skip));
                requestParameters.Add(new RouteQueryParameter("take", take));
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if (take > 0 && !redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessQueryTimeRange(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TEntity>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();
                long skip = request.GetParameter<long>("skip");
                long take = request.GetParameter<long>("take");

                long count = 0;

                foreach (var query in request.Queries)
                {
                    long queryStart = query.GetParameter<long>("start");
                    long queryStop = query.GetParameter<long>("stop");
                    long querySpan = query.GetParameter<long>("span");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterQueryRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterQueryRange(o));
                            count += okResults.Count();
                        }
                    }

                    long redirectStart = maxFound > queryStart ? maxFound + 1 : queryStart;
                    long redirectStop = queryStop;

                    if (redirectStop - redirectStart > 0 || count == 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Query Redirect = {query.Query} ::: Start = {redirectStart} :: Stop = {redirectStop}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("start", redirectStart));
                        redirectParameters.Add(new RouteQueryParameter("stop", redirectStop));
                        redirectParameters.Add(new RouteQueryParameter("span", querySpan));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Adjust Skip & Take based on read results
                skip = count > 0 ? Math.Max(0, skip - count) : skip;
                take = take - count;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                requestParameters.Add(new RouteQueryParameter("skip", skip));
                requestParameters.Add(new RouteQueryParameter("take", take));
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if (take > 0 && !redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessQueryTimeRangeId(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TEntity>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();
                long skip = request.GetParameter<long>("skip");
                long take = request.GetParameter<long>("take");
                bool paging = skip > 0 || take > 0;

                long count = 0;

                foreach (var query in request.Queries)
                {
                    var timeRangeIds = query.GetParameters("timeRangeId");
                    if (!timeRangeIds.IsNullOrEmpty())
                    {
                        var redirectParameters = new List<RouteQueryParameter>();
                        foreach (var timeRangeId in timeRangeIds)
                        {
                            redirectParameters.Add(new RouteQueryParameter("timeRangeId", timeRangeId));
                        }
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Adjust Skip & Take based on read results
                skip = count > 0 ? Math.Max(0, skip - count) : skip;
                take = take - count;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                requestParameters.Add(new RouteQueryParameter("skip", skip));
                requestParameters.Add(new RouteQueryParameter("take", take));
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if ((!paging || take > 0) && !redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessSpanRange(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TrakHoundTimeRangeSpan>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();

                foreach (var query in request.Queries)
                {
                    long queryStart = query.GetParameter<long>("start");
                    long queryStop = query.GetParameter<long>("stop");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterSpanRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterSpanRange(o));
                        }
                    }

                    long redirectStart = maxFound > queryStart ? maxFound + 1 : queryStart;
                    long redirectStop = queryStop;

                    if (redirectStop - redirectStart > 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Span Redirect = {query.Query} ::: Start = {redirectStart} :: Stop = {redirectStop}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("start", redirectStart));
                        redirectParameters.Add(new RouteQueryParameter("stop", redirectStop));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if (!redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessCountRange(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TrakHoundCount>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();

                foreach (var query in request.Queries)
                {
                    long queryStart = query.GetParameter<long>("start");
                    long queryStop = query.GetParameter<long>("stop");
                    long querySpan = query.GetParameter<long>("span");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterCountRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterCountRange(o));
                        }
                    }

                    long redirectStart = maxFound > queryStart ? maxFound + 1 : queryStart;
                    long redirectStop = queryStop;

                    if (redirectStop - redirectStart > 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Count Redirect = {query.Query} ::: Start = {redirectStart} :: Stop = {redirectStop}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("start", redirectStart));
                        redirectParameters.Add(new RouteQueryParameter("stop", redirectStop));
                        redirectParameters.Add(new RouteQueryParameter("span", querySpan));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if (!redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessAggregateRange(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TrakHoundAggregate>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();

                foreach (var query in request.Queries)
                {
                    long queryStart = query.GetParameter<long>("start");
                    long queryStop = query.GetParameter<long>("stop");
                    long querySpan = query.GetParameter<long>("span");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterAggregateRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterAggregateRange(o));
                        }
                    }

                    long redirectStart = maxFound > queryStart ? maxFound + 1 : queryStart;
                    long redirectStop = queryStop;

                    if (redirectStop - redirectStart > 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Count Redirect = {query.Query} ::: Start = {redirectStart} :: Stop = {redirectStop}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("start", redirectStart));
                        redirectParameters.Add(new RouteQueryParameter("stop", redirectStop));
                        redirectParameters.Add(new RouteQueryParameter("span", querySpan));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if (!redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        internal IParameterRouteRequest ProcessAggregateWindowRange(IParameterRouteRequest request, IEnumerable<TrakHoundResult<TrakHoundAggregateWindow>> results)
        {
            if (!request.Queries.IsNullOrEmpty())
            {
                var redirectQueries = new List<RouteQuery>();

                foreach (var query in request.Queries)
                {
                    long queryStart = query.GetParameter<long>("start");
                    long queryStop = query.GetParameter<long>("stop");
                    long querySpan = query.GetParameter<long>("span");

                    long maxFound = 0;
                    long minFound = 0;

                    var queryResults = results.Where(o => o.Request == query.Query);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var okResults = queryResults.Where(o => o.Type == TrakHoundResultType.Ok);
                        if (!okResults.IsNullOrEmpty())
                        {
                            maxFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Max(o => FilterAggregateWindowRange(o));
                            minFound = results.Where(o => o.Type == TrakHoundResultType.Ok).Min(o => FilterAggregateWindowRange(o));
                        }
                    }

                    long redirectStart = maxFound > queryStart ? maxFound + 1 : queryStart;
                    long redirectStop = queryStop;

                    if (redirectStop - redirectStart > 0)
                    {
                        Router.Logger?.LogTrace($"{request.Id} : > Count Redirect = {query.Query} ::: Start = {redirectStart} :: Stop = {redirectStop}");

                        var redirectParameters = new List<RouteQueryParameter>();
                        redirectParameters.Add(new RouteQueryParameter("start", redirectStart));
                        redirectParameters.Add(new RouteQueryParameter("stop", redirectStop));
                        redirectParameters.Add(new RouteQueryParameter("span", querySpan));
                        redirectQueries.Add(new RouteQuery(query.Query, redirectParameters));
                    }
                }

                var redirectRequest = new ParameterRouteRequest();
                redirectRequest.Name = request.Name;
                redirectRequest.Id = request.Id;

                // Add Skip & Take Parameters to Request
                var requestParameters = new List<RouteQueryParameter>();
                redirectRequest.Parameters = requestParameters;

                // Add Queries
                if (!redirectQueries.IsNullOrEmpty()) redirectRequest.AddQueries(redirectQueries);

                return redirectRequest;
            }

            return request;
        }

        #endregion


        public async Task<TrakHoundResponse<TEntity>> Read(IEnumerable<string> uuids, string requestId = null)
        {
            return await _router.Entities.Read<TEntity>(TrakHoundRoutes.Get(typeof(IEntityReadDriver<TEntity>)), uuids, requestId);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>> Subscribe(string requestId = null)
        {
            return await _router.Entities.Subscribe<TEntity>(TrakHoundRoutes.Get(typeof(IEntitySubscribeDriver<TEntity>)), requestId);
        }

		public async Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish(IEnumerable<TEntity> entities, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            var dEntities = entities?.ToDistinct();

            await OnBeforePublish(dEntities, publishMode, requestId);

            var response = await _router.Entities.Publish<TEntity>(TrakHoundRoutes.Get(typeof(IEntityPublishDriver<TEntity>)), dEntities, publishMode, requestId);

            await OnAfterPublish(response.Content, dEntities, publishMode, requestId);

			return response;
        }

        public async Task<TrakHoundResponse<bool>> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
            await OnBeforeIndex(requests, publishMode, requestId);

            var response = await _router.Entities.UpdateIndex<TEntity>(TrakHoundRoutes.Get(typeof(IEntityIndexUpdateDriver<TEntity>)), requests, publishMode, requestId);

            await OnAfterIndex(requests, publishMode, requestId);

            return response;
        }

        public async Task<TrakHoundResponse<bool>> Delete(IEnumerable<EntityDeleteRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null)
        {
			await OnBeforeDelete(requests, publishMode, requestId);

			var response = await _router.Entities.Delete<TEntity>(TrakHoundRoutes.Get(typeof(IEntityDeleteDriver<TEntity>)), requests, publishMode, requestId);

			await OnAfterDelete(requests, publishMode, requestId);

			return response;
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests, string requestId = null)
        {
            await OnBeforeExpire(requests, requestId);

            var response = await _router.Entities.Expire<TEntity>(TrakHoundRoutes.Get(typeof(IEntityExpirationDriver<TEntity>)), requests, requestId);

            await OnAfterExpire(requests, requestId);

            return response;
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests, string requestId = null)
        {
            await OnBeforeExpire(requests, requestId);

            var response = await _router.Entities.ExpireByAccess<TEntity>(TrakHoundRoutes.Get(typeof(IEntityExpirationAccessDriver<TEntity>)), requests, requestId);

            await OnAfterExpire(requests, requestId);

            return response;
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests, string requestId = null)
        {
            await OnBeforeExpire(requests, requestId);

            var response = await _router.Entities.ExpireByUpdate<TEntity>(TrakHoundRoutes.Get(typeof(IEntityExpirationUpdateDriver<TEntity>)), requests, requestId);

            await OnAfterExpire(requests, requestId);

            return response;
        }
    }
}
