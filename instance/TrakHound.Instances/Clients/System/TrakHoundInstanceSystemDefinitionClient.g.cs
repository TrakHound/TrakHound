// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemDefinitionClient : TrakHoundInstanceEntityClient<ITrakHoundDefinitionEntity>, ITrakHoundSystemDefinitionClient
    {
        private readonly TrakHoundInstanceSystemDefinitionMetadataClient _metadata;
        private readonly TrakHoundInstanceSystemDefinitionDescriptionClient _descriptions;
        private readonly TrakHoundInstanceSystemDefinitionWikiClient _wikis;
        public ITrakHoundSystemDefinitionMetadataClient Metadata => _metadata;
        public ITrakHoundSystemDefinitionDescriptionClient Description => _descriptions;
        public ITrakHoundSystemDefinitionWikiClient Wiki => _wikis;


        public TrakHoundInstanceSystemDefinitionClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
            _metadata = new TrakHoundInstanceSystemDefinitionMetadataClient(baseClient);
            _descriptions = new TrakHoundInstanceSystemDefinitionDescriptionClient(baseClient);
            _wikis = new TrakHoundInstanceSystemDefinitionWikiClient(baseClient);
        }


    }
}
