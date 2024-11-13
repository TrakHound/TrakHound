// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text;
using TrakHound.Messages;

namespace TrakHound.Http
{
    public static class TrakHoundHttpMessageResponse
    {
        public static byte[] Create(TrakHoundMessageResponse response)
        {
            if (!string.IsNullOrEmpty(response.Topic) && response.Content != null)
            {
                // TOPIC
                // SENDER_ID
                // RETAIN (0 or 1)
                // QOS (0, 1, or 2)
                // TIMESTAMP (UNIX ns)

                var headers = new string[5];
                headers[0] = response.Topic;
                headers[1] = response.SenderId;
                headers[2] = response.Retain ? "1" : "0";
                headers[3] = response.Qos.ToString();
                headers[4] = response.Timestamp.ToString();

                return CreateResponseBytes(headers, response.GetContentBytes());
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

        public static TrakHoundMessageResponse Parse(byte[] responseBytes)
        {
            var response = new TrakHoundMessageResponse();

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
                    if (headers.Length > 4)
                    {
                        response.Topic = headers[0];
                        response.SenderId = headers[1];
                        response.Retain = headers[2] == "1";
                        response.Qos = headers[3].ToByte();
                        response.Timestamp = headers[4].ToLong();
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
