// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseDefinitionDescription : IDatabaseEntity<ITrakHoundDefinitionDescriptionEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string DefinitionUuid { get; set; }

        public string LanguageCode { get; set; }

        public string Text { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundDefinitionDescriptionEntity ToEntity()
        {
            return new TrakHoundDefinitionDescriptionEntity
            {
                DefinitionUuid = DefinitionUuid,
                LanguageCode = LanguageCode,
                Text = Text,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
