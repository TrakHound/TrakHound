// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Mvc;
using TrakHound.Blazor;
using TrakHound.Instances;
using TrakHound.Security;

namespace TrakHound.Instance.Controllers
{
    public class IdentityRevokationController : ControllerBase
    {
        private readonly ITrakHoundInstance _instance;
        private readonly ITrakHoundSecurityManager _identityManager;
        private readonly ILocalStorageService _localStorageService;


        public IdentityRevokationController(ITrakHoundInstance instance, ILocalStorageService localStorageService)
        {
            _instance = instance;
            _identityManager = instance.SecurityManager;
            _localStorageService = localStorageService;
        }


        public async Task<IActionResult> ProcessRevoke()
        {
            var sessionId = TrakHound.Url.GetQueryParameter(HttpContext.Request.QueryString.Value, "sessionId");

            if (!string.IsNullOrEmpty(sessionId))
            {
                var hostConnection = new TrakHoundRequestConnection("HTTP", HttpContext.Connection.LocalIpAddress.ToString());
                var clientConnection = new TrakHoundRequestConnection("HTTP", HttpContext.Connection.RemoteIpAddress.ToString());

                var parameters = new TrakHoundIdentityParameters();
                parameters.Request = new TrakHoundRequestParameterCollection(HttpContext.Request, HttpContext.Response);
                parameters.Session = new TrakHoundSessionParameterCollection(HttpContext.Session);
                parameters.Persistent = new TrakHoundPersistentParameterCollection(_localStorageService);

                var closeSessionRequest = new TrakHoundAuthenticationSessionCloseRequest(sessionId, hostConnection, clientConnection, parameters);
                var closeSessionResponse = await _identityManager.Revoke(closeSessionRequest);

                // Process Response Action
                if (closeSessionResponse.Action != null)
                {
                    switch (closeSessionResponse.Action.Type)
                    {
                        case TrakHoundIdentityActionType.Success:
                            return Ok();

                        case TrakHoundIdentityActionType.Fail:
                            return StatusCode(401);

                        case TrakHoundIdentityActionType.Error:
                            return StatusCode(401);

                        case TrakHoundIdentityActionType.Redirect:
                            var redirectAction = closeSessionResponse.Action as TrakHoundIdentityRedirectAction;
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

            return StatusCode(500);
        }
    }
}
