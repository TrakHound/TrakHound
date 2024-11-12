// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Http;

namespace TrakHound.Management
{
    public class TrakHoundManagementOrganizationsClient
    {
        private readonly TrakHoundManagementClient _managementClient;


        public TrakHoundManagementOrganizationsClient(TrakHoundManagementClient managementClient)
        {
            _managementClient = managementClient;
        }


        #region "Organizations"

        public async Task<IEnumerable<TrakHoundOrganizationProfile>> GetOrganizations()
        {
            var url = Url.Combine(_managementClient.BaseUrl, "organizations");

            return await RestRequest.Get<IEnumerable<TrakHoundOrganizationProfile>>(url, _managementClient.ApiTokenHeader);
        }

        public async Task<TrakHoundOrganizationProfile> GetOrganization(string organization)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);

                return await RestRequest.Get<TrakHoundOrganizationProfile>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> AddOrganization(string organization, bool isPublic = false)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);
                url = Url.AddQueryParameter(url, "isPublic", isPublic);

                await RestRequest.Put(url, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }

        public async Task<IEnumerable<string>> GetOrganizationUsers(string organization)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, "users");

                return await RestRequest.Get<IEnumerable<string>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> AddOrganizationUser(string organization, string username)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, "users");
                url = Url.AddQueryParameter(url, "username", username);

                await RestRequest.Put(url, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteOrganization(string organization)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }

        public async Task<bool> RemoveOrganizationUser(string organization, string username)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, "users");
                url = Url.AddQueryParameter(url, "username", username);

                await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }

        #endregion

        #region "Groups"

        public async Task<IEnumerable<TrakHoundOrganizationGroup>> GetOrganizationGroups(string organization)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, "groups");

                return await RestRequest.Get<IEnumerable<TrakHoundOrganizationGroup>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<TrakHoundOrganizationGroup> GetOrganizationGroup(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, "groups");
                url = Url.Combine(url, groupId);

                return await RestRequest.Get<TrakHoundOrganizationGroup>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetOrganizationGroupUsers(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, "groups");
                url = Url.Combine(url, groupId);
                url = Url.Combine(url, "users");

                return await RestRequest.Get<IEnumerable<string>>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> AddOrganizationGroup(string organization, string group, string type = null)
        {
            if (!string.IsNullOrEmpty(organization))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, organization);
                url = Url.Combine(url, "groups");
                url = Url.AddQueryParameter(url, "group", group);
                url = Url.AddQueryParameter(url, "type", type);

                await RestRequest.Put(url, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }

        public async Task<bool> AddOrganizationGroupUser(string groupId, string username)
        {
            if (!string.IsNullOrEmpty(groupId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, "groups");
                url = Url.Combine(url, groupId);
                url = Url.Combine(url, "users");
                url = Url.AddQueryParameter(url, "username", username);

                await RestRequest.Put(url, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteOrganizationGroup(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, "groups");
                url = Url.Combine(url, groupId);

                return await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);
            }

            return false;
        }

        public async Task<bool> RemoveOrganizationGroupUser(string groupId, string username)
        {
            if (!string.IsNullOrEmpty(groupId))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "organizations");
                url = Url.Combine(url, "groups");
                url = Url.Combine(url, groupId);
                url = Url.Combine(url, "users");
                url = Url.AddQueryParameter(url, "username", username);

                await RestRequest.Delete(url, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }

        #endregion

    }
}
