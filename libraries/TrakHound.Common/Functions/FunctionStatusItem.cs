// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Functions
{
    public struct TrakHoundStatusItem
    {
        public TrakHoundFunctionStatusType StatusType { get; set; }

        public long Timestamp { get; set; }


        public TrakHoundStatusItem(TrakHoundFunctionStatusType statusType, long timestamp)
        {
            StatusType = statusType;
            Timestamp = timestamp;
        }
    }
}
