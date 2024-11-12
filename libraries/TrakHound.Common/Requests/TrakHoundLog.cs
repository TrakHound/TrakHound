// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundLog
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("objectUuid")]
        public string ObjectUuid { get; set; }

        [JsonPropertyName("level")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundLogLevel Level { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}
