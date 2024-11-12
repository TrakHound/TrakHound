// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Http;

namespace TrakHound.Management
{
    public class TrakHoundManagementApplicationsClient
    {
        private readonly TrakHoundManagementClient _managementClient;


        public TrakHoundManagementApplicationsClient(TrakHoundManagementClient managementClient)
        {
            _managementClient = managementClient;
        }


        public async Task<IEnumerable<TrakHoundApplicationProfile>> GetProfiles()
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);

                return await RestRequest.Get<IEnumerable<TrakHoundApplicationProfile>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundApplicationProfile> GetProfile(string profileId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);

                return await RestRequest.Get<TrakHoundApplicationProfile>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundApplication>> GetApplications(string profileId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.Combine(url, "applications");

                return await RestRequest.Get<IEnumerable<TrakHoundApplication>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundApplication> GetApplication(string applicationId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(applicationId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.AddQueryParameter(url, "applicationId", applicationId);

                return await RestRequest.Get<TrakHoundApplication>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundApplication> GetApplication(string profileId, string version = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.AddQueryParameter(url, "version", version);

                return await RestRequest.Get<TrakHoundApplication>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<byte[]> DownloadApplication(string applicationId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(applicationId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, applicationId);
                url = Url.Combine(url, "download");

                return await RestRequest.GetBytes(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<byte[]> DownloadApplicationByProfile(string profileId, string version = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.Combine(url, "download");
                url = Url.AddQueryParameter(url, "version", version);

                return await RestRequest.GetBytes(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> AddProfile(TrakHoundApplicationProfile profile)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && profile != null)
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);

                var response = await RestRequest.PostResponse(url, profile, headers: _managementClient.ApiTokenHeader);
                return response.StatusCode == 200;
            }

            return false;
        }

        public async Task<TrakHoundApplication> UploadApplication(TrakHoundApplication application)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && application != null)
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "add");

                return await RestRequest.Post<TrakHoundApplication>(url, application, headers: _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundApplication> UploadApplication(string applicationId, byte[] applicationBytes, string filename = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(applicationId) && !applicationBytes.IsNullOrEmpty())
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, applicationId);
                url = Url.Combine(url, "upload");
                url = Url.AddQueryParameter(url, "filename", filename);

                return await RestRequest.Post<TrakHoundApplication>(url, applicationBytes, headers: _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundApplication> UploadApplication(string applicationId, Stream applicationStream, string filename = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(applicationId) && applicationStream != null)
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, applicationId);
                url = Url.Combine(url, "upload");
                url = Url.AddQueryParameter(url, "filename", filename);

                return await RestRequest.Post<TrakHoundApplication>(url, applicationStream, headers: _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundApplication> SetActiveApplication(string profileId, string applicationId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(applicationId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.Combine(url, "active");
                url = Url.AddQueryParameter(url, "applicationId", applicationId);

                return await RestRequest.Put<TrakHoundApplication>(url, headers: _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> DeleteProfile(string organization, string profileId)
        {
            if (!string.IsNullOrEmpty(organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, profileId);

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }

        public async Task<bool> DeleteApplication(string organization, string applicationId)
        {
            if (!string.IsNullOrEmpty(organization) && !string.IsNullOrEmpty(applicationId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "applications");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, applicationId);

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }
    }
}
