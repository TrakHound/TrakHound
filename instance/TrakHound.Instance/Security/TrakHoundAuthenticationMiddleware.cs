// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Api;
using TrakHound.Blazor;
using TrakHound.Http;
using TrakHound.Instance.Services;
using TrakHound.Instances;
using TrakHound.Logging;
using TrakHound.Security;

namespace TrakHound.Instance.Security
{
    public class TrakHoundAuthenticationMiddleware
    {
        public const string CallbackPrefix = "/_identity/callback";
        public const string RevokePrefix = "/_identity/revoke";

        private static readonly ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundAuthenticationMiddleware>();
        private readonly ITrakHoundInstance _instance;
        private readonly ITrakHoundSecurityManager _identityManager;
        private readonly RequestDelegate _next;


        public TrakHoundAuthenticationMiddleware(ITrakHoundInstance instance, ITrakHoundSecurityManager identityManager, RequestDelegate next)
        {
            _instance = instance;
            _identityManager = identityManager;
            _next = next;
        }


        public async Task Invoke(HttpContext context, ITrakHoundInstance instance, TrakHoundAuthenticationSessionService authenticationSessionService, Blazored.LocalStorage.ILocalStorageService localStorageService)
        {
            if (!IsStaticRoute(context.Request.Path) && !IsAdminRoute(context.Request.Path))
            {
                // Prevent processing Callback request
                if (!context.Request.Path.StartsWithSegments(CallbackPrefix) && !context.Request.Path.StartsWithSegments(RevokePrefix))
                {
                    var parameters = new TrakHoundIdentityParameters();
                    parameters.Request = new TrakHoundRequestParameterCollection(context.Request, context.Response);
                    parameters.Session = new TrakHoundSessionParameterCollection(context.Session);
                    parameters.Persistent = new TrakHoundPersistentParameterCollection(localStorageService);

                    var resourceId = GetResourceId(instance, context);
                    if (resourceId != null)
                    {
                        var resource = _identityManager.GetResource(resourceId);
                        if (resource != null)
                        {
                            var connectionProtocol = context.Request.IsHttps ? "https" : "http";

                            var hostConnection = new TrakHoundRequestConnection(connectionProtocol, context.Request.Host.ToString());
                            var clientConnection = new TrakHoundRequestConnection(connectionProtocol, context.Connection.RemoteIpAddress.ToString());

                            // Create Request 
                            var request = new TrakHoundAuthenticationRequest(resourceId, hostConnection, clientConnection, parameters);

                            // Will need to add ability to store Body as well. Probably a struct?
                            //LinkStore.Add(request.Id, $"{context.Request.}{context.Request.QueryString}");
                            var requestedUrl = "/" + Url.Combine(_instance.Configuration.BasePath, $"{context.Request.Path}{context.Request.QueryString}");
                            _logger.LogError($"requestedUrl = {requestedUrl}");
                            LinkStore.Add(request.Id, requestedUrl);
                            //var requestedUrl = Url.Combine($"{context.Request.Scheme}://{context.Request.Host}", context.Request.PathBase, context.Request.Path);
                            //LinkStore.Add(request.Id, requestedUrl);

                            // Authenticate
                            var response = await _identityManager.Authenticate(request);

                            // Process Response Action
                            if (response.Action != null)
                            {
                                switch (response.Action.Type)
                                {
                                    case TrakHoundIdentityActionType.Success:

                                        context.Response.StatusCode = 200;

                                        if (response.Session != null)
                                        {
                                            // Create Scope ID
                                            var scopeId = Guid.NewGuid().ToString();
                                            context.Session.SetString(TrakHoundAuthenticationService.GetScopeIdKey(instance.Id), scopeId);

                                            // Add Authentication Session
                                            authenticationSessionService.Add(scopeId, response.Session);

                                            // Check Permissions for Authorization
                                            if (!resource.Permissions.IsNullOrEmpty() && !response.Session.Roles.IsNullOrEmpty())
                                            {
                                                if (!response.Session.Roles.Contains("*"))
                                                {
                                                    var success = false;
                                                    foreach (var permission in resource.Permissions)
                                                    {
                                                        if (response.Session.Roles.Contains("*") || response.Session.Roles.Contains(permission))
                                                        {
                                                            success = true;
                                                            break;
                                                        }
                                                    }

                                                    if (!success)
                                                    {
                                                        context.Response.StatusCode = 403;
                                                        return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Console.WriteLine($"Authorization Successful : Any Permission");
                                            }
                                        }

                                        break;

                                    case TrakHoundIdentityActionType.Fail:
                                        context.Response.StatusCode = 401;
                                        return;

                                    case TrakHoundIdentityActionType.Error:
                                        context.Response.StatusCode = 401;
                                        return;

                                    case TrakHoundIdentityActionType.Redirect:
                                        var redirectAction = response.Action as TrakHoundIdentityRedirectAction;
                                        if (redirectAction != null)
                                        {
                                            // Get Redirect Location (if present)
                                            var currentLocation = context.Request.Path.ToString()?.Trim('/').ToLower();
                                            var requestedLocation = redirectAction.Location?.Trim('/').ToLower();

                                            // Check if Redirect Location is same as Current Location (prevent endless looping)
                                            if (requestedLocation != currentLocation)
                                            {
                                                context.Response.StatusCode = 303;
                                                context.Response.Headers.Append("Location", redirectAction.Location);
                                                return;
                                            }
                                            else
                                            {
                                                // Return Success if already at the Redirect Location
                                                context.Response.StatusCode = 200;
                                            }
                                        }
                                        break;
                                }
                            }

                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }

        private static bool IsStaticRoute(PathString path)
        {
            var s = path.ToString();

            if (s.StartsWith("/_blazor")) return true;     // Blazor
            if (s.StartsWith("/_framework")) return true;  // Blazor
            if (s.StartsWith("/_content")) return true;    // Blazor
            if (s.StartsWith("/_packages")) return true;   // TrakHound Package (css, js, etc.) (this may need to be authenticated?)
            if (s.StartsWith("/_images")) return true;     // TrakHound Explorer Images Controller (this may need to be authenticated?)
            if (s.StartsWith("/img")) return true;         // Static Image Files
            if (s.StartsWith("/css")) return true;         // Static CSS Files

            return false;
        }

        private static bool IsAdminRoute(PathString path)
        {
            var s = path.ToString();

            if (s.StartsWith("/_admin")) return true;

            return false;
        }

        private string GetResourceId(ITrakHoundInstance instance, HttpContext httpContext)
        {
            string resourceId = null;
            if (resourceId == null) resourceId = GetSystemResourceId(instance, httpContext);
            if (resourceId == null) resourceId = GetApiResourceId(instance, httpContext);
            if (resourceId == null) resourceId = GetAppResourceId(instance, httpContext);

            return resourceId;
        }


        private string GetSystemResourceId(ITrakHoundInstance instance, HttpContext httpContext)
        {
            var route = httpContext.Request.Path.ToString();
            var requestedRoute = route.Trim('/');
            requestedRoute = $"/{requestedRoute}";

            if (requestedRoute.StartsWith("/_api/information")) return "System.Api.Information";

            return null;
        }

        private string GetAppResourceId(ITrakHoundInstance instance, HttpContext httpContext)
        {
            var route = httpContext.Request.Path.ToString();
            var requestedRoute = route.Trim('/');
            requestedRoute = $"/{requestedRoute}";

            if (instance.AppProvider.IsRouteValid(requestedRoute))
            {
                var pageInformation = instance.AppProvider.GetPageInformation(requestedRoute);
                if (pageInformation != null)
                {
                    return $"app:{pageInformation.App.Id}:{pageInformation.Id}";
                }
            }

            return null;
        }

        private string GetApiResourceId(ITrakHoundInstance instance, HttpContext httpContext)
        {
            var method = httpContext.Request.Method;
            var route = httpContext.Request.Path.ToString();

            var requestedRoute = route.Trim('/');
            if (requestedRoute.StartsWith(TrakHoundHttpApiController.RoutePrefix))
            {
                requestedRoute = requestedRoute.Remove(0, TrakHoundHttpApiController.RoutePrefix.Length);

                if (instance.ApiProvider.IsRouteValid(requestedRoute))
                {
                    TrakHoundApiEndpointInformation endpointInformation = null;

                    if (TrakHoundHttpApiController.IsQueryRequest(method, requestedRoute))
                    {
                        //Console.WriteLine($"API Query Resource Requested : Route = {requestedRoute}");
                        endpointInformation = instance.ApiProvider.GetEndpointInformation("Query", requestedRoute);
                    }
                    else if (TrakHoundHttpApiController.IsSubscribeRequest(method, requestedRoute))
                    {
                        var subsribeRoute = Url.RemoveLastFragment(requestedRoute); // remove 'subscribe' suffix
                        //Console.WriteLine($"API Subscribe Resource Requested : Route = {subsribeRoute}");
                        endpointInformation = instance.ApiProvider.GetEndpointInformation("Subscribe", requestedRoute);
                    }
                    else if (TrakHoundHttpApiController.IsPublishRequest(method, requestedRoute))
                    {
                        var publishRoute = Url.RemoveLastFragment(requestedRoute); // remove 'publish' suffix
                        //Console.WriteLine($"API Publish Resource Requested : Route = {publishRoute}");
                        endpointInformation = instance.ApiProvider.GetEndpointInformation("Publish", publishRoute);
                    }
                    else if (TrakHoundHttpApiController.IsDeleteRequest(method, requestedRoute))
                    {
                        var deleteRoute = requestedRoute.EndsWith(TrakHoundHttpApiController.DeleteSuffix) ? Url.RemoveLastFragment(requestedRoute) : requestedRoute; // remove 'delete' suffix
                        //Console.WriteLine($"API Delete Resource Requested : Route = {deleteRoute}");
                        endpointInformation = instance.ApiProvider.GetEndpointInformation("Delete", deleteRoute);
                    }

                    if (endpointInformation != null)
                    {
                        var resourceId = $"api:{endpointInformation.Api.Id}:{endpointInformation.Controller.Id}:{endpointInformation.Id}";

                        //Console.WriteLine($"API Endpoint Resource Requested : ID = {resourceId}");

                        return resourceId;
                    }
                }
            }

            return null;
        }
    }
}
