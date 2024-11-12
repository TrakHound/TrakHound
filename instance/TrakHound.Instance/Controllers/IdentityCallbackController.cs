// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Mvc;
using TrakHound.Blazor;
using TrakHound.Instance.Services;
using TrakHound.Instances;
using TrakHound.Security;

namespace TrakHound.Instance.Controllers
{
    public class IdentityCallbackController : ControllerBase
    {
        private readonly ITrakHoundInstance _instance;
        private readonly ITrakHoundSecurityManager _identityManager;
        private readonly ILocalStorageService _localStorageService;


        public IdentityCallbackController(ITrakHoundInstance instance, ILocalStorageService localStorageService)
        {
            _instance = instance;
            _identityManager = instance.SecurityManager;
            _localStorageService = localStorageService;
        }


        public async Task<IActionResult> ProcessCallback()
        {
            var providerId = TrakHound.Url.GetRouteParameter(HttpContext.Request.Path, "/_identity/callback/{providerId}", "providerId");
            if (!string.IsNullOrEmpty(providerId))
            {
                var hostConnection = new TrakHoundRequestConnection("HTTP", HttpContext.Connection.LocalIpAddress.ToString());
                var clientConnection = new TrakHoundRequestConnection("HTTP", HttpContext.Connection.RemoteIpAddress.ToString());

                var parameters = new TrakHoundIdentityParameters();
                parameters.Request = new TrakHoundRequestParameterCollection(HttpContext.Request, HttpContext.Response);
                parameters.Session = new TrakHoundSessionParameterCollection(HttpContext.Session);
                parameters.Persistent = new TrakHoundPersistentParameterCollection(_localStorageService);

                var callbackRequest = new TrakHoundAuthenticationCallbackRequest(hostConnection, clientConnection, parameters);
                var callbackResponse = await _identityManager.Callback(providerId, callbackRequest);

                // Process Response Action
                if (callbackResponse.Action != null)
                {
                    switch (callbackResponse.Action.Type)
                    {
                        case TrakHoundIdentityActionType.Success:
                            var link = LinkStore.Get(callbackResponse.AuthenticationRequestId);
                            if (link != null)
                            {
                                return Redirect(link);
                            }
                            else
                            {
                                return NotFound();
                            }

                        case TrakHoundIdentityActionType.Fail:
                            return StatusCode(401);

                        case TrakHoundIdentityActionType.Error:
                            return StatusCode(401);

                        case TrakHoundIdentityActionType.Redirect:
                            var redirectAction = callbackResponse.Action as TrakHoundIdentityRedirectAction;
                            if (redirectAction != null)
                            {
                                // Get Redirect Location (if present)
                                var currentLocation = HttpContext.Request.Path.ToString()?.Trim('/').ToLower();
                                var requestedLocation = TrakHound.Url.Decode(redirectAction.Location)?.Trim('/').ToLower();
                                if (!requestedLocation.StartsWith("http://") || !requestedLocation.StartsWith("https://"))
                                {
                                    var baseUrl = TrakHound.Url.Combine(HttpContext.Request.Host.Value, _instance.Configuration.BasePath);
                                    requestedLocation = $"{HttpContext.Request.Scheme}://{TrakHound.Url.Combine(baseUrl, requestedLocation)}".Trim('/').ToLower();
                                }

                                // Check if Redirect Location is same as Current Location (prevent endless looping)
                                if (requestedLocation != currentLocation)
                                {
                                    return Redirect(requestedLocation);
                                }
                                else
                                {
                                    // Return Success if already at the Redirect Location
                                    return Ok();
                                }
                            }
                            break;
                    }
                }
            }
            else
            {

            }

            return StatusCode(500);
        }
    }
}
