// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseEntityCount
    { 
        public string RequestedId { get; set; }

        public long Count { get; set; }
    }
}
