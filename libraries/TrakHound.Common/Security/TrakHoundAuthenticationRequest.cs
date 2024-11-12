// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public sealed class TrakHoundAuthenticationRequest
    {
        public string Id { get; }

        public string ResourceId { get; }

        public ITrakHoundRequestConnection Host { get; }

        public ITrakHoundRequestConnection Client { get; }

        public DateTime Created { get; }

        public ITrakHoundIdentityParameters Parameters { get; }


        public TrakHoundAuthenticationRequest(
            string resourceId,
            ITrakHoundRequestConnection host,
            ITrakHoundRequestConnection client,
            ITrakHoundIdentityParameters parameters = null
            )
        {
            Id = Guid.NewGuid().ToString();
            Host = host;
            Client = client;
            Created = DateTime.UtcNow;
            ResourceId = resourceId;
            Parameters = parameters;
        }
    }
}
