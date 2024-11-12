// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Api;

namespace TrakHound.GitHub.Api
{
    public static class GitHubTokens
    {
        public static string GetToken(ITrakHoundApiConfiguration configuration)
        {
            return configuration.GetParameter("GitHubToken");
        }
    }
}
