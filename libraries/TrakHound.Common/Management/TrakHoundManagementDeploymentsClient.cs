// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Deployments;
using TrakHound.Http;

namespace TrakHound.Management
{
    public class TrakHoundManagementDeploymentsClient
    {
        private readonly TrakHoundManagementClient _managementClient;


        public TrakHoundManagementDeploymentsClient(TrakHoundManagementClient managementClient)
        {
            _managementClient = managementClient;
        }


        public async Task<IEnumerable<TrakHoundDeploymentProfile>> GetProfiles()
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);

                return await RestRequest.Get<IEnumerable<TrakHoundDeploymentProfile>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundDeploymentProfile> GetProfile(string profileId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);

                return await RestRequest.Get<TrakHoundDeploymentProfile>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundDeployment>> GetDeployments(string profileId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.Combine(url, "deployments");

                return await RestRequest.Get<IEnumerable<TrakHoundDeployment>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundDeployment> GetDeployment(string deploymentId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(deploymentId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.AddQueryParameter(url, "deploymentId", deploymentId);

                return await RestRequest.Get<TrakHoundDeployment>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundDeployment> GetDeployment(string profileId, string version = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.AddQueryParameter(url, "version", version);

                return await RestRequest.Get<TrakHoundDeployment>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<byte[]> DownloadDeployment(string deploymentId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(deploymentId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, deploymentId);
                url = Url.Combine(url, "download");

                return await RestRequest.GetBytes(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<byte[]> DownloadDeploymentByProfile(string profileId, string version = null)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.Combine(url, "download");
                url = Url.AddQueryParameter(url, "version", version);

                return await RestRequest.GetBytes(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> AddProfile(TrakHoundDeploymentProfile profile)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && profile != null)
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, _managementClient.Organization);

                var response = await RestRequest.PostResponse(url, profile, headers: _managementClient.ApiTokenHeader);
                return response.StatusCode == 200;
            }

            return false;
        }

        public async Task<TrakHoundDeployment> UploadDeployment(byte[] deploymentBytes)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !deploymentBytes.IsNullOrEmpty())
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, "upload");

                return await RestRequest.Post<TrakHoundDeployment>(url, deploymentBytes, headers: _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundDeployment> SetActiveDeployment(string profileId, string deploymentId)
        {
            if (!string.IsNullOrEmpty(_managementClient.Organization) && !string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(deploymentId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, _managementClient.Organization);
                url = Url.Combine(url, profileId);
                url = Url.Combine(url, "active");
                url = Url.AddQueryParameter(url, "deploymentId", deploymentId);

                return await RestRequest.Put<TrakHoundDeployment>(url, headers: _managementClient.ApiTokenHeader);
            }

            return null;
        }


        public async Task<bool> DeleteProfile(string organization, string profileId)
        {
            if (!string.IsNullOrEmpty(organization) && !string.IsNullOrEmpty(profileId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, "profiles");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, profileId);

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }

        public async Task<bool> DeleteDeployment(string organization, string deploymentId)
        {
            if (!string.IsNullOrEmpty(organization) && !string.IsNullOrEmpty(deploymentId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "deployments");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, deploymentId);

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }
    }
}
