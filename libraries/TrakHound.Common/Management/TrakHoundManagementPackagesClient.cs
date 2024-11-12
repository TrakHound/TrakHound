// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Http;
using TrakHound.Packages;

namespace TrakHound.Management
{
    public class TrakHoundManagementPackagesClient
    {
        private readonly TrakHoundManagementClient _managementClient;


        public TrakHoundManagementPackagesClient(TrakHoundManagementClient managementClient)
        {
            _managementClient = managementClient;
        }


        public async Task<TrakHoundPackage> Get(string packageUuid)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageUuid))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageUuid);

                var response = await RestRequest.Get<TrakHoundPackageJson>(url, _managementClient.ApiTokenHeader);
                return response?.ToPackage();
            }

            return null;
        }

        public async Task<TrakHoundPackage> Get(string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageId);
                url = Url.Combine(url, packageVersion);

                var response = await RestRequest.Get<TrakHoundPackageJson>(url, _managementClient.ApiTokenHeader);
                return response?.ToPackage();
            }

            return null;
        }

        public async Task<TrakHoundPackage> GetLatest(string packageId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageId);
                url = Url.Combine(url, "latest");

                var response = await RestRequest.Get<TrakHoundPackageJson>(url, _managementClient.ApiTokenHeader);
                return response?.ToPackage();
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetVersions(string packageId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageId);
                url = Url.Combine(url, "versions");

                return await RestRequest.Get<IEnumerable<string>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> Query(string category, string query = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(category))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "category");
                url = Url.Combine(url, category);
                url = Url.AddQueryParameter(url, "q", query);

                var responses = await RestRequest.Get<IEnumerable<TrakHoundPackageJson>>(url, _managementClient.ApiTokenHeader);
                return responses?.Select(o => o.ToPackage());
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> Query(IEnumerable<TrakHoundPackageQuery> queries)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !queries.IsNullOrEmpty())
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);

                var responses = await RestRequest.Post<IEnumerable<TrakHoundPackageJson>>(url, queries, headers: _managementClient.ApiTokenHeader);
                return responses?.Select(o => o.ToPackage());
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> QueryLatestByOrganization()
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "latest");

                var responses = await RestRequest.Get<IEnumerable<TrakHoundPackageJson>>(url, _managementClient.ApiTokenHeader);
                return responses?.Select(o => o.ToPackage());
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> QueryLatestByOrganization(string organization)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, "latest");

                var responses = await RestRequest.Get<IEnumerable<TrakHoundPackageJson>>(url, _managementClient.ApiTokenHeader);
                return responses?.Select(o => o.ToPackage());
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundPackage>> QueryUpdates(IEnumerable<TrakHoundPackageQuery> queries)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !queries.IsNullOrEmpty())
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "updates");

                var responses = await RestRequest.Post<IEnumerable<TrakHoundPackageJson>>(url, queries, headers: _managementClient.ApiTokenHeader);
                return responses?.Select(o => o.ToPackage());
            }

            return null;
        }

        public async Task<byte[]> Download(string packageUuid)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageUuid))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageUuid);
                url = Url.Combine(url, "download");

                try
                {
                    var stream = await RestRequest.GetStream(url, headers: _managementClient.ApiTokenHeader);
                    if (stream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            stream.Close();
                            return memoryStream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public async Task<byte[]> Download(string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageId);
                url = Url.Combine(url, packageVersion);
                url = Url.Combine(url, "download");

                try
                {
                    var stream = await RestRequest.GetStream(url, headers: _managementClient.ApiTokenHeader);
                    if (stream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            stream.Close();
                            return memoryStream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public async Task<byte[]> DownloadLatest(string packageId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(packageId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, packageId);
                url = Url.Combine(url, "latest");
                url = Url.Combine(url, "download");

                try
                {
                    var stream = await RestRequest.GetStream(url, headers: _managementClient.ApiTokenHeader);
                    if (stream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            stream.Close();
                            return memoryStream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }


        public async Task<byte[]> DownloadArchive(IEnumerable<TrakHoundPackageQuery> queries)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !queries.IsNullOrEmpty())
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "download");

                var response = await RestRequest.PostResponse(url, queries, headers: _managementClient.ApiTokenHeader);
                if (response.StatusCode == 200)
                {
                    return response.Content;
                }
            }

            return null;
        }


        public async Task<TrakHoundPackage> Upload(byte[] packageBytes)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !packageBytes.IsNullOrEmpty())
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "publish");

                var packageJson = await RestRequest.Post<TrakHoundPackageJson>(url, packageBytes, headers: _managementClient.ApiTokenHeader);
                return packageJson?.ToPackage();
            }

            return null;
        }

        public async Task<TrakHoundPackage> Upload(Stream packageStream)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && packageStream != null)
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "publish");

                var packageJson = await RestRequest.Post<TrakHoundPackageJson>(url, packageStream, headers: _managementClient.ApiTokenHeader);
                return packageJson?.ToPackage();
            }

            return null;
        }


        public async Task<bool> DeletePackage(string organization, string packageId)
        {
            if (!string.IsNullOrEmpty(organization) && !string.IsNullOrEmpty(packageId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, packageId);
                url = Url.Combine(url, "delete");

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }

        public async Task<bool> DeletePackageVersion(string organization, string packageUuid)
        {
            if (!string.IsNullOrEmpty(organization) && !string.IsNullOrEmpty(packageUuid))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "packages");
                url = Url.Combine(url, organization);
                url = Url.AddQueryParameter(url, "packageUuid", packageUuid);
                url = Url.Combine(url, "delete");

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }
    }
}
