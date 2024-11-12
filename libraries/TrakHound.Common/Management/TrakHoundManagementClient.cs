// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Http;

namespace TrakHound.Management
{
    public class TrakHoundManagementClient
    {
        internal const string ApiHeaderKey = "X-TrakHound-Api";

        private readonly string _baseUrl;
        internal string _apiToken;

        private readonly TrakHoundManagementApplicationsClient _applications;
        private readonly TrakHoundManagementDeploymentsClient _deployments;
        private readonly TrakHoundManagementOrganizationsClient _organizations;
        private readonly TrakHoundManagementPackagesClient _packages;
        private readonly TrakHoundManagementUsersClient _users;


        public string BaseUrl => _baseUrl;

        public string Organization { get; set; }

        internal Dictionary<string, string> ApiTokenHeader
        {
            get
            {
                var x = new Dictionary<string, string>();
                x[ApiHeaderKey] = _apiToken;
                return x;
            }
        }

        public TrakHoundManagementApplicationsClient Applications => _applications;

        public TrakHoundManagementDeploymentsClient Deployments => _deployments;

        public TrakHoundManagementOrganizationsClient Organizations => _organizations;

        public TrakHoundManagementPackagesClient Packages => _packages;

        public TrakHoundManagementUsersClient Users => _users;


        public TrakHoundManagementClient(string baseUrl, string organization = null)
        {
            RestRequest.Timeout = 500000;

            _baseUrl = baseUrl;
            Organization = organization;
            _applications = new TrakHoundManagementApplicationsClient(this);
            _deployments = new TrakHoundManagementDeploymentsClient(this);
            _organizations = new TrakHoundManagementOrganizationsClient(this);
            _packages = new TrakHoundManagementPackagesClient(this);
            _users = new TrakHoundManagementUsersClient(this);
        }


        public void SetToken(string token) => _apiToken = token;
    }
}
