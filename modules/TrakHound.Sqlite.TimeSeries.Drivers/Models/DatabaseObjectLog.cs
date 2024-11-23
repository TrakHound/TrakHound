// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectLog : IDatabaseEntity<ITrakHoundObjectLogEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public int LogLevel { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectLogEntity ToEntity()
        {
            return new TrakHoundObjectLogEntity
            {
                ObjectUuid = ObjectUuid,
                LogLevel = LogLevel,
                Code = Code,
                Message = Message,
                Timestamp = Timestamp,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}

