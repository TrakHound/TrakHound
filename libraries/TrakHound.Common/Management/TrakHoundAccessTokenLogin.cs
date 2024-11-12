// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Management
{
    public class TrakHoundAccessTokenLogin
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("accessToken")]
        public TrakHoundAccessToken AccessToken { get; set; }
    }
}
