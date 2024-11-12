// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionDefinitionMetadataClient : TrakHoundCollectionEntityClient<ITrakHoundDefinitionMetadataEntity>, ITrakHoundSystemDefinitionMetadataClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionDefinitionMetadataClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundDefinitionMetadataEntity>> QueryByDefinitionUuid(
            string definitionUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionMetadataEntity>> QueryByDefinitionUuid(
            IEnumerable<string> definitionUuids,
            string routerId = null)
        {
            return default;
        }
}
}
