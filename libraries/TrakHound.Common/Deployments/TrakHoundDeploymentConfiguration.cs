// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Deployments
{
    public class TrakHoundDeploymentConfiguration
    {
        private const string BackupDirectoryName = "backup";
        public const string Filename = "deployment.config.yaml";


        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("profileId")]
        public string ProfileId { get; set; }


        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        [JsonIgnore]
        [YamlIgnore]
        public string Path { get; set; }


        public static TrakHoundDeploymentConfiguration Read(string path = null)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
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

                        var configuration = deserializer.Deserialize<TrakHoundDeploymentConfiguration>(text);
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
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
            if (path != null) configurationPath = path;

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

            try
            {
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
