// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Functions
{
    class FunctionStatus
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundFunctionStatusType Status { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
