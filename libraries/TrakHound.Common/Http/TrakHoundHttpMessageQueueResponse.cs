// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using TrakHound.MessageQueues;

namespace TrakHound.Http
{
    public static class TrakHoundHttpMessageQueueResponse
    {
        public static byte[] Create(TrakHoundMessageQueueResponse response)
        {
            if (!string.IsNullOrEmpty(response.DeliveryId) && response.Content != null)
            {
                var headerBytes = response.DeliveryId.ToUtf8Bytes();
                var dividerBytes = new byte[] { 10, 13 };
                var contentBytes = response.GetContentBytes();

                var responseBytes = new byte[headerBytes.Length + dividerBytes.Length + contentBytes.Length];
                Array.Copy(headerBytes, 0, responseBytes, 0, headerBytes.Length);
                Array.Copy(dividerBytes, 0, responseBytes, headerBytes.Length, dividerBytes.Length);
                Array.Copy(contentBytes, 0, responseBytes, headerBytes.Length + dividerBytes.Length, contentBytes.Length);
                return responseBytes;
            }

            return null;
        }

        public static TrakHoundMessageQueueResponse Parse(byte[] responseBytes)
        {
            var response = new TrakHoundMessageQueueResponse();

            if (responseBytes != null)
            {
                var deliveryIdIndex = 0;

                var i = 0;
                while (i < responseBytes.Length)
                {
                    if (responseBytes[i] == 10 && responseBytes[i + 1] == 13)
                    {
                        deliveryIdIndex = i;
                        break;
                    }

                    i++;
                }

                if (deliveryIdIndex > 0)
                {
                    var deliveryIdBytes = new byte[deliveryIdIndex];
                    Array.Copy(responseBytes, 0, deliveryIdBytes, 0, deliveryIdIndex);
                    var deliveryId = StringFunctions.GetUtf8String(deliveryIdBytes);
                    response.DeliveryId = deliveryId;

                    var contentLength = responseBytes.Length - deliveryIdIndex - 2; // 2 = 10 & 13 bytes for CR+LF
                    var contentBytes = new byte[contentLength];
                    Array.Copy(responseBytes, deliveryIdIndex + 2, contentBytes, 0, contentLength);
                    response.Content = new MemoryStream(contentBytes);
                }
            }

            return response;
        }
    }
}
