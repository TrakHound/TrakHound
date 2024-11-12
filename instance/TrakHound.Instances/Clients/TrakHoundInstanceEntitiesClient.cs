// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Clients
{
    public partial class TrakHoundInstanceEntitiesClient : TrakHoundEntitiesClientBase, ITrakHoundEntitiesClient
    {
        public TrakHoundInstanceEntitiesClient(TrakHoundInstanceClient baseClient)
        {
            base.EntitiesClient = baseClient.System.Entities;
            base.ApiClient = baseClient.Api;
        }
    }
}
