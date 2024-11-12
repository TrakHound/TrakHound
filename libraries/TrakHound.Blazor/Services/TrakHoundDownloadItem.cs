// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.Services
{
    public class TrakHoundDownloadItem
    {
        public string Key { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
