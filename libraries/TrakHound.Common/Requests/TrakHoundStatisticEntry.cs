// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundStatisticEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, RangeStart.ToDateTime().ToUnixTime(), RangeEnd.ToDateTime().ToUnixTime());
                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId
        {
            get => ObjectPath;
            set => ObjectPath = value;
        }

        [JsonIgnore]
        public override string AssemblyDefinitionId
        {
            get => ObjectDefinitionId;
            set => ObjectDefinitionId = value;
        }

        [JsonPropertyName("objectPath")]
        public string ObjectPath { get; set; }

        [JsonPropertyName("objectDefinitionId")]
        public string ObjectDefinitionId { get; set; }

        [JsonPropertyName("rangeStart")]
        public string RangeStart { get; set; }

        [JsonPropertyName("rangeEnd")]
        public string RangeEnd { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("dataType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundStatisticDataType DataType { get; set; }

        [JsonPropertyName("aggregateType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundUpdateType AggregateType { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }


        public TrakHoundStatisticEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            Timestamp = DateTime.Now;
        }

        public TrakHoundStatisticEntry(string objectPath, double value, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            Value = value.ToString();

            Timestamp = DateTime.Now;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, double value, DateTime timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            Value = value.ToString();
            Timestamp = timestamp;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, double value, long timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            Value = value.ToString();
            Timestamp = timestamp.ToDateTime();
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, string timeRangeId, double value, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            Value = value.ToString();
            Timestamp = DateTime.Now;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, DateTime from, DateTime to, double value, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = from.ToISO8601String();
            RangeEnd = to.ToISO8601String();
            Value = value.ToString();
            Timestamp = DateTime.Now;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, long from, long to, double value, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = from.ToString();
            RangeEnd = to.ToString();
            Value = value.ToString();
            Timestamp = DateTime.Now;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, string fromExpression, string toExpression, double value, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = fromExpression;
            RangeEnd = toExpression;
            Value = value.ToString();
            Timestamp = DateTime.Now;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, string timeRangeId, double value, DateTime timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            Value = value.ToString();
            Timestamp = timestamp;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, DateTime from, DateTime to, double value, DateTime timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = from.ToISO8601String();
            RangeEnd = to.ToISO8601String();
            Value = value.ToString();
            Timestamp = timestamp;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, long from, long to, double value, DateTime timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = from.ToString();
            RangeEnd = to.ToString();
            Value = value.ToString();
            Timestamp = timestamp;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, string fromExpression, string toExpression, double value, DateTime timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = fromExpression;
            RangeEnd = toExpression;
            Value = value.ToString();
            Timestamp = timestamp;
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, string timeRangeId, double value, long timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            Value = value.ToString();
            Timestamp = timestamp.ToDateTime();
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, DateTime from, DateTime to, double value, long timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = from.ToISO8601String();
            RangeEnd = to.ToISO8601String();
            Value = value.ToString();
            Timestamp = timestamp.ToDateTime();
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, long from, long to, double value, long timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = from.ToString();
            RangeEnd = to.ToString();
            Value = value.ToString();
            Timestamp = timestamp.ToDateTime();
            DataType = dataType;
            AggregateType = aggregateType;
        }

        public TrakHoundStatisticEntry(string objectPath, string fromExpression, string toExpression, double value, long timestamp, TrakHoundStatisticDataType dataType = TrakHoundStatisticDataType.Float, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Statistic;

            ObjectPath = objectPath;
            RangeStart = fromExpression;
            RangeEnd = toExpression;
            Value = value.ToString();
            Timestamp = timestamp.ToDateTime();
            DataType = dataType;
            AggregateType = aggregateType;
        }
    }
}
