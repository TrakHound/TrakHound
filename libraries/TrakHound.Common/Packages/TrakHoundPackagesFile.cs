// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrakHound.Packages
{
    public class TrakHoundPackagesFile
    {
        public const string Filename = "trakhound-packages.json";


        [JsonPropertyName("autoUpdates")]
        public IEnumerable<TrakHoundPackageUpdateConfiguration> AutoUpdates { get; set; }

        [JsonPropertyName("packages")]
        public IEnumerable<TrakHoundPackagesFileItem> Packages { get; set; }


        public static TrakHoundPackagesFile Read(string path = null)
        {
            string filePath;
            if (!string.IsNullOrEmpty(path)) filePath = Path.Combine(path, Filename);
            else filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<TrakHoundPackagesFile>(json);
                }
                catch { }
            }

            return null;
        }

        public static async Task<TrakHoundPackagesFile> ReadAsync(string path = null)
        {
            string filePath;
            if (!string.IsNullOrEmpty(path)) filePath = Path.Combine(path, Filename);
            else filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

            if (File.Exists(filePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<TrakHoundPackagesFile>(json);
                }
                catch { }
            }

            return null;
        }


        public static bool Write(IEnumerable<TrakHoundPackagesFileItem> packages, IEnumerable<TrakHoundPackageUpdateConfiguration> autoUpdates = null, string path = null)
        {
            if (!packages.IsNullOrEmpty())
            {
                string filePath;
                if (!string.IsNullOrEmpty(path)) filePath = Path.Combine(path, Filename);
                else filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

                var file = new TrakHoundPackagesFile();
                file.Packages = packages;
                file.AutoUpdates = autoUpdates;

                Json.Write(file, filePath);
                return true;
            }

            return false;
        }

        public static async Task<bool> WriteAsync(IEnumerable<TrakHoundPackagesFileItem> packages, IEnumerable<TrakHoundPackageUpdateConfiguration> autoUpdates = null, string path = null)
        {
            if (!packages.IsNullOrEmpty())
            {
                string filePath;
                if (!string.IsNullOrEmpty(path)) filePath = Path.Combine(path, Filename);
                else filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);

                var file = new TrakHoundPackagesFile();
                file.Packages = packages;
                file.AutoUpdates = autoUpdates;

                await Json.WriteAsync(file, filePath, default);
                return true;
            }

            return false;
        }
    }
}
