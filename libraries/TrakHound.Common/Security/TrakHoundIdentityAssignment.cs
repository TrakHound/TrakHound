// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Security
{
    public class TrakHoundIdentityAssignment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("resourceId")]
        public string ResourceId { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<string> Roles { get; set; }

        [JsonPropertyName("permissions")]
        public IEnumerable<string> Permissions { get; set; }
    }
}
