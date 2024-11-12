// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Serialization;

namespace TrakHound.Functions
{
    public class FunctionRunModel
    {
        [JsonPropertyName("id")]
        [TrakHoundName]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        [TrakHoundState]
        public string Status { get; set; }

        [JsonPropertyName("statusCode")]
        [TrakHoundNumber]
        public int StatusCode { get; set; }

        [JsonPropertyName("start")]
        [TrakHoundTimestamp]
        public DateTime Start { get; set; }

        [JsonPropertyName("end")]
        [TrakHoundTimestamp]
        public DateTime End { get; set; }

        [JsonPropertyName("duration")]
        [TrakHoundDuration]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("engine")]
        [TrakHoundReference]
        public FunctionEngineModel Engine { get; set; }
    }
}
