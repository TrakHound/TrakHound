// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundAssignment
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("assignee")]
        public string Assignee { get; set; }

        [JsonPropertyName("assigneeUuid")]
        public string AssigneeUuid { get; set; }

        [JsonPropertyName("member")]
        public string Member { get; set; }

        [JsonPropertyName("memberUuid")]
        public string MemberUuid { get; set; }

        [JsonPropertyName("addTimestamp")]
        public DateTime AddTimestamp { get; set; }

        [JsonPropertyName("removeTimestamp")]
        public DateTime? RemoveTimestamp { get; set; }

        [JsonIgnore]
        public TimeSpan Duration => GetDuration(this);

        [JsonPropertyName("addSourceUuid")]
        public string AddSourceUuid { get; set; }

        [JsonPropertyName("removeSourceUuid")]
        public string RemoveSourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }


        public static TimeSpan GetDuration(TrakHoundAssignment assignment)
        {
            if (assignment != null)
            {
                var start = assignment.AddTimestamp;
                var end = assignment.RemoveTimestamp.HasValue ? assignment.RemoveTimestamp.Value : DateTime.Now;

                return end - start;
            }

            return TimeSpan.Zero;
        }
    }
}
