// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TrakHound.Entities
{
    public struct TrakHoundEntityBlobResponse
    {
        public bool IsValid => !string.IsNullOrEmpty(Collection) && !string.IsNullOrEmpty(Key) && Content != null && Created > 0;

        public string Collection { get; set; }

        public string Key { get; set; }

        public Stream Content { get; set; }

        public long Created { get; set; }

        public IReadOnlyDictionary<string, string> Metadata { get; set; }


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
