// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundSource
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<TrakHoundSourceMetadata> Metadata { get; set; }
    }
}
