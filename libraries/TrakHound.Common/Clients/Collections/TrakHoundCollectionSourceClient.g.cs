// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionSourceClient : TrakHoundCollectionEntityClient<ITrakHoundSourceEntity>, ITrakHoundSystemSourceClient
    {
        private readonly TrakHoundEntityCollection _collection;

        private readonly TrakHoundCollectionSourceMetadataClient _metadata;



        public ITrakHoundSystemSourceMetadataClient Metadata => _metadata;


        public TrakHoundCollectionSourceClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


            _metadata = new TrakHoundCollectionSourceMetadataClient(collection);
        }

}
}
