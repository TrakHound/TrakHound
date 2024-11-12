// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Services;

namespace TrakHound.Http
{
    public class TrakHoundHttpServiceStatus
    {
        [JsonPropertyName("serviceId")]
        public string ServiceId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }


        public TrakHoundHttpServiceStatus() { }

        public TrakHoundHttpServiceStatus(TrakHoundServiceStatus status)
        {
            ServiceId = status.ServiceId;
            Status = status.Status.ToString();
            Message = status.Message;
        }

        public TrakHoundServiceStatus ToStatus()
        {
            var status = new TrakHoundServiceStatus();
            status.ServiceId = ServiceId;
            status.Status = Status.ConvertEnum<TrakHoundServiceStatusType>();
            status.Message = Message;
            return status;
        }


        public static IEnumerable<TrakHoundHttpServiceStatus> Create(IEnumerable<TrakHoundServiceStatus> statuses)
        {
            var httpStatuses = new List<TrakHoundHttpServiceStatus>();
            if (!statuses.IsNullOrEmpty())
            {
                foreach (var status in statuses)
                {
                    httpStatuses.Add(new TrakHoundHttpServiceStatus(status));
                }
            }
            return httpStatuses;
        }

        public static IEnumerable<TrakHoundServiceStatus> ToStatuses(IEnumerable<TrakHoundHttpServiceStatus> httpStatuses)
        {
            var statuses = new List<TrakHoundServiceStatus>();
            if (!httpStatuses.IsNullOrEmpty())
            {
                foreach (var httpStatus in httpStatuses)
                {
                    statuses.Add(httpStatus.ToStatus());
                }
            }
            return statuses;
        }
    }
}
