// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Octokit;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.GitHub.Api.Controllers
{
    [TrakHoundApiController("releases")]
    public class ReleaseController : TrakHoundApiController
    {
        [TrakHoundApiQuery("{organization}/{repository}")]
        public async Task<TrakHoundApiResponse> Query([FromRoute] string organization, [FromRoute] string repository)
        {
            var token = GitHubTokens.GetToken(Configuration);
            if (token != null)
            {
                var githubClient = new GitHubClient(new ProductHeaderValue("trakhound-github-api"));

                var tokenAuth = new Credentials(token);
                githubClient.Credentials = tokenAuth;

                var releases = await githubClient.Repository.Release.GetAll(organization, repository);
                if (!releases.IsNullOrEmpty())
                {
                    return Ok(releases);
                }
                else
                {
                    return NotFound($"No Releases found for '{organization}/{repository}'");
                }
            }
            else
            {
                return NotAuthorized("GitHubToken Configuration Parameter Not Found");
            }
        }

        [TrakHoundApiQuery("{organization}/{repository}/{releaseId}")]
        public async Task<TrakHoundApiResponse> Get([FromRoute] string organization, [FromRoute] string repository, [FromRoute] int releaseId)
        {
            var token = GitHubTokens.GetToken(Configuration);
            if (token != null)
            {
                var githubClient = new GitHubClient(new ProductHeaderValue("trakhound-github-api"));

                var tokenAuth = new Credentials(token);
                githubClient.Credentials = tokenAuth;

                var release = await githubClient.Repository.Release.Get(organization, repository, releaseId);
                if (release != null)
                {
                    return Ok(release);
                }
                else
                {
                    return NotFound($"No Releases found for '{organization}/{repository}'");
                }
            }
            else
            {
                return NotAuthorized("GitHubToken Configuration Parameter Not Found");
            }
        }
    }
}
