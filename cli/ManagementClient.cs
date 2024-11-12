// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Management;

namespace TrakHound.CLI
{
    internal static class ManagementClient
    {
        private const string _defaultServer = "https://www.trakhound.com/management";
        private const string _defaultOrganization = "Public";

        private static TrakHoundManagementClient _managementClient;

        public static async Task<TrakHoundManagementClient> Get(string server = _defaultServer, string organization = _defaultOrganization)
        {
            if (_managementClient == null)
            {
                var configuration = TrakHoundCliConfiguration.Instance;

                var baseUrl = !string.IsNullOrEmpty(server) ? server : configuration.ManagementServer;
                var targetOrganization = !string.IsNullOrEmpty(organization) ? organization : configuration.Organization;
                var managementClient = new TrakHoundManagementClient(baseUrl, targetOrganization);

                if (!string.IsNullOrEmpty(configuration.AccessTokenName) && !string.IsNullOrEmpty(configuration.AccessTokenCode))
                {
                    //var login = await managementClient.Authentication.LoginAccessToken(configuration.AccessTokenName, configuration.AccessTokenCode);
                    //if (login != null && !string.IsNullOrEmpty(login.Token))
                    //{
                    //    _managementClient = managementClient;
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Login Error : Access Token Invalid");

                    //    return null;
                    //}
                }
                else
                {
                    _managementClient = managementClient;
                }
            }

            return _managementClient;
        }
    }
}
