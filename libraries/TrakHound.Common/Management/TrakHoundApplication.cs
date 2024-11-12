// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Management
{
    public class TrakHoundApplication
    {
        [JsonPropertyName("id")]
        public string Id => GenerateId(this);

        [JsonPropertyName("profileId")]
        public string ProfileId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("buildDate")]
        public DateTime BuildDate { get; set; }

        [JsonPropertyName("hash")]
        public string Hash => GenerateHash(this);

        private readonly Dictionary<string, object> _metadata = new Dictionary<string, object>();
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata
        {
            get => _metadata;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        _metadata.Remove(entry.Key);
                        _metadata.Add(entry.Key, entry.Value);
                    }
                }
            }
        }


        public static string GenerateId(TrakHoundApplication application)
        {
            if (application != null)
            {
                return GenerateId(application.ProfileId, application.Version);
            }

            return null;
        }

        public static string GenerateId(string profileId, string version)
        {
            if (!string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(version))
            {
                return $"{profileId}:{version}".ToMD5Hash();
            }

            return null;
        }

        public static string GenerateHash(TrakHoundApplication application)
        {
            if (application != null)
            {
                return $"{application.ProfileId}:{application.Version}:{application.BuildDate.ToUnixTime()}".ToMD5Hash();
            }

            return null;
        }
    }
}
