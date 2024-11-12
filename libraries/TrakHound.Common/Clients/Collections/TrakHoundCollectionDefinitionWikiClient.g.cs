// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionDefinitionWikiClient : TrakHoundCollectionEntityClient<ITrakHoundDefinitionWikiEntity>, ITrakHoundSystemDefinitionWikiClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionDefinitionWikiClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundDefinitionWikiEntity>> QueryByDefinitionUuid(
            string definitionUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionWikiEntity>> QueryByDefinitionUuid(
            IEnumerable<string> definitionUuids,
            string routerId = null)
        {
            return default;
        }
}
}
