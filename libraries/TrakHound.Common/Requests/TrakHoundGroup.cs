// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundGroup
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("groupUuid")]
        public string GroupUuid { get; set; }

        [JsonPropertyName("member")]
        public string Member { get; set; }

        [JsonPropertyName("memberUuid")]
        public string MemberUuid { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}
