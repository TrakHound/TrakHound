// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.Volumes;

namespace TrakHound.Security
{
    public abstract class TrakHoundIdentityProviderBase : ITrakHoundIdentityProvider
    {
        private readonly ITrakHoundIdentityProviderConfiguration _configuration;
        private readonly ITrakHoundVolume _volume;

        public ITrakHoundIdentityProviderConfiguration Configuration => _configuration;
        
        public ITrakHoundVolume Volume => _volume;

        private string _id;
        public string Id
        {
            get
            {
                if (_id == null) _id = _configuration?.Id;
                return _id;
            }
        }

        public string Name => GetType().Name;


        public TrakHoundIdentityProviderBase(ITrakHoundIdentityProviderConfiguration configuration, ITrakHoundVolume volume)
        {
            _configuration = configuration;
            _volume = volume;
        }


        public Task<TrakHoundAuthenticationResponse> Authenticate(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationRequest request)
        {
            return OnAuthenticate(securityManager, request);
        }

        protected virtual Task<TrakHoundAuthenticationResponse> OnAuthenticate(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationRequest request)
        {
            return Task.FromResult<TrakHoundAuthenticationResponse>(TrakHoundAuthenticationResponse.NotImplemented(request.Id, Id, request.ResourceId));
        }


        public Task<TrakHoundAuthenticationSessionCloseResponse> Revoke(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationSessionCloseRequest request)
        {
            return OnRevoke(securityManager, request);
        }

        protected virtual Task<TrakHoundAuthenticationSessionCloseResponse> OnRevoke(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationSessionCloseRequest request)
        {
            return Task.FromResult<TrakHoundAuthenticationSessionCloseResponse>(TrakHoundAuthenticationSessionCloseResponse.NotImplemented(null));
        }


        public Task<TrakHoundAuthenticationCallbackResponse> HandleCallback(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationCallbackRequest request)
        {
            return OnHandleCallback(securityManager, request);
        }

        protected virtual Task<TrakHoundAuthenticationCallbackResponse> OnHandleCallback(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationCallbackRequest request)
        {
            return Task.FromResult<TrakHoundAuthenticationCallbackResponse>(TrakHoundAuthenticationCallbackResponse.NotImplemented(null));
        }
    }
}
