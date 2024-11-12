// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.Volumes;

namespace TrakHound.Security
{
    public interface ITrakHoundIdentityProvider
    {
        string Id { get; }

        ITrakHoundIdentityProviderConfiguration Configuration { get; }

        ITrakHoundVolume Volume { get; }


        Task<TrakHoundAuthenticationResponse> Authenticate(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationRequest request);

        Task<TrakHoundAuthenticationSessionCloseResponse> Revoke(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationSessionCloseRequest request);

        Task<TrakHoundAuthenticationCallbackResponse> HandleCallback(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationCallbackRequest request);
    }
}
