// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Buffers
{
    public class TrakHoundBufferQueueMetrics
    {
        [JsonPropertyName("isActive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool IsActive { get; set; }

        [JsonPropertyName("itemCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int ItemCount { get; set; }

        [JsonPropertyName("itemLimit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int ItemLimit { get; set; }

        [JsonPropertyName("totalItemCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long TotalItemCount { get; set; }

        [JsonPropertyName("itemRate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public double ItemRate { get; set; }

        [JsonPropertyName("lastUpdated")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long LastUpdated { get; set; }
    }
}
