// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public class TrakHoundEntityTableResponse
    {
        public string TableUuid { get; set; }

        public object[] Rows { get; set; }
    }
}
