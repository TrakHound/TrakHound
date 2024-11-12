// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using TrakHound.Instances;
using TrakHound.Security;

namespace TrakHound.Blazor
{
    public class TrakHoundAuthenticationService
    {
        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundAuthenticationSessionService _sessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public TrakHoundAuthenticationService(ITrakHoundInstance instance, TrakHoundAuthenticationSessionService sessionService, IHttpContextAccessor httpContextAccessor)
        {
            _instance = instance;
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ITrakHoundSession> GetSession()
        {
            var sessionParameters = new TrakHoundSessionParameterCollection(_httpContextAccessor.HttpContext?.Session);
            var sessionId = await sessionParameters.GetValue(GetScopeIdKey(_instance.Id));
            if (sessionId != null)
            {
                return _sessionService.Get(sessionId);
            }

            return null;
        }

        public static string GetScopeIdKey(string instanceId)
        {
            return $"trakhound.instance.{instanceId}.authentication.scopeId";
        }
    }
}
