// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectBoolean : IDatabaseEntity<ITrakHoundObjectBooleanEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public bool Value { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectBooleanEntity ToEntity()
        {
            return new TrakHoundObjectBooleanEntity
            {
                ObjectUuid = ObjectUuid,
                Value = Value,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
