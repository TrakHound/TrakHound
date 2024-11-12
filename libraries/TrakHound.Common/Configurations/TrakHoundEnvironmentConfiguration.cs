// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Configurations
{
    public class TrakHoundEnvironmentConfiguration
    {
        public const string Filename = "trakhound-environment.config.yaml";
        private static TrakHoundEnvironmentConfiguration _configuration;


        public TrakHoundEnvironmentInstanceConfiguration Instance { get; set; }


        public static TrakHoundEnvironmentConfiguration Get()
        {
            if (_configuration == null) _configuration = Read();
            if (_configuration == null)
            {
                var configuration = new TrakHoundEnvironmentConfiguration();
                configuration.Instance = TrakHoundEnvironmentInstanceConfiguration.GetDefault();
                Save(configuration);
                _configuration = configuration;
            }
            return _configuration;
        }

        private static TrakHoundEnvironmentConfiguration Read()
        {
            var configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
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

                        return deserializer.Deserialize<TrakHoundEnvironmentConfiguration>(text);
                    }
                }
                catch { }
            }

            return null;
        }

        public static void Save(TrakHoundEnvironmentConfiguration configuration, string configurationPath = null)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
            if (!string.IsNullOrEmpty(configurationPath)) path = configurationPath;

            if (configuration != null && !string.IsNullOrEmpty(path))
            {
                try
                {
                    var serializer = new SerializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();
                    var yaml = serializer.Serialize(configuration);
                    File.WriteAllText(path, yaml);
                }
                catch { }
            }
        }
    }
}
