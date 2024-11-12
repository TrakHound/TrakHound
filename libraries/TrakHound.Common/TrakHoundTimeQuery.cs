// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundTimeQuery
    {
        public string Target { get; set; }

        public long Timestamp { get; set; }


        public TrakHoundTimeQuery(string target, long timestamp)
        {
            Target = target;
            Timestamp = timestamp;
        }
    }
}
