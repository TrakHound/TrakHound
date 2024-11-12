// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Instance.Configurations
{
    public class RemotesConfiguration
    {
        public const string Filename = "remotes.config.yaml";


        public IEnumerable<RemoteConfiguration> Remotes { get; set; }


        public static RemotesConfiguration Read(string path = null)
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

                        return deserializer.Deserialize<RemotesConfiguration>(text);
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

        public static void Save(RemotesConfiguration configuration, string path = null)
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

        public static RemotesConfiguration GetDefault()
        {
            var configuration = new RemotesConfiguration();

            var remoteConfiguration = new RemoteConfiguration();
            remoteConfiguration.Id = "public";
            remoteConfiguration.Name = "Public";
            remoteConfiguration.BaseUrl = "https://www.trakhound.com/management";
            remoteConfiguration.Organization = "Public";
            configuration.Remotes = new RemoteConfiguration[] { remoteConfiguration };

            return configuration;
        }
    }
}
