// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text;

namespace TrakHound.Messages
{
    public struct TrakHoundMessageResponse
    {
        public bool IsValid => !string.IsNullOrEmpty(BrokerId) && !string.IsNullOrEmpty(Topic) && Content != null && Timestamp > 0;

        public string BrokerId { get; set; }

        public string Topic { get; set; }

        public Stream Content { get; set; }

        public bool Retain { get; set; }

        public byte Qos { get; set; }

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
