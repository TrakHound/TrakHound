// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundDuration
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("objectUuid")]
        public string ObjectUuid { get; set; }

        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ulong Value { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}
