// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;
using TrakHound.Messages;

namespace TrakHound.Requests
{
    public class TrakHoundMessage
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("objectUuid")]
        public string ObjectUuid { get; set; }

        [JsonPropertyName("brokerId")]
        public string BrokerId { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("retain")]
        public bool Retain { get; set; }

        [JsonPropertyName("qos")]
        public int QoS { get; set; }

        [JsonPropertyName("content")]
        public Stream Content { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }


        public byte[] GetContentBytes() => TrakHoundMessageResponse.GetContentBytes(Content);

        public string GetContentUtf8String() => TrakHoundMessageResponse.GetContentUtf8String(Content);
    }
}
