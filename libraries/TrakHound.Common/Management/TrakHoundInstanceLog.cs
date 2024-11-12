// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Management
{
    public class TrakHoundInstanceLog
    {
        public string Filename { get; set; }

        public long FileSize { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
