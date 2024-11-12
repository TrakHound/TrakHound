// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Functions
{
    public class FunctionTask
	{
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; }

        [JsonPropertyName("functionId")]
        public string FunctionId { get; set; }

        [JsonPropertyName("schedule")]
		public string Schedule { get; set; }

		[JsonPropertyName("parameters")]
		public Dictionary<string, string> Parameters { get; set; }
	}
}
