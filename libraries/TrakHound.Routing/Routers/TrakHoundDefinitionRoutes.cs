// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public static class TrakHoundDefinitionRoutes
    {
        // Definitions
        public const string DefinitionsRead = "Definitions.Instances.Read.Absolute";
        public const string DefinitionsQuery = "Definitions.Instances.Read.Query";
        public const string DefinitionsSubscribe = "Definitions.Instances.Read.Subscribe";
        public const string DefinitionsPublish = "Definitions.Instances.Write.Publish";
        public const string DefinitionsDelete = "Definitions.Instances.Write.Delete";
        public const string DefinitionsExpire = "Definitions.Instances.Write.Expire";

        // Metadata
        public const string MetadataRead = "Definitions.Metadata.Read.Absolute";
        public const string MetadataQuery = "Definitions.Metadata.Read.Query";
        public const string MetadataSubscribe = "Definitions.Metadata.Read.Subscribe";
        public const string MetadataPublish = "Definitions.Metadata.Write.Publish";
        public const string MetadataDelete = "Definitions.Metadata.Write.Delete";
        public const string MetadataEmpty = "Definitions.Metadata.Write.Empty";
        public const string MetadataExpire = "Definitions.Metadata.Write.Expire";

        // Descriptions
        public const string DescriptionRead = "Definitions.Descriptions.Read.Absolute";
        public const string DescriptionQuery = "Definitions.Descriptions.Read.Query";
        public const string DescriptionSubscribe = "Definitions.Descriptions.Read.Subscribe";
        public const string DescriptionPublish = "Definitions.Descriptions.Write.Publish";
        public const string DescriptionDelete = "Definitions.Descriptions.Write.Delete";
        public const string DescriptionEmpty = "Definitions.Descriptions.Write.Empty";
        public const string DescriptionExpire = "Definitions.Descriptions.Write.Expire";

        // Wikis
        public const string WikisRead = "Definitions.Wikis.Read.Absolute";
        public const string WikisQuery = "Definitions.Wikis.Read.Query";
        public const string WikisSubscribe = "Definitions.Wikis.Read.Subscribe";
        public const string WikisPublish = "Definitions.Wikis.Write.Publish";
        public const string WikisDelete = "Definitions.Wikis.Write.Delete";
        public const string WikisEmpty = "Definitions.Wikis.Write.Empty";
        public const string WikisExpire = "Definitions.Wikis.Write.Expire";


        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            // Definitions
            { typeof(IEntityReadDriver<ITrakHoundDefinitionEntity>), DefinitionsRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundDefinitionEntity>), DefinitionsSubscribe },
            { typeof(IDefinitionQueryDriver), DefinitionsQuery },
            { typeof(IEntityPublishDriver<ITrakHoundDefinitionEntity>), DefinitionsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundDefinitionEntity>), DefinitionsDelete },
            { typeof(IEntityExpirationDriver<ITrakHoundDefinitionEntity>), DefinitionsExpire },

            // Metadata
            { typeof(IEntityReadDriver<ITrakHoundDefinitionMetadataEntity>), MetadataRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundDefinitionMetadataEntity>), MetadataSubscribe },
            { typeof(IDefinitionMetadataQueryDriver), MetadataQuery },
            { typeof(IEntityPublishDriver<ITrakHoundDefinitionMetadataEntity>), MetadataPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundDefinitionMetadataEntity>), MetadataDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundDefinitionMetadataEntity>), MetadataEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundDefinitionMetadataEntity>), MetadataExpire },

            // Descriptions
            { typeof(IEntityReadDriver<ITrakHoundDefinitionDescriptionEntity>), DescriptionRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundDefinitionDescriptionEntity>), DescriptionSubscribe },
            { typeof(IDefinitionDescriptionQueryDriver), DescriptionQuery },
            { typeof(IEntityPublishDriver<ITrakHoundDefinitionDescriptionEntity>), DescriptionPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundDefinitionDescriptionEntity>), DescriptionDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundDefinitionDescriptionEntity>), DescriptionEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundDefinitionDescriptionEntity>), DescriptionExpire },
       
            // Wiki
            { typeof(IEntityReadDriver<ITrakHoundDefinitionWikiEntity>), WikisRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundDefinitionWikiEntity>), WikisSubscribe },
            { typeof(IDefinitionWikiQueryDriver), WikisQuery },
            { typeof(IEntityPublishDriver<ITrakHoundDefinitionWikiEntity>), WikisPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundDefinitionWikiEntity>), WikisDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundDefinitionWikiEntity>), WikisEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundDefinitionWikiEntity>), WikisExpire },
        };
    }
}
