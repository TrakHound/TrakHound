// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemSourceClient : TrakHoundInstanceEntityClient<ITrakHoundSourceEntity>, ITrakHoundSystemSourceClient
    {
        private readonly TrakHoundInstanceSystemSourceMetadataClient _metadata;
        public ITrakHoundSystemSourceMetadataClient Metadata => _metadata;


        public TrakHoundInstanceSystemSourceClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
            _metadata = new TrakHoundInstanceSystemSourceMetadataClient(baseClient);
        }


    }
}
