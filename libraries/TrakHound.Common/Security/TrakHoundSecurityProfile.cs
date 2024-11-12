// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Security
{
    public class TrakHoundSecurityProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("providerId")]
        public string ProviderId { get; set; }

        [JsonPropertyName("assignments")]
        public IEnumerable<TrakHoundIdentityAssignment> Assignments { get; set; }


        public static TrakHoundSecurityProfile Read(string configurationPath)
        {
            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = System.IO.File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build();

                        return deserializer.Deserialize<TrakHoundSecurityProfile>(text);
                    }
                }
                catch { }
            }

            return default;
        }

        public async Task Save(string path = null)
        {
            var configurationPath = path;
            if (configurationPath.IsNullOrEmpty()) configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "identity", $"{Id}.config.yaml");

            var identityPath = Path.GetDirectoryName(configurationPath);        

            try
            {
                if (!Directory.Exists(identityPath)) Directory.CreateDirectory(identityPath);

                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(this);
                await System.IO.File.WriteAllTextAsync(configurationPath, yaml);
            }
            catch { }
        }
    }
}
