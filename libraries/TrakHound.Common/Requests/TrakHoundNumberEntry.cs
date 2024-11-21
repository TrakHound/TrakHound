// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundNumberEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Number);
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

        [JsonPropertyName("dataType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundNumberDataType DataType { get; set; }

        [JsonPropertyName("aggregateType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundUpdateType AggregateType { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public TrakHoundNumberEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;
        }

        public TrakHoundNumberEntry(string objectPath, object value, TrakHoundNumberDataType dataType, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = dataType;
            Value = value?.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, byte value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Byte;
            Value = value.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, short value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Int16;
            Value = value.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, int value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Int32;
            Value = value.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, long value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Int64;
            Value = value.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, float value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Float;
            Value = value.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, double value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Double;
            Value = value.ToString();
            AggregateType = aggregateType;
        }

        public TrakHoundNumberEntry(string objectPath, decimal value, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Number;

            ObjectPath = objectPath;
            DataType = TrakHoundNumberDataType.Decimal;
            Value = value.ToString();
            AggregateType = aggregateType;
        }
    }
}
