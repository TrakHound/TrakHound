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
    internal partial class TrakHoundHttpSystemDefinitionClient : TrakHoundHttpEntityClientBase<ITrakHoundDefinitionEntity>, ITrakHoundSystemDefinitionClient
    {
        private readonly TrakHoundHttpSystemDefinitionMetadataClient _metadata;
        private readonly TrakHoundHttpSystemDefinitionDescriptionClient _descriptions;
        private readonly TrakHoundHttpSystemDefinitionWikiClient _wikis;


        public ITrakHoundSystemDefinitionMetadataClient Metadata => _metadata;
        public ITrakHoundSystemDefinitionDescriptionClient Description => _descriptions;
        public ITrakHoundSystemDefinitionWikiClient Wiki => _wikis;


        public TrakHoundHttpSystemDefinitionClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
            _metadata = new TrakHoundHttpSystemDefinitionMetadataClient(baseClient, entitiesClient);
            _descriptions = new TrakHoundHttpSystemDefinitionDescriptionClient(baseClient, entitiesClient);
            _wikis = new TrakHoundHttpSystemDefinitionWikiClient(baseClient, entitiesClient);
        }


    }
}
