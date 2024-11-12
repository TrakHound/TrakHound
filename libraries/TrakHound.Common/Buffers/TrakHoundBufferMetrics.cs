// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Buffers
{
    public class TrakHoundBufferMetrics
    {
        [JsonPropertyName("bufferId")]
        public string BufferId { get; set; }

        [JsonPropertyName("driverId")]
        public string DriverId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("operationType")]
        public string OperationType { get; set; }

        [JsonPropertyName("startTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long StartTime { get; set; }

        [JsonPropertyName("stopTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long StopTime { get; set; }

        [JsonPropertyName("lastUpdated")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long LastUpdated { get; set; }

        [JsonPropertyName("queue")]
        public TrakHoundBufferQueueMetrics Queue { get; set; }

        [JsonPropertyName("fileBuffer")]
        public TrakHoundFileBufferMetrics FileBuffer { get; set; }


        public TrakHoundBufferMetrics()
        {
            Queue = new TrakHoundBufferQueueMetrics();
            FileBuffer = new TrakHoundFileBufferMetrics();
        }
    }
}
