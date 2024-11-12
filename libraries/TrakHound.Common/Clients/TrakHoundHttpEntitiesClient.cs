// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Clients
{
    public class TrakHoundEntitiesHttpClient : TrakHoundEntitiesClientBase, ITrakHoundEntitiesClient
    {
        private readonly string _baseUrl;
        private readonly string _routerId;


        public TrakHoundEntitiesHttpClient(TrakHoundHttpClient baseClient)
        {
            base.ApiClient = baseClient.Api;
            base.EntitiesClient = baseClient.System.Entities;
        }
    }
}
