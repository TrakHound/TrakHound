// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing.Routers
{
    public class DefinitionsRouter
    {
        private readonly TrakHoundDefinitionRouter _definitions;
        private readonly TrakHoundDefinitionMetadataRouter _metadata;
        private readonly TrakHoundDefinitionDescriptionRouter _descriptions;
        private readonly TrakHoundDefinitionWikiRouter _wikis;



        public TrakHoundDefinitionRouter Definitions => _definitions;
        public TrakHoundDefinitionMetadataRouter Metadata => _metadata;
        public TrakHoundDefinitionDescriptionRouter Descriptions => _descriptions;
        public TrakHoundDefinitionWikiRouter Wikis => _wikis;


        public DefinitionsRouter(TrakHoundRouter router)
        {
            _definitions = new TrakHoundDefinitionRouter(router);
            _metadata = new TrakHoundDefinitionMetadataRouter(router);
            _descriptions = new TrakHoundDefinitionDescriptionRouter(router);
            _wikis = new TrakHoundDefinitionWikiRouter(router);
        }
    }
}
