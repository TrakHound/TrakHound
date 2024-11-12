// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Requests
{
    public class TrakHoundTimeRangeObject
    {
        public string Path { get; set; }
        public IEnumerable<TrakHoundTimeRange> TimeRanges { get; set; }
    }
}
