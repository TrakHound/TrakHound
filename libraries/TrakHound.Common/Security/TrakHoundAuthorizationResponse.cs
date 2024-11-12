// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Security
{
    public struct TrakHoundAuthorizationResponse
    {
        public string RequestId { get; }

        public string ProviderId { get; }

        public string ResourceId { get; }

        public ITrakHoundIdentityAction Action { get; }

        public ITrakHoundSession Session { get; }


        public TrakHoundAuthorizationResponse(
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


        public static TrakHoundAuthorizationResponse Success(
            string requestId,
            string providerId,
            string resourceId,
            ITrakHoundSession session,
            string message = null
            )
        {
            var action = new TrakHoundIdentitySuccessAction();
            action.Message = message;
            return new TrakHoundAuthorizationResponse(requestId, providerId, resourceId, action, session);
        }

        public static TrakHoundAuthorizationResponse Fail(
            string requestId,
            string providerId,
            string resourceId,
            string message = null
            )
        {
            var action = new TrakHoundIdentityFailAction();
            action.Message = message;
            return new TrakHoundAuthorizationResponse(requestId, providerId, resourceId, action, null);
        }

        public static TrakHoundAuthorizationResponse Error(
            string requestId,
            string providerId,
            string resourceId,
            string message = null
            )
        {
            var action = new TrakHoundIdentityErrorAction();
            action.Message = message;
            return new TrakHoundAuthorizationResponse(requestId, providerId, resourceId, action, null);
        }

        public static TrakHoundAuthorizationResponse NotImplemented(
            string requestId,
            string providerId,
            string resourceId,
            string message = null
            )
        {
            var action = new TrakHoundIdentityNotImplementedAction();
            action.Message = message;
            return new TrakHoundAuthorizationResponse(requestId, providerId, resourceId, action, null);
        }
    }
}
