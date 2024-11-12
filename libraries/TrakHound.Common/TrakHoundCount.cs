// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public class TrakHoundCount
    { 
        public string Target { get; set; }

        public long Count { get; set; }


        public TrakHoundCount(string target, long count)
        {
            Target = target;
            Count = count;
        }
    }
}
