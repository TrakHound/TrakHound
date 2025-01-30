// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;
using TrakHound.MessageQueues;
using TrakHound.Messages;

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
        public string Content { get; set; }

        [JsonPropertyName("deliveryId")]
        public string DeliveryId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }


        public Stream GetContentStream()
        {
            var bytes = GetContentBytes();
            if (!bytes.IsNullOrEmpty())
            {
                try
                {
                    return new MemoryStream(bytes);
                }
                catch { }
            }

            return null;
        }

        public byte[] GetContentBytes() => StringFunctions.ToUtf8Bytes(StringFunctions.Base64Decode(Content));

        public string GetContentUtf8String() => StringFunctions.Base64Decode(Content);

        //public byte[] GetContentBytes() => TrakHoundMessageQueueResponse.GetContentBytes(Content);

        //public string GetContentUtf8String() => TrakHoundMessageQueueResponse.GetContentUtf8String(Content);
    }
}
