﻿// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseAggregateWindow
    { 
        public string RequestedId { get; set; }

        public double Value { get; set; }

        public long Start { get; set; }

        public long End { get; set; }
    }
}