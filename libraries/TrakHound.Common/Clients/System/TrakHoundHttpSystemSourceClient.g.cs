// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemSourceClient : TrakHoundHttpEntityClientBase<ITrakHoundSourceEntity>, ITrakHoundSystemSourceClient
    {
        private readonly TrakHoundHttpSystemSourceMetadataClient _metadata;


        public ITrakHoundSystemSourceMetadataClient Metadata => _metadata;


        public TrakHoundHttpSystemSourceClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
            _metadata = new TrakHoundHttpSystemSourceMetadataClient(baseClient, entitiesClient);
        }


    }
}
