// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundMessageQueueEntry
    {
        [JsonPropertyName("objectPath")]
        public string ObjectPath { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }


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
    }
}
