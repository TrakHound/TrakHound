// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace TrakHound.Instances
{
    public class TrakHoundInstanceInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundInstanceType Type { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundInstanceStatus Status { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("environment")]
        public TrakHoundInstanceEnvironmentInformation Environment { get; set; }

        [JsonPropertyName("deployment")]
        public TrakHoundInstanceDeploymentInformation Deployment { get; set; }

        [JsonPropertyName("interfaces")]
        public IEnumerable<TrakHoundInstanceInterfaceInformation> Interfaces { get; set; }


        public TrakHoundInstanceInformation()
        {
            Environment = new TrakHoundInstanceEnvironmentInformation();
        }


        public TrakHoundInstanceInterfaceInformation GetInterface(string interfaceType)
        {
            if (!string.IsNullOrEmpty(interfaceType) && !Interfaces.IsNullOrEmpty())
            {
                return Interfaces.FirstOrDefault(o => o.Type == interfaceType);
            }

            return null;
        }
    }
}
