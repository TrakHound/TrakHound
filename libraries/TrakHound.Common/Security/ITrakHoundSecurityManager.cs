// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Security
{
    public interface ITrakHoundSecurityManager
    {
        Task<TrakHoundAuthenticationResponse> Authenticate(TrakHoundAuthenticationRequest request);
        
        Task<TrakHoundAuthenticationSessionCloseResponse> Revoke(TrakHoundAuthenticationSessionCloseRequest request);

        Task<TrakHoundAuthenticationCallbackResponse> Callback(string providerId, TrakHoundAuthenticationCallbackRequest request);


        void AddResource(TrakHoundIdentityResourceType resourceType, string resourceId);

        void AddResource(TrakHoundIdentityResourceType resourceType, string resourceId, string permission);

        void AddResource(TrakHoundIdentityResourceType resourceType, string resourceId, IEnumerable<string> permissions);

        TrakHoundIdentityResource GetResource(string resourceId);

        void RemoveResource(string resourceId);


        void AddProfile(string profileId, string providerId, string description = null);

        void AddProfile(TrakHoundSecurityProfile profile);

        IEnumerable<TrakHoundSecurityProfile> GetProfiles();

        void RemoveProfile(string profileId);


        void AddAssignment(string profileId, TrakHoundIdentityAssignment assignment);

        void RemoveAssignment(string assignmentId);


        IEnumerable<ITrakHoundIdentityProvider> GetProviders();

        ITrakHoundIdentityProvider GetProvider(string providerId);


        TrakHoundAuthenticationRequest GetAuthenticationRequest(string requestId);
    }
}
