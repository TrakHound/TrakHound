// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundIndexEntry
    {
        [JsonPropertyName("index")]
        public string Index { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("dataType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public EntityIndexDataType DataType { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }


        public TrakHoundIndexEntry() { }

        public TrakHoundIndexEntry(string index, string name, string value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.String;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, bool value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Boolean;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, byte value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Byte;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, short value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Int16;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, int value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Int32;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, long value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Int64;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, float value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Float;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, double value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Duration;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, decimal value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Decimal;
            Value = value;
        }

        public TrakHoundIndexEntry(string index, string name, DateTime value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Timestamp;
            Value = value.ToUnixTime();
        }

        public TrakHoundIndexEntry(string index, string name, TimeSpan value)
        {
            Index = index;
            Name = name;
            DataType = EntityIndexDataType.Timestamp;
            Value = value.TotalNanoseconds;
        }
    }
}
