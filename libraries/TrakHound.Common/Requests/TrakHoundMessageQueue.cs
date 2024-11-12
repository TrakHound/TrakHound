// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundMessageQueue
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("objectUuid")]
        public string ObjectUuid { get; set; }

        [JsonPropertyName("queueId")]
        public string QueueId { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("content")]
        public Stream Content { get; set; }

        [JsonPropertyName("deliveryId")]
        public string DeliveryId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
