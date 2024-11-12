// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public struct TrakHoundAuthenticationCallbackResponse
    {
        public string Id { get; }

        public string AuthenticationRequestId { get; }

        public DateTime Created { get; }

        public ITrakHoundIdentityAction Action { get; }

        public ITrakHoundIdentityParameters Parameters { get; }


        public TrakHoundAuthenticationCallbackResponse(
            string authenticationRequestId,
            ITrakHoundIdentityAction action,
            ITrakHoundIdentityParameters parameters = null
            )
        {
            Id = Guid.NewGuid().ToString();
            AuthenticationRequestId = authenticationRequestId;
            Action = action;
            Created = DateTime.UtcNow;
            Parameters = parameters;
        }


        public static TrakHoundAuthenticationCallbackResponse Success(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null)
        {
            var action = new TrakHoundIdentitySuccessAction();
            return new TrakHoundAuthenticationCallbackResponse(authenticationRequestId, action, parameters);
        }

        public static TrakHoundAuthenticationCallbackResponse Fail(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null, string message = null)
        {
            var action = new TrakHoundIdentityFailAction();
            action.Message = message;
            return new TrakHoundAuthenticationCallbackResponse(authenticationRequestId, action, null);
        }

        public static TrakHoundAuthenticationCallbackResponse Error(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null, string message = null)
        {
            var action = new TrakHoundIdentityErrorAction();
            action.Message = message;
            return new TrakHoundAuthenticationCallbackResponse(authenticationRequestId, action, null);
        }

        public static TrakHoundAuthenticationCallbackResponse NotImplemented(string authenticationRequestId, ITrakHoundIdentityParameters parameters = null, string message = null)
        {
            var action = new TrakHoundIdentityNotImplementedAction();
            action.Message = message;
            return new TrakHoundAuthenticationCallbackResponse(authenticationRequestId, action, null);
        }

        public static TrakHoundAuthenticationCallbackResponse Redirect(string location, string authenticationRequestId, ITrakHoundIdentityParameters parameters = null)
        {
            var action = new TrakHoundIdentityRedirectAction(location);
            return new TrakHoundAuthenticationCallbackResponse(authenticationRequestId, action, null);
        }
    }
}
