// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace TrakHound.Functions
{
    public class TrakHoundFunctionConfiguration : ITrakHoundFunctionConfiguration
    {
        public const string ConfigurationCategory = "function";


        public string Id { get; set; }

        public string Description { get; set; }

        public string FunctionId { get; set; }

        public string VolumeId { get; set; }

        public string RouterId { get; set; }

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        [YamlIgnore]
        public string Category => ConfigurationCategory;

        [YamlIgnore]
        public string Path { get; set; }

        [YamlIgnore]
        public string Hash => GenerateHash();


        public TrakHoundFunctionConfiguration()
        {
            Parameters = new Dictionary<string, object>();
        }

        public TrakHoundFunctionConfiguration(ITrakHoundFunctionConfiguration configuration)
        {
            Parameters = new Dictionary<string, object>();

            if (configuration != null)
            {
                Id = configuration.Id;
                Path = configuration.Path;
                Description = configuration.Description;
                FunctionId = configuration.FunctionId;
                VolumeId = configuration.VolumeId;
                RouterId = configuration.RouterId;
                PackageId = configuration.PackageId;
                PackageVersion = configuration.PackageVersion;
                Parameters = configuration.Parameters;
            }
        }


        private string GenerateHash()
        {
            return $"{Id}:{Description}:{FunctionId}:{VolumeId}:{RouterId}:{PackageId}:{PackageVersion}:{Parameters?.ToJson()}".ToSHA256Hash();
        }


        public string GetParameter(string name)
        {
            Parameters.TryGetValue(name, out var obj);
            if (obj != null) return obj.ToString();
            return null;
        }

        public T GetParameter<T>(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Parameters.TryGetValue(name, out var obj);
                if (obj != null)
                {
                    var x = obj;
                    if (obj.GetType() == typeof(JsonElement) && ((JsonElement)x).ValueKind == JsonValueKind.Number) x = x.ToInt();


                    try
                    {
                        if (typeof(T).IsAssignableFrom(obj.GetType()))
                        {
                            return (T)obj;
                        }
                        else
                        {
                            return (T)Convert.ChangeType(obj, typeof(T));
                        }
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
                Parameters.Remove(name);
                Parameters.Add(name, value);
            }
        }
    }
}
