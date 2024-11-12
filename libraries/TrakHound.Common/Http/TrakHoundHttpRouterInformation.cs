// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Routing;

namespace TrakHound.Http
{
    public class TrakHoundHttpRouterInformation : ITrakHoundRouterInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isTemporary")]
        public bool IsTemporary { get; set; }
    }
}
