// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using TrakHound.MessageQueues;

namespace TrakHound.Http
{
    public class TrakHoundHttpMessageQueueResponse
    {
        [JsonPropertyName("queueId")]
        public string QueueId { get; set; }

        [JsonPropertyName("deliveryId")]
        public string DeliveryId { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }


        public TrakHoundMessageQueueResponse ToResponse()
        {
            var response = new TrakHoundMessageQueueResponse();
            response.DeliveryId = DeliveryId;
            response.Timestamp = Timestamp;

            var contentBytes = StringFunctions.Base64Decode(Content).ToUtf8Bytes();
            if (!contentBytes.IsNullOrEmpty())
            {
                response.Content = new MemoryStream(contentBytes);
            }

            return response;
        }

        public static byte[] Create(TrakHoundMessageQueueResponse response)
        {
            if (!string.IsNullOrEmpty(response.DeliveryId) && response.Content != null)
            {
                return Create(new TrakHoundMessageQueueResponse[] { response });
            }

            return null;
        }

        public static byte[] Create(IEnumerable<TrakHoundMessageQueueResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var httpResponses = new List<TrakHoundHttpMessageQueueResponse>(responses.Count());
                foreach (var response in responses)
                {
                    if (!string.IsNullOrEmpty(response.DeliveryId) && response.Content != null)
                    {
                        var httpResponse = new TrakHoundHttpMessageQueueResponse();
                        httpResponse.DeliveryId = response.DeliveryId;
                        httpResponse.Timestamp = response.Timestamp;
                        httpResponse.Content = StringFunctions.Base64Encode(response.GetContentBytes());
                        httpResponses.Add(httpResponse);
                    }
                }
                return httpResponses.ToJson().ToUtf8Bytes();
            }

            return null;
        }

        private static byte[] CreateResponseBytes(string[] headers, byte[] content)
        {
            var headerDividerBytes = new byte[] { 10, 13 };

            var builder = new StringBuilder();
            for (var i = 0; i < headers.Length; i++)
            {
                builder.Append(headers[i]);
                builder.Append("\r");
            }
            var headerBytes = builder.ToString().ToUtf8Bytes();

            var responseBytes = new byte[headerBytes.Length + headerDividerBytes.Length + content.Length];
            Array.Copy(headerBytes, 0, responseBytes, 0, headerBytes.Length);
            Array.Copy(headerDividerBytes, 0, responseBytes, headerBytes.Length, headerDividerBytes.Length);
            Array.Copy(content, 0, responseBytes, headerBytes.Length + headerDividerBytes.Length, content.Length);
            return responseBytes;
        }

        private static byte[] CreateResponseBytes(IEnumerable<byte[]> contents)
        {
            if (!contents.IsNullOrEmpty())
            {
                var i = 0;
                var responseBytes = new byte[contents.Sum(o => o.Length)];
                foreach (var content in contents)
                {
                    Array.Copy(content, 0, responseBytes, i, content.Length);
                    i += content.Length;
                }
                return responseBytes;
            }

            return null;
        }

        public static IEnumerable<TrakHoundMessageQueueResponse> Parse(byte[] responseBytes)
        {
            if (!responseBytes.IsNullOrEmpty())
            {
                var json = StringFunctions.GetUtf8String(responseBytes);
                var httpResponses = Json.Convert<IEnumerable<TrakHoundHttpMessageQueueResponse>>(json);
                if (!httpResponses.IsNullOrEmpty())
                {
                    var responses = new List<TrakHoundMessageQueueResponse>();
                    foreach (var httpResponse in httpResponses)
                    {
                        responses.Add(httpResponse.ToResponse());
                    }     
                    return responses;
                }
            }

            return null;
        }
    }
}
