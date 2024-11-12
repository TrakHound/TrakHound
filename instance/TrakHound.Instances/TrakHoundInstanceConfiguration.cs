// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Instances
{
    public class TrakHoundInstanceConfiguration
    {
        private const string BackupDirectoryName = "backup";
        public const string Filename = "instance.config.yaml";


        [JsonPropertyName("instanceId")]
        public string InstanceId { get; set; }

        [JsonPropertyName("instanceKey")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public string InstanceKey { get; set; }

        [JsonPropertyName("name")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public string Description { get; set; }


        [JsonPropertyName("httpAddress")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public string HttpAddress { get; set; }

        [JsonPropertyName("httpPort")]
        public int HttpPort { get; set; }


        [JsonPropertyName("adminInterfaceEnabled")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public bool AdminInterfaceEnabled { get; set; }

        [JsonPropertyName("adminAuthenticationEnabled")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public bool AdminAuthenticationEnabled { get; set; }

        [JsonPropertyName("systemApiEnabled")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public bool SystemApiEnabled { get; set; }


        [JsonPropertyName("deploymentProfileId")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public string DeploymentProfileId { get; set; }

        [JsonPropertyName("managementServer")]
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public string ManagementServer { get; set; }


        [JsonPropertyName("basePath")]
        public string BasePath { get; set; }


        [JsonIgnore]
        [YamlIgnore]
        public string Path { get; set; }

        [JsonIgnore]
        [YamlIgnore]
        public string ChangeToken { get; set; }


        public TrakHoundInstanceConfiguration()
        {
            InstanceId = $"i-{StringFunctions.RandomString(10)}";
            HttpPort = 8472;
            AdminInterfaceEnabled = true;
            AdminAuthenticationEnabled = true;
            SystemApiEnabled = true;
        }


        public static TrakHoundInstanceConfiguration Read(string path = null)
        {
            var configurationPath = path;
            if (string.IsNullOrEmpty(configurationPath)) configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

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

                        var configuration = deserializer.Deserialize<TrakHoundInstanceConfiguration>(text);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }

        public void Save(string path = null, bool createBackup = true)
        {
            try
            {
                var configurationPath = path;
                if (configurationPath.IsNullOrEmpty()) System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

                var configurationDirectory = System.IO.Path.GetDirectoryName(configurationPath);
                if (!Directory.Exists(configurationDirectory)) Directory.CreateDirectory(configurationDirectory);

                if (createBackup)
                {
                    // Create Backup of Configuration File
                    var backupDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BackupDirectoryName);
                    if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
                    var backupFilename = System.IO.Path.ChangeExtension(UnixDateTime.Now.ToString(), ".backup.yaml");
                    var backupPath = System.IO.Path.Combine(backupDir, backupFilename);
                    if (File.Exists(configurationPath))
                    {
                        File.Copy(configurationPath, backupPath);
                    }
                }

                // Update ChangeToken
                ChangeToken = Guid.NewGuid().ToString();


                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(this);
                File.WriteAllText(configurationPath, yaml);
            }
            catch { }
        }
    }
}
