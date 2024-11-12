// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrakHound.Packages
{
    public class TrakHoundPackageClient
    {
        private const int defaultTimeout = 5000;


        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(defaultTimeout)
        };


        private readonly IEnumerable<string> _packageServers;


        public TrakHoundPackageClient(string packageServer)
        {
            _packageServers = new List<string> { packageServer };
        }

        public TrakHoundPackageClient(IEnumerable<string> packageServers)
        {
            _packageServers = packageServers;
        }


        public async Task<TrakHoundPackage> Get(string packageUuid)
        {
            if (_packageServers != null && !string.IsNullOrEmpty(packageUuid))
            {
                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        var url = Url.Combine(packageServer, "package");

                        // Add Package ID
                        url = Url.Combine(url, packageUuid);

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        var json = await response.Content.ReadAsStringAsync();
                        var jsonPackage = JsonSerializer.Deserialize<TrakHoundPackageJson>(json);
                        return jsonPackage.ToPackage();
                    }
                    catch { }
                }
            }

            return null;
        }

        public async Task<TrakHoundPackage> Get(string packageId, string packageVersion = null)
        {
            if (_packageServers != null && !string.IsNullOrEmpty(packageId))
            {
                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        var url = Url.Combine(packageServer, "package");

                        // Add Package ID
                        url = Url.AddQueryParameter(url, "id", packageId);

                        // Add Version
                        url = Url.AddQueryParameter(url, "version", packageVersion);

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        var json = await response.Content.ReadAsStringAsync();
                        var jsonPackage = JsonSerializer.Deserialize<TrakHoundPackageJson>(json);
                        return jsonPackage.ToPackage();
                    }
                    catch { }
                }
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetVersions(string packageId)
        {
            if (_packageServers != null && !string.IsNullOrEmpty(packageId))
            {
                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        var url = Url.Combine(packageServer, "package");
                        url = Url.Combine(url, packageId);
                        url = Url.Combine(url, "versions");

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        var json = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<IEnumerable<string>>(json);
                    }
                    catch { }
                }
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> Query(string category, string query)
        {
            if (_packageServers != null && !string.IsNullOrEmpty(category))
            {
                var packages = new List<TrakHoundPackage>();

                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        var url = Url.Combine(packageServer, "packages");
                        url = Url.Combine(url, category);
                        url = Url.AddQueryParameter(url, "q", query);

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        var json = await response.Content.ReadAsStringAsync();
                        var jsonPackages = JsonSerializer.Deserialize<IEnumerable<TrakHoundPackageJson>>(json);
                        return jsonPackages?.Select(o => o.ToPackage());
                    }
                    catch { }
                }

                return packages;
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> CheckForUpdates(IEnumerable<TrakHoundPackageQuery> queries)
        {
            if (_packageServers != null && !queries.IsNullOrEmpty())
            {
                var packages = new List<TrakHoundPackage>();

                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        var queriesJson = Json.Convert(queries);

                        var url = Url.Combine(packageServer, "packages");
                        url = Url.Combine(url, "updates");
                        url = Url.AddQueryParameter(url, "queries", queriesJson);

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        var json = await response.Content.ReadAsStringAsync();
                        var jsonPackages = JsonSerializer.Deserialize<IEnumerable<TrakHoundPackageJson>>(json);
                        return jsonPackages?.Select(o => o.ToPackage());
                    }
                    catch { }
                }

                return packages;
            }

            return null;
        }

        public async Task<byte[]> Download(string packageId, string packageVersion = null)
        {
            if (_packageServers != null && !string.IsNullOrEmpty(packageId))
            {
                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        var url = Url.Combine(packageServer, "package");
                        url = Url.Combine(url, packageId);
                        url = Url.Combine(url, "download");

                        // Add Version
                        url = Url.AddQueryParameter(url, "version", packageVersion);

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        using (var memoryStream = new MemoryStream())
                        {
                            var responseStream = response.Content.ReadAsStream();
                            responseStream.CopyTo(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                    catch { }
                }              
            }

            return null;
        }

        public async Task<bool> Upload(byte[] packageBytes)
        {
            if (_packageServers != null && packageBytes != null && packageBytes.Length > 0)
            {
                foreach (var packageServer in _packageServers)
                {
                    try
                    {
                        using var content = new ByteArrayContent(packageBytes);

                        var response = await _httpClient.PostAsync(packageServer, content);
                        response.EnsureSuccessStatusCode();
                    }
                    catch 
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
