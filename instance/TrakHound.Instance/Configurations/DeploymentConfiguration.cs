// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Instance.Configurations
{
    public class DeploymentConfiguration
    {
        public const string Filename = "deployment.config.yaml";


        public string ProfileId { get; set; }

        public int Version { get; set; }

        public long LastUpdated { get; set; }


        public static DeploymentConfiguration Read(string path = null)
        {
            var configurationPath = path;
            if (string.IsNullOrEmpty(configurationPath)) configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build();

                        return deserializer.Deserialize<DeploymentConfiguration>(text);
                    }
                }
                catch { }
            }

            return default;
        }

        public void Save(string path = null)
        {
            Save(this, path);
        }

        public static void Save(DeploymentConfiguration configuration, string path = null)
        {
            var configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", Filename);
            if (path != null) configurationPath = path;

            if (configuration != null && !string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var serializer = new SerializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();
                    var yaml = serializer.Serialize(configuration);
                    File.WriteAllText(configurationPath, yaml);
                }
                catch { }
            }
        }
    }
}
