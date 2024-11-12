// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using TrakHound.Security;
using TrakHound.Volumes;

namespace TrakHound.Token.Identity
{
    public class Module : TrakHoundIdentityProviderBase
    {
        private const string _tokenParameterKey = "token";


        public Module(ITrakHoundIdentityProviderConfiguration configuration, ITrakHoundVolume volume) : base(configuration, volume) { }


        protected override async Task<TrakHoundAuthenticationResponse> OnAuthenticate(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationRequest request)
        {
            var requestToken = await request.Parameters.Request.GetValue(_tokenParameterKey);
            if (!string.IsNullOrEmpty(requestToken))
            {
                // Read Configured Token from configuration
                var configurationToken = Configuration.GetParameter(_tokenParameterKey);

                if (requestToken == configurationToken)
                {
                    var user = new TrakHoundIdentityUser();
                    user.Id = "ExampleUser";
                    user.Type = TrakHoundIdentityUserType.Origin;

                    var session = new TrakHoundAuthenticatedSession();
                    session.SessionId = Guid.NewGuid().ToString();
                    session.ValidFrom = DateTime.Now;
                    session.ValidTo = session.ValidFrom.AddHours(12);
                    session.ProviderId = Id;
                    session.User = user;

                    return TrakHoundAuthenticationResponse.Success(request.Id, Id, request.ResourceId, session, message: "Token Read Successfully");
                }
                else
                {
                    return TrakHoundAuthenticationResponse.Fail(request.Id, Id, request.ResourceId, message: "Invalid Token");
                }
            }
            else
            {
                return TrakHoundAuthenticationResponse.Fail(request.Id, Id, request.ResourceId, message: "Token Not Found");
            }
        }
    }
}
