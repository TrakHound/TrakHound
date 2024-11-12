// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectObservation : IDatabaseEntity<ITrakHoundObjectObservationEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public int DataType { get; set; }

        public string Value { get; set; }

        public ulong BatchId { get; set; }

        public ulong Sequence { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectObservationEntity ToEntity()
        {
            return new TrakHoundObjectObservationEntity
            {
                ObjectUuid = ObjectUuid,
                DataType = DataType,
                Value = Value,
                BatchId = BatchId,
                Sequence = Sequence,
                Timestamp = Timestamp,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}

