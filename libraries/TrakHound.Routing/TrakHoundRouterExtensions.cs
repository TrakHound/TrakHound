// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Routing.Routers;

namespace TrakHound.Routing
{
    internal static class TrakHoundRouterExtensions
    {
        private const string PublishOperationName = "Publish";
        private const string EmptyOperationName = "Empty";
        private const string IndexOperationName = "Index";
        private const string DeleteOperationName = "Delete";
        private const string ExpireOperationName = "Expire";
        private const string ExpireAccessOperationName = "Expire-Access";
        private const string ExpireUpdateOperationName = "Expire-Update";


        public static async Task<TrakHoundResponse<TEntity>> Read<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<string> entityIds,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Read Driver Function
            Func<RouteTargetDriverRequest<IEntityReadDriver<TEntity>, TEntity>, Task<TrakHoundResponse<TEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Read(serviceRequest.Request.Queries);
            };

            // Set Read Router Function
            Func<RouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TEntity>>> routerFunction = async (routerRequest) =>
            {
                return await Read<TEntity>(routerRequest.Router.Entities, route, routerRequest.Request.Queries, routerRequest.Request.Id);
            };

            var request = new EntityRouteRequest<TEntity>("Read", requestId, entityIds);

            // Run Targets
            var response = await RouteTarget.Run(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityReadDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await router.ProcessOptions(request.Id, response.Options);

            return TrakHoundEntityRouter<TEntity>.HandleResponse<TEntity>(request.Id, response.Results, response.Duration);
        }

        public static async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>> Subscribe<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntitySubscribeDriver<TEntity>, TEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe();
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>>> routerFunction = async (routerRequest) =>
            {
                return await Subscribe<TEntity>(routerRequest.Router.Entities, route, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>("Subscribe", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntitySubscribeDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<TEntity>.HandleResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>(request.Id, response.Results, response.Duration);
        }


        public static async Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<TEntity> entities,
            TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Publish Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityPublishDriver<TEntity>, TEntity>, Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>>> serviceFunction = async (serviceRequest) =>
            {
                var filteredEntities = await Filter(serviceRequest.Target, entities);
                if (!filteredEntities.IsNullOrEmpty())
                {
                    if (publishMode == TrakHoundOperationMode.Sync)
                    {
                        return await serviceRequest.Driver.Publish(filteredEntities);
                    }
                    else
                    {
                        return AddToQueue(router, serviceRequest.Driver, filteredEntities);
                    }
                }
                else
                {
                    return new TrakHoundResponse<TrakHoundPublishResult<TEntity>>();
                }
            };

            // Set Publish Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>>> routerFunction = async (routerRequest) =>
            {
                var filteredEntities = await Filter(routerRequest.Target, entities);
                if (!filteredEntities.IsNullOrEmpty())
                {
                    return await Publish<TEntity>(routerRequest.Router.Entities, route, entities, publishMode, routerRequest.Request.Id);
                }
                else
                {
                    return new TrakHoundResponse<TrakHoundPublishResult<TEntity>>();
                }
            };

            var request = new EntityParameterRouteRequest<TEntity>(PublishOperationName, requestId, entities?.Select(o => o.Uuid));

            var response = await ParameterRouteTarget.Run(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityPublishDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await router.ProcessOptions(request.Id, response.Options);

            return TrakHoundEntityRouter<TEntity>.HandleResponse<TrakHoundPublishResult<TEntity>>(request.Id, response.Results, response.Duration);
        }

        private static TrakHoundResponse<TrakHoundPublishResult<TEntity>> AddToQueue<TEntity>(
            TrakHoundEntitiesRouter router,
            IEntityPublishDriver<TEntity> driver,
            IEnumerable<TEntity> items
            )
            where TEntity : ITrakHoundEntity
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TrakHoundPublishResult<TEntity>>>();

            if (driver != null && !items.IsNullOrEmpty())
            {
                var buffer = router.Buffers.GetPublishBuffer<TEntity>(driver.Configuration.Id);
                if (buffer != null)
                {
                    foreach (var item in items)
                    {
                        var success = buffer.Add(item);
                        var resultType = success ? TrakHoundResultType.Ok : TrakHoundResultType.InternalError;
                        var publishResult = new TrakHoundPublishResult<TEntity>(TrakHoundPublishResultType.Queued, item);

                        results.Add(new TrakHoundResult<TrakHoundPublishResult<TEntity>>(driver.Id, item.Uuid, resultType, publishResult));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundPublishResult<TEntity>>(results, stpw.ElapsedTicks);
        }


        public static async Task<TrakHoundResponse<bool>> Empty<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<EntityEmptyRequest> requests,
            TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityEmptyDriver<TEntity>, TEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                if (publishMode == TrakHoundOperationMode.Sync)
                {
                    return await serviceRequest.Driver.Empty(requests);
                }
                else
                {
                    return AddToQueue(router, serviceRequest.Driver, requests);
                }
            };

            // Set Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await Empty<TEntity>(routerRequest.Router.Entities, route, requests, publishMode, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>(EmptyOperationName, requestId, requests.Select(o => o.EntityUuid));

            var response = await ParameterRouteTarget.Run<IEntityEmptyDriver<TEntity>, TEntity, bool>(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityEmptyDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }


        private static TrakHoundResponse<bool> AddToQueue<TEntity>(
            TrakHoundEntitiesRouter router,
            IEntityEmptyDriver<TEntity> driver,
            IEnumerable<EntityEmptyRequest> requests
            )
            where TEntity : ITrakHoundEntity
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (driver != null && !requests.IsNullOrEmpty())
            {
                var buffer = router.Buffers.GetEmptyBuffer<TEntity>(driver.Configuration.Id);
                if (buffer != null)
                {
                    foreach (var request in requests)
                    {
                        var success = buffer.Add(request);
                        var resultType = success ? TrakHoundResultType.Ok : TrakHoundResultType.InternalError;
                        results.Add(new TrakHoundResult<bool>(driver.Id, request.EntityUuid, resultType, success));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }


        public static async Task<TrakHoundResponse<bool>> UpdateIndex<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<EntityIndexPublishRequest> requests,
            TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Publish Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityIndexUpdateDriver<TEntity>, TEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                if (publishMode == TrakHoundOperationMode.Sync)
                {
                    return await serviceRequest.Driver.UpdateIndex(requests);
                }
                else
                {
                    return AddToQueue(router, serviceRequest.Driver, requests);
                }
            };

            // Set Publish Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await UpdateIndex<TEntity>(routerRequest.Router.Entities, route, requests, publishMode, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>(IndexOperationName, requestId, requests.Select(o => o.Target));

            var response = await ParameterRouteTarget.Run<IEntityIndexUpdateDriver<TEntity>, TEntity, bool>(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityIndexUpdateDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await router.ProcessOptions(request.Id, response.Options);
            }

            return TrakHoundEntityRouter<TEntity>.HandleResponse<bool>(request.Id, response.Results, response.Duration);
        }


        private static TrakHoundResponse<bool> AddToQueue<TEntity>(
            TrakHoundEntitiesRouter router,
            IEntityIndexUpdateDriver<TEntity> driver,
            IEnumerable<EntityIndexPublishRequest> items
            )
            where TEntity : ITrakHoundEntity
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (driver != null && !items.IsNullOrEmpty())
            {
                var buffer = router.Buffers.GetIndexBuffer<TEntity>(driver.Configuration.Id);
                if (buffer != null)
                {
                    foreach (var item in items)
                    {
                        var success = buffer.Add(item);
                        var resultType = success ? TrakHoundResultType.Ok : TrakHoundResultType.InternalError;
                        results.Add(new TrakHoundResult<bool>(driver.Id, item.Target, resultType, success));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }


        public static async Task<TrakHoundResponse<bool>> Delete<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<EntityDeleteRequest> requests,
            TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Publish Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityDeleteDriver<TEntity>, TEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                if (publishMode == TrakHoundOperationMode.Sync)
                {
                    return await serviceRequest.Driver.Delete(requests);
                }
                else
                {
                    return AddToQueue(router, serviceRequest.Driver, requests);
                }
            };

            // Set Publish Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await Delete<TEntity>(routerRequest.Router.Entities, route, requests, publishMode, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>(DeleteOperationName, requestId, requests.Select(o => o.Target));

            var response = await ParameterRouteTarget.Run<IEntityDeleteDriver<TEntity>, TEntity, bool>(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityDeleteDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await router.ProcessOptions(request.Id, response.Options);
            }

            return TrakHoundEntityRouter<TEntity>.HandleResponse<bool>(request.Id, response.Results, response.Duration);
        }


        private static TrakHoundResponse<bool> AddToQueue<TEntity>(
            TrakHoundEntitiesRouter router,
            IEntityDeleteDriver<TEntity> driver,
            IEnumerable<EntityDeleteRequest> items
            )
            where TEntity : ITrakHoundEntity
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (driver != null && !items.IsNullOrEmpty())
            {
                var buffer = router.Buffers.GetDeleteBuffer<TEntity>(driver.Configuration.Id);
                if (buffer != null)
                {
                    foreach (var item in items)
                    {
                        var success = buffer.Add(item);
                        var resultType = success ? TrakHoundResultType.Ok : TrakHoundResultType.InternalError;
                        results.Add(new TrakHoundResult<bool>(driver.Id, item.Target, resultType, success));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }


        public static async Task<TrakHoundResponse<EntityDeleteResult>> Expire<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<EntityDeleteRequest> requests,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Publish Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityExpirationDriver<TEntity>, TEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Expire(requests);
            };

            // Set Publish Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> routerFunction = async (routerRequest) =>
            {
                return await Expire<TEntity>(routerRequest.Router.Entities, route, requests, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>(ExpireOperationName, requestId, requests.Select(o => o.Target));

            var response = await ParameterRouteTarget.Run<IEntityExpirationDriver<TEntity>, TEntity, EntityDeleteResult>(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityExpirationDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<TEntity>.HandleResponse<EntityDeleteResult>(request.Id, response.Results, response.Duration);
        }

        public static async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<EntityDeleteRequest> requests,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Publish Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityExpirationAccessDriver<TEntity>, TEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.ExpireByAccess(requests);
            };

            // Set Publish Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> routerFunction = async (routerRequest) =>
            {
                return await ExpireByAccess<TEntity>(routerRequest.Router.Entities, route, requests, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>(ExpireAccessOperationName, requestId, requests.Select(o => o.Target));

            var response = await ParameterRouteTarget.Run<IEntityExpirationAccessDriver<TEntity>, TEntity, EntityDeleteResult>(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityExpirationAccessDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<TEntity>.HandleResponse<EntityDeleteResult>(request.Id, response.Results, response.Duration);
        }

        public static async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate<TEntity>(
            this TrakHoundEntitiesRouter router,
            string route,
            IEnumerable<EntityDeleteRequest> requests,
            string requestId = null
            )
            where TEntity : ITrakHoundEntity
        {
            // Set Publish Driver Function
            Func<ParameterRouteTargetDriverRequest<IEntityExpirationUpdateDriver<TEntity>, TEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.ExpireByUpdate(requests);
            };

            // Set Publish Router Function
            Func<ParameterRouteTargetRouterRequest<TEntity>, Task<TrakHoundResponse<EntityDeleteResult>>> routerFunction = async (routerRequest) =>
            {
                return await ExpireByUpdate<TEntity>(routerRequest.Router.Entities, route, requests, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<TEntity>(ExpireUpdateOperationName, requestId, requests.Select(o => o.Target));

            var response = await ParameterRouteTarget.Run<IEntityExpirationUpdateDriver<TEntity>, TEntity, EntityDeleteResult>(
                router.Router.Id,
                request,
                router.Router.Logger,
                router.Router.GetTargets<IEntityExpirationUpdateDriver<TEntity>>(route),
                serviceFunction,
                routerFunction
                );

            return TrakHoundEntityRouter<TEntity>.HandleResponse<EntityDeleteResult>(request.Id, response.Results, response.Duration);
        }


        public static async Task ProcessOptions<TEntity>(this TrakHoundEntitiesRouter router, string requestId, IEnumerable<RouteRedirectOption<TEntity>> options) where TEntity : ITrakHoundEntity
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
                        await Publish<TEntity>(router, TrakHoundRoutes.Get(typeof(IEntityPublishDriver<TEntity>)), objs, TrakHoundOperationMode.Async, requestId);
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
                        await Empty<TEntity>(router, TrakHoundRoutes.Get(typeof(IEntityEmptyDriver<TEntity>)), entityIds, TrakHoundOperationMode.Async, requestId);
                    }
                }
            }
        }

        public static async Task ProcessOptions<TEntity>(this TrakHoundEntitiesRouter router, string requestId, IEnumerable<RouteRedirectOption<TrakHoundPublishResult<TEntity>>> options) where TEntity : ITrakHoundEntity
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
                            if (option.Argument.Result != null) objs.Add(option.Argument.Result);
                        }

                        // Publish to Router
                        await Publish<TEntity>(router, TrakHoundRoutes.Get(typeof(IEntityPublishDriver<TEntity>)), objs, TrakHoundOperationMode.Async, requestId);
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
                        await Empty<TEntity>(router, TrakHoundRoutes.Get(typeof(IEntityEmptyDriver<TEntity>)), entityIds, TrakHoundOperationMode.Async, requestId);
                    }
                }
            }
        }

        public static async Task ProcessOptions(this TrakHoundEntitiesRouter router, string requestId, IEnumerable<RouteRedirectOption<bool>> options)
        {
            if (!options.IsNullOrEmpty())
            {
                var optionTypes = options.Select(o => o.Option).Distinct();
                foreach (var optionType in optionTypes)
                {
                    var typeOptions = options.Where(o => o.Option == optionType);
                }
            }
        }

        private static async Task<TEntity> Filter<TEntity>(IRouteTarget target, TEntity inputEntity) where TEntity : ITrakHoundEntity
        {
            if (target.Filter != null && inputEntity != null)
            {
                if (await target.Filter.Match(inputEntity))
                {
                    return inputEntity;
                }
                else
                {
                    return default;
                }
            }

            return inputEntity;
        }

        private static async Task<IEnumerable<TEntity>> Filter<TEntity>(IRouteTarget target, IEnumerable<TEntity> inputEntities) where TEntity : ITrakHoundEntity
        {
            if (target.Filter != null && !inputEntities.IsNullOrEmpty())
            {
                var input = new List<ITrakHoundEntity>();
                foreach (var entity in inputEntities) input.Add(entity);

                var matches = await target.Filter.Match(input);
                if (!matches.IsNullOrEmpty())
                {
                    var output = new List<TEntity>();
                    foreach (var match in matches) output.Add((TEntity)match.Entity);
                    return output;
                }
                else
                {
                    return null;
                }
            }

            return inputEntities;
        }
    }
}
