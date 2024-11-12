// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Instances
{
    public class TrakHoundInstanceEnvironmentInformation
    {
        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("operatingSystem")]
        public TrakHoundInstanceOperatingSystemInformation OperatingSystem { get; set; }


        public TrakHoundInstanceEnvironmentInformation()
        {
            OperatingSystem = new TrakHoundInstanceOperatingSystemInformation();
        }
    }
}
