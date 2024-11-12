// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Http;

namespace TrakHound.Management
{
    public class TrakHoundManagementUsersClient
    {
        private readonly TrakHoundManagementClient _managementClient;


        public TrakHoundManagementUsersClient(TrakHoundManagementClient managementClient)
        {
            _managementClient = managementClient;
        }


        public async Task<TrakHoundUserProfile> GetUser(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "users");
                url = Url.Combine(url, username);

                return await RestRequest.Get<TrakHoundUserProfile>(url, _managementClient.ApiTokenHeader);
            }

            return null;
        }

        public async Task<bool> AddUser(TrakHoundCreateUserProfile profile)
        {
            if (profile != null && !string.IsNullOrEmpty(profile.Username))
            {
                var url = Url.Combine(_managementClient.BaseUrl, "users");

                await RestRequest.Post(url, profile, headers: _managementClient.ApiTokenHeader);

                return true;
            }

            return false;
        }
    }
}
