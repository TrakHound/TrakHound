// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing.Routers
{
    public class SourcesRouter
    {
        private readonly TrakHoundSourceRouter _sources;
        private readonly TrakHoundSourceMetadataRouter _metadata;



        public TrakHoundSourceRouter Sources => _sources;
        public TrakHoundSourceMetadataRouter Metadata => _metadata;


        public SourcesRouter(TrakHoundRouter router)
        {
            _sources = new TrakHoundSourceRouter(router);
            _metadata = new TrakHoundSourceMetadataRouter(router);
        }
    }
}
