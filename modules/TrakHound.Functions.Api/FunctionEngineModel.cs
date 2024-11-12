// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Serialization;

namespace TrakHound.Functions
{
    public class FunctionEngineModel
    {
        [JsonPropertyName("id")]
        [TrakHoundName]
        public string Id { get; set; }

        [JsonPropertyName("packageId")]
        [TrakHoundString]
        public string PackageId { get; set; }

        [JsonPropertyName("packageVersion")]
        [TrakHoundString]
        public string PackageVersion { get; set; }

        [JsonPropertyName("sender")]
        [TrakHoundString]
        public string Sender { get; set; }
    }
}
