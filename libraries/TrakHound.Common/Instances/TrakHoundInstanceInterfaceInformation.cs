// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Instances
{
    public class TrakHoundInstanceInterfaceInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }


        public TrakHoundInstanceInterfaceInformation()
        {
            Id = Guid.NewGuid().ToString();
            Port = 8472;
        }


        public string GetBaseUrl()
        {
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Address))
            {
                switch (Type.ToLower())
                {
                    case "http": return $"http://{Address}:{Port}";
                }
            }

            return null;
        }
    }
}
