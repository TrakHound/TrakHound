// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundObjectRequest
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("definitionId")]
        public string DefinitionId { get; set; }

        [JsonPropertyName("definitionVersion")]
        public string DefinitionVersion { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }


        public TrakHoundObjectRequest()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}
