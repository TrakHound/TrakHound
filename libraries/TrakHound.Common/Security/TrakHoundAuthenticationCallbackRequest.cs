// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    public struct TrakHoundAuthenticationCallbackRequest
    {
        public string Id { get; }

        public ITrakHoundRequestConnection Host { get; }

        public ITrakHoundRequestConnection Client { get; }

        public DateTime Created { get; }

        public ITrakHoundIdentityParameters Parameters { get; }


        public TrakHoundAuthenticationCallbackRequest(
            ITrakHoundRequestConnection host,
            ITrakHoundRequestConnection client,
            ITrakHoundIdentityParameters parameters = null
            )
        {
            Id = Guid.NewGuid().ToString();
            Host = host;
            Client = client;
            Created = DateTime.UtcNow;
            Parameters = parameters;
        }
    }
}
