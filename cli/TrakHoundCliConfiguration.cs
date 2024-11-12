// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.CLI
{
    public class TrakHoundCliConfiguration
    {
        public const string Filename = "cli.config.yaml";
        public static TrakHoundCliConfiguration Instance { get; private set; }


        public string ManagementServer { get; set; }

        public string Organization { get; set; }

        public string AccessTokenName { get; set; }

        public string AccessTokenCode { get; set; }


        public static void Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
            if (File.Exists(path))
            {
                Instance = Read();
            }

            if (Instance == null) Instance = new TrakHoundCliConfiguration();
        }

        public static TrakHoundCliConfiguration Read()
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

                        return deserializer.Deserialize<TrakHoundCliConfiguration>(text);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
