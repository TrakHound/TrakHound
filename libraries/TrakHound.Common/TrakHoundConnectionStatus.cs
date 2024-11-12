// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundConnectionStatus
    {
        public bool IsConnected { get; set; }

        public string Message { get; set; }

        public long Timestamp { get; set; }
    }
}
