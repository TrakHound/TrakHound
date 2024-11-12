// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Entities.Filters;
using TrakHound.Logging;

namespace TrakHound.Routing
{
    class RouteTarget : IRouteTarget
    {
        public string Id { get; set; }

        public string RouteId { get; set; }

        public TrakHoundRouteConfiguration RouteConfiguration { get; set; }

        public TrakHoundTargetConfiguration TargetConfiguration { get; set; }

        public TrakHoundEntityPatternFilter Filter { get; set; }

        public RouteTargetType Type { get; set; }

        /// <summary>
        /// List of Redirects to redirect requests upon the specified 'On' condition returned from the Target
        /// </summary>
        public IEnumerable<RouteRedirect> Redirects { get; set; }


        public RouteTarget(
            TrakHoundRouterConfiguration routerConfiguration,
            TrakHoundRouteConfiguration routeConfiguration,
            TrakHoundTargetConfiguration targetConfiguration,
            TrakHoundEntityPatternFilter filter
            )
        {
            if (routeConfiguration != null && targetConfiguration != null)
            {
                Id = targetConfiguration.Source;
                RouteId = routeConfiguration.Id;
                RouteConfiguration = routeConfiguration;
                TargetConfiguration = targetConfiguration;
                Type = targetConfiguration.Type.ConvertEnum<RouteTargetType>();
                Filter = filter;

                // Read Redirects
                if (!targetConfiguration.Redirects.IsNullOrEmpty())
                {
                    var redirects = new List<RouteRedirect>();
                    foreach (var redirectConfiguration in targetConfiguration.Redirects)
                    {
                        redirects.Add(new RouteRedirect(routerConfiguration, routeConfiguration, redirectConfiguration, filter));
                    }
                    Redirects = redirects;
                }
            }
        }


        public IEnumerable<string> GetIds()
        {
            var ids = new List<string>();

            if (!Redirects.IsNullOrEmpty())
            {
                foreach (var redirect in Redirects)
                {
                    ids.AddRange(redirect.GetTargetIds());
                }
            }

            return ids;
        }


        #region "Internal"

        private static async Task<RouteTargetResponse<TReturn>> RunDriverTarget<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IRouteDriverTarget target,
            TDriver driver,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<TrakHoundResponse<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TReturn>>> routerFunction,
            Func<RouteRequest, IEnumerable<TrakHoundResult<TReturn>>, RouteRequest> processFunction
            )
            where TDriver : ITrakHoundDriver
        {
            // Initialize Response variables
            long responseDuration = 0;
            var responseResults = new List<TrakHoundResult<TReturn>>();
            var responseOptions = new List<RouteRedirectOption<TReturn>>();

            if (target != null)
            {
                try
                {
                    // Read the Response from the Driver
                    var driverResponse = await driverFunction(new RouteTargetDriverRequest<TDriver, TEntity>(target, driver, request));
                    if (driverResponse.HasResults) responseResults.AddRange(driverResponse.Results);

                    logger.LogTrace($"{request.Id} : Router ({routerId}) : {request.Name} => ({driver.Id}) : Driver Target Response in {TimeSpan.FromTicks(driverResponse.Duration).TotalMilliseconds}ms");

                    return await ProcessResults<TDriver, TEntity, TReturn>(routerId, request, logger, target, driver.Id, responseResults, driverFunction, routerFunction, processFunction, driverResponse.Duration);
                }
                catch (Exception ex)
                {
                    if (!request.Queries.IsNullOrEmpty())
                    {
                        foreach (var requestQuery in request.Queries) responseResults.Add(new TrakHoundResult<TReturn>(driver.Id, requestQuery, TrakHoundResultType.InternalError));
                    }
                    else
                    {
                        responseResults.Add(new TrakHoundResult<TReturn>(driver.Id, request.Id, TrakHoundResultType.InternalError));
                    }
                }
            }

            // Return a new ApiTargetResponse
            return new RouteTargetResponse<TReturn>(responseResults, responseDuration, responseOptions);
        }

        private static async Task<RouteTargetResponse<TReturn>> RunDriverTargets<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IRouteDriverTarget target,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<TrakHoundResponse<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TReturn>>> routerFunction,
            Func<RouteRequest, IEnumerable<TrakHoundResult<TReturn>>, RouteRequest> processFunction
            ) 
            where TDriver : ITrakHoundDriver
        {
            long responseDuration = 0;
            var responseResults = new List<TrakHoundResult<TReturn>>();
            var responseOptions = new Dictionary<string, RouteRedirectOption<TReturn>>();

            if (target != null)
            {
                // Get Driver(s) (of type) from DriverTarget
                var drivers = target.GetDrivers<TDriver>();
                if (!drivers.IsNullOrEmpty())
                {
                    foreach (var driver in drivers)
                    {
                        // Run Driver Target
                        var targetResponse = await RunDriverTarget<TDriver, TEntity, TReturn>(routerId, request, logger, target, driver, driverFunction, routerFunction, processFunction);

                        // Set Results
                        if (targetResponse.HasResults) responseResults.AddRange(targetResponse.Results);

                        // Set Options
                        foreach (var option in targetResponse.Options)
                        {
                            var key = $"{option.Option}:{option.Id}";
                            if (!responseOptions.ContainsKey(key))
                            {
                                responseOptions.Add(key, option);
                            }
                        }
                    }
                }
                else
                {
                    if (!request.Queries.IsNullOrEmpty())
                    {
                        foreach (var query in request.Queries)
                        {
                            responseResults.Add(new TrakHoundResult<TReturn>(routerId, query, TrakHoundResultType.RouteNotConfigured));
                        }
                    }
                    else responseResults.Add(new TrakHoundResult<TReturn>(routerId, null, TrakHoundResultType.RouteNotConfigured));

                    return await ProcessResults(routerId, request, logger, target, target.Id, responseResults, driverFunction, routerFunction, processFunction);
                }
            }

            return new RouteTargetResponse<TReturn>(responseResults, responseDuration, responseOptions.Values);
        }


        private static async Task<RouteTargetResponse<TReturn>> RunRouterTarget<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IRouteRouterTarget target,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<TrakHoundResponse<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TReturn>>> routerFunction,
            Func<RouteRequest, IEnumerable<TrakHoundResult<TReturn>>, RouteRequest> processFunction
            )
            where TDriver : ITrakHoundDriver
        {
            // Initialize Response variables
            long responseDuration = 0;
            var responseResults = new List<TrakHoundResult<TReturn>>();
            var responseOptions = new List<RouteRedirectOption<TReturn>>();

            if (target != null)
            {
                var router = target.Router;
                if (router != null)
                {
                    // Read the Response from the Driver
                    var routerResponse = await routerFunction(new RouteTargetRouterRequest<TEntity>(target, router, request));
                    if (routerResponse.HasResults) responseResults.AddRange(routerResponse.Results);

                    logger.LogTrace($"{request.Id} : Router ({routerId}) : {request.Name} => ({router.Id}) : Driver Target Response in {TimeSpan.FromTicks(routerResponse.Duration).TotalMilliseconds}ms");

                    return await ProcessResults(routerId, request, logger, target, router.Id, responseResults, driverFunction, routerFunction, processFunction, routerResponse.Duration);
                }
            }

            // Return a new ApiTargetResponse
            return new RouteTargetResponse<TReturn>(responseResults, responseDuration, responseOptions);
        }


        private static async Task<RouteTargetResponse<TReturn>> ProcessResults<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IRouteTarget target,
            string sourceId,
            IEnumerable<TrakHoundResult<TReturn>> results,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<TrakHoundResponse<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TReturn>>> routerFunction,
            Func<RouteRequest, IEnumerable<TrakHoundResult<TReturn>>, RouteRequest> processFunction,
            long duration = 0
            )
            where TDriver : ITrakHoundDriver
        {
            // Initialize Response variables
            long responseDuration = duration;
            var responseResults = new List<TrakHoundResult<TReturn>>();
            var responseOptions = new List<RouteRedirectOption<TReturn>>();

            // Add Results
            if (!results.IsNullOrEmpty()) responseResults.AddRange(results);

            // Create a Request to use for Redirects (can be adjusted by "processFunction")
            var redirectRequestInstance = new RouteRequest();
            redirectRequestInstance.Name = request.Name;
            redirectRequestInstance.Id = request.Id;
            redirectRequestInstance.AddQueries(request.Queries);
            RouteRequest redirectRequest = redirectRequestInstance;

            // Process the Request (used for adjusting subsequent Queries based on Results)
            if (processFunction != null) redirectRequest = processFunction(redirectRequest, results);

            if (target != null)
            {
                var resultTypes = results.Select(o => o.Type).Distinct();

                foreach (var resultType in resultTypes)
                {
                    var typeResults = results.Where(o => o.Type == resultType);
                    var typeRequests = typeResults.Select(o => o.Request).Distinct();
                    var typeResultsCount = typeResults.Count();
                    var typeRequestsCount = typeRequests.Count();

                    logger.LogTrace($"{request.Id} : {resultType} : {typeRequestsCount} Requests : {typeResultsCount} Results : Router ({routerId}) : {request.Name} => ({sourceId})");

                    // Redirects
                    if (!target.Redirects.IsNullOrEmpty())
                    {
                        // Get the Redirect that contains the ResultType
                        var redirect = target.Redirects.FirstOrDefault(o => o.Conditions.Contains(resultType.ToString()));
                        if (redirect != null)
                        {
                            //logger.LogInformation($"{request.Id} : > Redirect : Router ({routerId}) : {request.Name} => ({sourceId}) Driver Target : Redirecting {typeRequestsCount} Requests on \"{resultType}\"");
                            logger.LogTrace($"{request.Id} : > Redirect : Router ({routerId}) : {request.Name} => ({sourceId}) Driver Target : Redirecting {typeRequestsCount} Requests on \"{resultType}\"");

                            // Run the Redirect Targets
                            var redirectResponse = await Run<TDriver, TEntity, TReturn>(routerId, redirectRequest, logger, redirect.Targets, driverFunction, routerFunction, processFunction, target);
                            if (redirectResponse.HasResults) responseResults.AddRange(redirectResponse.Results);

                            // Set Publish Options based on Redirect Results
                            if (redirectResponse.IsSuccess && !redirect.Options.IsNullOrEmpty())
                            {
                                if (redirect.Options.Any(o => o == RouteRedirectOptions.Publish))
                                {
                                    // Omit the original source (can cause an endless loop)
                                    if (target.Id != sourceId)
                                    {
                                        logger.LogTrace($"{redirectRequest.Id} : Option [{RouteRedirectOptions.Publish}] => Router ({routerId}) set to {RouteRedirectOptions.Publish} {redirectResponse.SuccessResults.Count()} Results");

                                        foreach (var successResult in redirectResponse.SuccessResults)
                                        {
                                            responseOptions.Add(new RouteRedirectOption<TReturn>(RouteRedirectOptions.Publish, successResult.Request, successResult.Content));
                                        }
                                    }
                                }
                            }

                            // Set Empty Options based on Redirect Results
                            if (redirectResponse.IsEmpty && !redirect.Options.IsNullOrEmpty())
                            {
                                if (redirect.Options.Any(o => o == RouteRedirectOptions.Empty))
                                {
                                    // Omit the original source (can cause an endless loop)
                                    if (target.Id != sourceId)
                                    {
                                        logger.LogTrace($"{redirectRequest.Id} : Option [{RouteRedirectOptions.Empty}] => Router ({routerId}) set to {RouteRedirectOptions.Empty} {redirectResponse.EmptyResults.Count()} Results");

                                        foreach (var emptyResult in redirectResponse.EmptyResults)
                                        {
                                            responseOptions.Add(new RouteRedirectOption<TReturn>(RouteRedirectOptions.Empty, emptyResult.Request, emptyResult.Content, emptyResult.Request));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Return a new ApiTargetResponse
            return new RouteTargetResponse<TReturn>(responseResults, responseDuration, responseOptions);
        }


        private static async Task<RouteTargetResponse<TReturn>> RunTarget<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IRouteTarget target,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<TrakHoundResponse<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TReturn>>> routerFunction,
            Func<RouteRequest, IEnumerable<TrakHoundResult<TReturn>>, RouteRequest> processFunction = null,
            IRouteTarget sourceTarget = null
            )
            where TDriver : ITrakHoundDriver
        {
            if (target != null)
            {
                if (sourceTarget != null)
                {
                    logger.LogTrace($"{request.Id} : > Redirect : Router ({routerId}) : Router Target ({sourceTarget.Id}) Redirected to ({target.Id})");
                }

                // Check to see if Target is a Driver Target for the correct Driver Type
                if (target.GetType() == typeof(ParameterRouteDriverTarget))
                {
                    // Cast to Target Driver to the Type of Driver in Router
                    var driverTarget = (ParameterRouteDriverTarget)target;

                    return await RunDriverTargets(routerId, request, logger, driverTarget, driverFunction, routerFunction, processFunction);
                }
                else if (target.GetType() == typeof(ParameterRouteRouterTarget))
                {
                    // Cast to Target Router
                    var routerTarget = (ParameterRouteRouterTarget)target;

                    return await RunRouterTarget(routerId, request, logger, routerTarget, driverFunction, routerFunction, processFunction);
                }
            }

            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var responseResults = new List<TrakHoundResult<TReturn>>();
            foreach (var query in request.Queries)
            {
                responseResults.Add(new TrakHoundResult<TReturn>(routerId, query, TrakHoundResultType.RouteNotConfigured));
            }

            return new RouteTargetResponse<TReturn>(responseResults, stpw.ElapsedTicks);
        }

        #endregion


        public static async Task<RouteTargetResponse<TReturn>> Run<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IEnumerable<IRouteTarget> targets,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<TrakHoundResponse<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TReturn>>> routerFunction,
            Func<RouteRequest, IEnumerable<TrakHoundResult<TReturn>>, RouteRequest> processFunction = null,
            IRouteTarget sourceTarget = null
            )
            where TDriver : ITrakHoundDriver
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var responseResults = new List<TrakHoundResult<TReturn>>();
            var responseOptions = new Dictionary<string, RouteRedirectOption<TReturn>>();

            if (sourceTarget == null)
            {
                logger.LogTrace($"{request.Id} : {request.Name} Request to Router ({routerId}) Received");
            }

            if (!targets.IsNullOrEmpty())
            {
                var responses = new List<RouteTargetResponse<TReturn>>();
                foreach (var target in targets)
                {
                    responses.Add(await RunTarget(routerId, request, logger, target, driverFunction, routerFunction, processFunction, sourceTarget));
                }

                if (!responses.IsNullOrEmpty())
                {
                    foreach (var response in responses)
                    {
                        // Add Results
                        if (response.HasResults) responseResults.AddRange(response.Results);

                        // Add Options
                        foreach (var option in response.Options)
                        {
                            var key = $"{option.Option}:{option.Id}";
                            if (!responseOptions.ContainsKey(key))
                            {
                                responseOptions.Add(key, option);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var query in request.Queries)
                {
                    responseResults.Add(new TrakHoundResult<TReturn>(routerId, query, TrakHoundResultType.RouteNotConfigured));
                }
            }

            stpw.Stop();

            if (sourceTarget == null)
            {
                logger.LogTrace($"{request.Id} : {request.Name} Request to Router ({routerId}) Completed in {stpw.ElapsedMilliseconds}ms");
            }

            return new RouteTargetResponse<TReturn>(responseResults, stpw.ElapsedTicks, responseOptions.Values);
        }

        public static async Task<ITrakHoundConsumer<TReturn>> Subscribe<TDriver, TEntity, TReturn>(
            string routerId,
            RouteRequest request,
            ITrakHoundLogger logger,
            IEnumerable<IRouteTarget> targets,
            Func<RouteTargetDriverRequest<TDriver, TEntity>, Task<ITrakHoundConsumer<TReturn>>> driverFunction,
            Func<RouteTargetRouterRequest<TEntity>, Task<ITrakHoundConsumer<TReturn>>> routerFunction,
            IRouteTarget sourceTarget = null
            )
            where TDriver : ITrakHoundDriver
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var consumers = new List<ITrakHoundConsumer<TReturn>>();
            ITrakHoundConsumer<TReturn> consumer = null;

            if (sourceTarget == null)
            {
                logger.LogTrace( $"{request.Id} : {request.Name} Request to Router ({routerId}) Received");
            }

            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    // Check to see if Target is a Driver Target for the correct Driver Type
                    if (target.GetType() == typeof(ParameterRouteDriverTarget))
                    {
                        // Cast to Target Driver to the Type of Driver in Router
                        var driverTarget = (ParameterRouteDriverTarget)target;

                        var drivers = driverTarget.GetDrivers<TDriver>();
                        if (!drivers.IsNullOrEmpty())
                        {
                            foreach (var driver in drivers)
                            {
                                // Get Consumer from Driver
                                consumer = await driverFunction(new RouteTargetDriverRequest<TDriver, TEntity>(target, driver, request));
                                if (consumer != null) consumers.Add(consumer);
                            }
                        }
                    }
                    else if (target.GetType() == typeof(ParameterRouteRouterTarget))
                    {
                        // Cast to Target Router
                        var routerTarget = (ParameterRouteRouterTarget)target;

                        // Get Consumer from Router
                        consumer = await routerFunction(new RouteTargetRouterRequest<TEntity>(target, routerTarget.Router, request));
                        if (consumer != null) consumers.Add(consumer);
                    }
                }
            }

            stpw.Stop();

            if (sourceTarget == null)
            {
                logger.LogTrace($"{request.Id} : {request.Name} Request to Router ({routerId}) Completed in {stpw.ElapsedMilliseconds}ms");
            }

            if (!consumers.IsNullOrEmpty())
            {
                // Return a new Consumer (combines individual Consumers)
                return new TrakHoundConsumer<TReturn>(consumers);
            }

            return null;
        }
    }
}
