// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Configurations;
using YamlDotNet.Serialization;

namespace TrakHound.Blazor.Diagrams
{
    public class TrakHoundDiagramConfiguration : ITrakHoundConfiguration
    {
        public const string ConfigurationCategory = "diagram";


        public string Id { get; set; }

        public IEnumerable<TrakHoundDiagramNodeConfiguration> Nodes { get; set; }


        [YamlIgnore]
        public string Category => ConfigurationCategory;

        [YamlIgnore]
        public string Path { get; set; }

        [YamlIgnore]
        public string Hash => GenerateHash();

        private string GenerateHash()
        {
            return UnixDateTime.Now.ToString().ToMD5Hash();
        }
    }
}
