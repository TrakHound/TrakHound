// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public struct TrakHoundAuthenticationResponse
    {
        public string RequestId { get; }

        public string ProviderId { get; }

        public string ResourceId { get; }

        public ITrakHoundIdentityAction Action { get; }

        public ITrakHoundSession Session { get; }


        public TrakHoundAuthenticationResponse(
            string requestId,
            string providerId,
            string resourceId,
            ITrakHoundIdentityAction action,
            ITrakHoundSession session
            )
        {
            RequestId = requestId;
            ProviderId = providerId;
            ResourceId = resourceId;
            Action = action;
            Session = session;
        }


        public static TrakHoundAuthenticationResponse Success(
            string requestId,
            string providerId,
            string resourceId,
            ITrakHoundSession session,
            string message = null
            )
        {
            var action = new TrakHoundIdentitySuccessAction();
            action.Message = message;
            return new TrakHoundAuthenticationResponse(requestId, providerId, resourceId, action, session);
        }

        public static TrakHoundAuthenticationResponse Fail(
            string requestId,
            string providerId,
            string resourceId,
            string message = null
            )
        {
            var action = new TrakHoundIdentityFailAction();
            action.Message = message;
            return new TrakHoundAuthenticationResponse(requestId, providerId, resourceId, action, null);
        }

        public static TrakHoundAuthenticationResponse Error(
            string requestId,
            string providerId,
            string resourceId,
            string message = null
            )
        {
            var action = new TrakHoundIdentityErrorAction();
            action.Message = message;
            return new TrakHoundAuthenticationResponse(requestId, providerId, resourceId, action, null);
        }

        public static TrakHoundAuthenticationResponse NotImplemented(
            string requestId,
            string providerId,
            string resourceId,
            string message = null
            )
        {
            var action = new TrakHoundIdentityNotImplementedAction();
            action.Message = message;
            return new TrakHoundAuthenticationResponse(requestId, providerId, resourceId, action, null);
        }

        public static TrakHoundAuthenticationResponse Redirect(
            string location,
            string requestId,
            string providerId,
            string resourceId
            )
        {
            var action = new TrakHoundIdentityRedirectAction(location);
            return new TrakHoundAuthenticationResponse(requestId, providerId, resourceId, action, null);
        }
    }
}
