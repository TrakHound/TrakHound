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

        public long ValueInteger { get; set; }

        public double ValueFloat { get; set; }

        public string ValueText { get; set; }

        public ulong BatchId { get; set; }

        public ulong Sequence { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectObservationEntity ToEntity()
        {
            string value = null;
            switch ((TrakHoundObservationDataType)DataType)
            {
                case TrakHoundObservationDataType.Boolean: value = ValueInteger.ToBoolean().ToString(); break;
                case TrakHoundObservationDataType.Byte: value = ValueInteger.ToByte().ToString(); break;
                case TrakHoundObservationDataType.Int16: value = ValueInteger.ToString(); break;
                case TrakHoundObservationDataType.Int32: value = ValueInteger.ToString(); break;
                case TrakHoundObservationDataType.Int64: value = ValueInteger.ToString(); break;
                case TrakHoundObservationDataType.Float: value = ValueFloat.ToString("N"); break;
                case TrakHoundObservationDataType.Double: value = ValueFloat.ToString("N"); break;
                case TrakHoundObservationDataType.Decimal: value = ValueFloat.ToString("N"); break;
                case TrakHoundObservationDataType.Reference: value = ValueText; break;
                case TrakHoundObservationDataType.String: value = ValueText; break;
                case TrakHoundObservationDataType.Vocabulary: value = ValueText; break;
            }

            return new TrakHoundObjectObservationEntity
            {
                ObjectUuid = ObjectUuid,
                DataType = DataType,
                Value = value,
                BatchId = BatchId,
                Sequence = Sequence,
                Timestamp = Timestamp,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}

