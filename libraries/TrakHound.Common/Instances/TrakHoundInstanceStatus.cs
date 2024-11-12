// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Instances
{
    public enum TrakHoundInstanceStatus
    {
        Error = -2,
        Stopping = -1,
        Stopped = 0,
        Starting = 1,
        Started = 2
    }
}
