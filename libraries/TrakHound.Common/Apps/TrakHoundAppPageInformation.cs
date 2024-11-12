// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Apps
{
    public class TrakHoundAppPageInformation
    {
        [JsonIgnore]
        public TrakHoundAppInformation App { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameters")]
        public IEnumerable<TrakHoundAppPageParameterInformation> Parameters { get; set; }
    }
}
