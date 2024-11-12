// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Services
{
    public struct TrakHoundServiceStatus
    {
        public string ServiceId { get; set; } 

        public TrakHoundServiceStatusType Status { get; set; } 

        public string Message { get; set; }


        public TrakHoundServiceStatus(string serviceId, TrakHoundServiceStatusType status, string message = null)
        {
            ServiceId = serviceId;
            Status = status;
            Message = message;
        }
    }
}
