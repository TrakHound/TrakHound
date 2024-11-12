// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionDefinitionClient : TrakHoundCollectionEntityClient<ITrakHoundDefinitionEntity>, ITrakHoundSystemDefinitionClient
    {
        private readonly TrakHoundEntityCollection _collection;

        private readonly TrakHoundCollectionDefinitionMetadataClient _metadata;
        private readonly TrakHoundCollectionDefinitionDescriptionClient _descriptions;
        private readonly TrakHoundCollectionDefinitionWikiClient _wikis;



        public ITrakHoundSystemDefinitionMetadataClient Metadata => _metadata;
        public ITrakHoundSystemDefinitionDescriptionClient Description => _descriptions;
        public ITrakHoundSystemDefinitionWikiClient Wiki => _wikis;


        public TrakHoundCollectionDefinitionClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


            _metadata = new TrakHoundCollectionDefinitionMetadataClient(collection);
            _descriptions = new TrakHoundCollectionDefinitionDescriptionClient(collection);
            _wikis = new TrakHoundCollectionDefinitionWikiClient(collection);
        }

}
}
