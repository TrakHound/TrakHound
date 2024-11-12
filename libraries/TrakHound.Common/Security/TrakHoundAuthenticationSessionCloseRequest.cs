// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public struct TrakHoundAuthenticationSessionCloseRequest
    {
        public string Id { get; }

        public string SessionId { get; }

        public ITrakHoundRequestConnection Host { get; }

        public ITrakHoundRequestConnection Client { get; }

        public DateTime Created { get; }

        public ITrakHoundIdentityParameters Parameters { get; }


        public TrakHoundAuthenticationSessionCloseRequest(
            string sessionId,
            ITrakHoundRequestConnection host,
            ITrakHoundRequestConnection client,
            ITrakHoundIdentityParameters parameters = null
            )
        {
            Id = Guid.NewGuid().ToString();
            SessionId = sessionId;
            Host = host;
            Client = client;
            Created = DateTime.UtcNow;
            Parameters = parameters;
        }
    }
}
