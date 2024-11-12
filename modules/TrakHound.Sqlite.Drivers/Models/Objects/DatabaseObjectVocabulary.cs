// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectVocabulary : IDatabaseEntity<ITrakHoundObjectVocabularyEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public string DefinitionUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectVocabularyEntity ToEntity()
        {
            return new TrakHoundObjectVocabularyEntity
            {
                ObjectUuid = ObjectUuid,
                DefinitionUuid = DefinitionUuid,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
