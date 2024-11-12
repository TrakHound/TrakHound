// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace TrakHound.Security
{
    public class TrakHoundIdentityProviderConfiguration : ITrakHoundIdentityProviderConfiguration
    {
        public const string ConfigurationCategory = "identity";


        /// <summary>
        /// A unique Identifier for the Api
        /// </summary>
        public string Id { get; set; }

        public string Description { get; set; }

        public string VolumeId { get; set; }

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        [YamlIgnore]
        public string Category => ConfigurationCategory;

        [YamlIgnore]
        public string Path { get; set; }

        [YamlIgnore]
        public string Hash => GenerateHash();


        private string GenerateHash()
        {
            return $"{Id}:{PackageId}:{PackageVersion}".ToSHA256Hash();
        }


        public bool ParameterExists(string name)
        {
            return !Parameters.IsNullOrEmpty() && Parameters.ContainsKey(name);
        }

        public string GetParameter(string name)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                Parameters.TryGetValue(name, out var obj);
                if (obj != null) return obj.ToString();
            }

            return null;
        }

        public T GetParameter<T>(string name)
        {
            if (!Parameters.IsNullOrEmpty() && !string.IsNullOrEmpty(name))
            {
                Parameters.TryGetValue(name, out var obj);
                if (obj != null)
                {
                    var x = obj;
                    if (obj.GetType() == typeof(JsonElement) && ((JsonElement)x).ValueKind == JsonValueKind.Number) x = x.ToInt();


                    try
                    {
                        return (T)Convert.ChangeType(obj, typeof(T));
                    }
                    catch { }
                }
            }

            return default(T);
        }

        public void SetParameter(string name, object value)
        {
            if (!string.IsNullOrEmpty(name) && value != null)
            {
                if (Parameters == null) Parameters = new Dictionary<string, object>();
                Parameters.Remove(name);
                Parameters.Add(name, value.ToString());
            }
        }
    }
}
