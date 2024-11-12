// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectMetadataClient : TrakHoundCollectionEntityClient<ITrakHoundObjectMetadataEntity>, ITrakHoundSystemObjectMetadataClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectMetadataClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            string entityUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            IEnumerable<string> entityUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            string entityUuid,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            IEnumerable<string> entityUuids,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByName(
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            return default;
        }
}
}
