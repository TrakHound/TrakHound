// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Packages
{
    public class TrakHoundPackagePublishProfile
    {
        public const string Filename = "trakhound.package.publish.json";


        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("destinations")]
        public IEnumerable<TrakHoundPackagePublishDestination> Destinations { get; set; }


        public TrakHoundPackagePublishProfile()
        {
            Version = "0.0.0";
        }
    }
}
