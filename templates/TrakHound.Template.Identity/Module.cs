using System.Threading.Tasks;
using TrakHound.Security;

namespace $projectname$
{
    public class Module : TrakHoundIdentityProviderBase
    {
        public Module(ITrakHoundIdentityProviderConfiguration configuration) : base(configuration) { }


        protected override async Task<TrakHoundAuthenticationResponse> OnAuthenticate(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationRequest request)
        {
            return TrakHoundAuthenticationResponse.NotImplemented(request.Id, Id, request.ResourceId, message: "Not Implemented");
        }

        protected override async Task<TrakHoundAuthenticationCallbackResponse> OnHandleCallback(ITrakHoundSecurityManager securityManager, TrakHoundAuthenticationCallbackRequest request)
        {
            // Authentication Request ID should be read from either the request parameters or an external source

            return TrakHoundAuthenticationCallbackResponse.NotImplemented("[AUTHENTICATION_REQUEST_ID]", message: "Not Implemented");
        }
    }
}
