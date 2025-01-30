// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TrakHound.MessageQueues;

namespace TrakHound.Http
{
    public static class TrakHoundHttpMessageQueueResponse
    {
        private const string _headerStart = "#_TRAKHOUND_MESSAGE_BEGIN_#";


        public static byte[] Create(TrakHoundMessageQueueResponse response)
        {
            if (!string.IsNullOrEmpty(response.DeliveryId) && response.Content != null)
            {
                // DELIVERY_ID
                // TIMESTAMP (UNIX ns)

                var headers = new string[3];
                headers[0] = _headerStart;
                headers[1] = response.DeliveryId;
                headers[2] = response.Timestamp.ToString();

                return CreateResponseBytes(headers, response.GetContentBytes());
            }

            return null;
        }

        public static byte[] Create(IEnumerable<TrakHoundMessageQueueResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var responseBytes = new List<byte[]>();
                foreach (var response in responses)
                {
                    if (!string.IsNullOrEmpty(response.DeliveryId) && response.Content != null)
                    {
                        // DELIVERY_ID
                        // TIMESTAMP (UNIX ns)

                        var headers = new string[3];
                        headers[0] = _headerStart;
                        headers[1] = response.DeliveryId;
                        headers[2] = response.Timestamp.ToString();

                        responseBytes.Add(CreateResponseBytes(headers, response.GetContentBytes()));
                    }
                }
                return CreateResponseBytes(responseBytes);
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

        public static TrakHoundMessageQueueResponse Parse(byte[] responseBytes)
        {
            var response = new TrakHoundMessageQueueResponse();

            if (responseBytes != null)
            {
                var headerIndex = 0;

                var i = 0;
                while (i < responseBytes.Length)
                {
                    if (responseBytes[i] == 10 && responseBytes[i + 1] == 13)
                    {
                        headerIndex = i;
                        break;
                    }

                    i++;
                }

                if (headerIndex > 0)
                {
                    var headerBytes = new byte[headerIndex];
                    Array.Copy(responseBytes, 0, headerBytes, 0, headerIndex);
                    var header = StringFunctions.GetUtf8String(headerBytes);

                    var headers = header.Split("\r");
                    if (headers.Length > 2)
                    {
                        response.DeliveryId = headers[1];
                        response.Timestamp = headers[2].ToLong();
                    }

                    var contentLength = responseBytes.Length - headerIndex - 2; // 2 = 10 & 13 bytes for CR+LF
                    var contentBytes = new byte[contentLength];
                    Array.Copy(responseBytes, headerIndex + 2, contentBytes, 0, contentLength);
                    response.Content = new MemoryStream(contentBytes);
                }
            }

            return response;
        }
    }
}
