// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public struct TrakHoundAuthenticationSessionCloseResponse
    {
        public string Id { get; }

        public string SessionId { get; }

        public DateTime Created { get; }

        public ITrakHoundIdentityAction Action { get; }

        public ITrakHoundIdentityParameters Parameters { get; }


        public TrakHoundAuthenticationSessionCloseResponse(
            string sessionId,
            ITrakHoundIdentityAction action,
            ITrakHoundIdentityParameters parameters = null
            )
        {
            Id = Guid.NewGuid().ToString();
            SessionId = sessionId;
            Created = DateTime.UtcNow;
            Action = action;
            Parameters = parameters;
        }


        public static TrakHoundAuthenticationSessionCloseResponse Success(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null)
        {
            var action = new TrakHoundIdentitySuccessAction();
            return new TrakHoundAuthenticationSessionCloseResponse(authenticationRequestId, action, parameters);
        }

        public static TrakHoundAuthenticationSessionCloseResponse Fail(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null, string message = null)
        {
            var action = new TrakHoundIdentityFailAction();
            action.Message = message;
            return new TrakHoundAuthenticationSessionCloseResponse(authenticationRequestId, action, null);
        }

        public static TrakHoundAuthenticationSessionCloseResponse Error(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null, string message = null)
        {
            var action = new TrakHoundIdentityErrorAction();
            action.Message = message;
            return new TrakHoundAuthenticationSessionCloseResponse(authenticationRequestId, action, null);
        }

        public static TrakHoundAuthenticationSessionCloseResponse NotImplemented(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null, string message = null)
        {
            var action = new TrakHoundIdentityNotImplementedAction();
            action.Message = message;
            return new TrakHoundAuthenticationSessionCloseResponse(authenticationRequestId, action, null);
        }

        public static TrakHoundAuthenticationSessionCloseResponse Redirect(string location, string authenticationRequestId, ITrakHoundIdentityParameters parameters = null)
        {
            var action = new TrakHoundIdentityRedirectAction(location);
            return new TrakHoundAuthenticationSessionCloseResponse(authenticationRequestId, action, null);
        }
    }
}
