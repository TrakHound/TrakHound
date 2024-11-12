// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionSourceMetadataClient : TrakHoundCollectionEntityClient<ITrakHoundSourceMetadataEntity>, ITrakHoundSystemSourceMetadataClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionSourceMetadataClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            string sourceUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            IEnumerable<string> sourceUuids,
            string routerId = null)
        {
            return default;
        }
}
}
