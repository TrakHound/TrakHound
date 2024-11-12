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
    public partial class TrakHoundObjectMetadataRouter : TrakHoundEntityRouter<ITrakHoundObjectMetadataEntity>
    {
        public TrakHoundObjectMetadataRouter(TrakHoundRouter router) : base(router) { }
     

        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(IEnumerable<string> entityUuids, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectMetadataQueryDriver, ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Metadata.QueryByEntityUuid(routerRequest.Request.Queries.Select(o => o.Query), routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectMetadataEntity>("Query", requestId, entityUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectMetadataQueryDriver>(TrakHoundObjectRoutes.MetadataQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(IEnumerable<string> entityUuids, string name, TrakHoundMetadataQueryType queryType, string query, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectMetadataQueryDriver, ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query), name, queryType, query);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Metadata.QueryByEntityUuid(routerRequest.Request.Queries.Select(o => o.Query), name, queryType, query, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectMetadataEntity>("Query", requestId, entityUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectMetadataQueryDriver>(TrakHoundObjectRoutes.MetadataQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> QueryByName(string name, TrakHoundMetadataQueryType queryType, string query, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectMetadataQueryDriver, ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(name, queryType, query);
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Metadata.QueryByName(name, queryType, query, routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundObjectMetadataEntity>("Query", requestId, name);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectMetadataQueryDriver>(TrakHoundObjectRoutes.MetadataQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(requestId, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<IObjectMetadataDeleteDriver, ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<bool>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.DeleteByObjectUuid(objectUuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundObjectMetadataEntity>, Task<TrakHoundResponse<bool>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Objects.Metadata.DeleteByObjectUuid(objectUuids, routerRequest.Request.Id);
            };


            var request = new EntityParameterRouteRequest<ITrakHoundObjectMetadataEntity>("DeleteByObjectUuid", requestId, objectUuids);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IObjectMetadataDeleteDriver>(TrakHoundObjectRoutes.MetadataDelete),
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
