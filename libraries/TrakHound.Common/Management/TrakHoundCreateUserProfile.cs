// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Management
{
    public class TrakHoundCreateUserProfile : TrakHoundUserProfile
    {
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
