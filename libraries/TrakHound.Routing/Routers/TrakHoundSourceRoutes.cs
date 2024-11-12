// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    internal static class TrakHoundSourceRoutes
    {
        // Sources
        public const string SourcesRead = "Sources.Read.Absolute";
        public const string SourcesQuery = "Sources.Read.Query";
        public const string SourcesSubscribe = "Sources.Read.Subscribe";
        public const string SourcesPublish = "Sources.Write.Publish";
        public const string SourcesDelete = "Sources.Write.Delete";
        public const string SourcesExpire = "Sources.Write.Expire";

        // Metadata
        public const string MetadataRead = "Sources.Metadata.Read.Absolute";
        public const string MetadataQuery = "Sources.Metadata.Read.Query";
        public const string MetadataSubscribe = "Sources.Metadata.Read.Subscribe";
        public const string MetadataPublish = "Sources.Metadata.Write.Publish";
        public const string MetadataDelete = "Sources.Metadata.Write.Delete";
        public const string MetadataEmpty = "Sources.Metadata.Write.Empty";
        public const string MetadataExpire = "Sources.Metadata.Write.Expire";



        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            // Sources
            { typeof(IEntityReadDriver<ITrakHoundSourceEntity>), SourcesRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundSourceEntity>), SourcesSubscribe },
            { typeof(ISourceQueryDriver), SourcesQuery },
            { typeof(IEntityPublishDriver<ITrakHoundSourceEntity>), SourcesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundSourceEntity>), SourcesDelete },
            { typeof(IEntityExpirationDriver<ITrakHoundSourceEntity>), SourcesExpire },

            // Metadata
            { typeof(IEntityReadDriver<ITrakHoundSourceMetadataEntity>), MetadataRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundSourceMetadataEntity>), MetadataSubscribe },
            { typeof(ISourceMetadataQueryDriver), MetadataQuery },
            { typeof(IEntityPublishDriver<ITrakHoundSourceMetadataEntity>), MetadataPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundSourceMetadataEntity>), MetadataDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundSourceMetadataEntity>), MetadataEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundSourceMetadataEntity>), MetadataExpire },
        };
    }
}
