// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text;

namespace TrakHound.Entities
{
    public struct TrakHoundEntityMessageQueueResponse
    {
        public bool IsValid => !string.IsNullOrEmpty(Queue) && !string.IsNullOrEmpty(DeliveryId) && Content != null && Timestamp > 0;

        public string Queue { get; set; }

        public string DeliveryId { get; set; }

        public Stream Content { get; set; }

        public long Timestamp { get; set; }


        public byte[] GetContentBytes() => GetContentBytes(Content);

        public string GetContentUtf8String() => GetContentUtf8String(Content);


        public static byte[] GetContentBytes(Stream contentStream)
        {
            if (contentStream != null)
            {
                try
                {
                    using (var readStream = new MemoryStream())
                    {
                        contentStream.CopyTo(readStream);
                        readStream.Seek(0, SeekOrigin.Begin);
                        return readStream.ToArray();
                    }
                }
                catch { }
            }

            return null;
        }

        public static string GetContentUtf8String(Stream contentStream)
        {
            var contentBytes = GetContentBytes(contentStream);
            if (contentBytes != null)
            {
                try
                {
                    return Encoding.UTF8.GetString(contentBytes);
                }
                catch { }
            }

            return null;
        }
    }
}
