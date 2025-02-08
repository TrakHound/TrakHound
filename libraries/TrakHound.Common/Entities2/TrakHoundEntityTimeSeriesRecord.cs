// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public class TrakHoundEntityTimeSeriesRecord
    {
        public long Timestamp { get; set; }

        public IReadOnlyDictionary<string, object> Fields { get; set; }
    }
}
