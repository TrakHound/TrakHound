// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Octokit;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.GitHub.Api.Controllers
{
    [TrakHoundApiController("issues")]
    public class IssueController : TrakHoundApiController
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

                var issues = await githubClient.Issue.GetAllForRepository(organization, repository);
                if (!issues.IsNullOrEmpty())
                {
                    return Ok(issues);
                }
                else
                {
                    return NotFound($"No Issues found for '{organization}/{repository}'");
                }
            }
            else
            {
                return NotAuthorized("GitHubToken Configuration Parameter Not Found");
            }
        }

        [TrakHoundApiQuery("{organization}/{repository}/{issueId}")]
        public async Task<TrakHoundApiResponse> Get([FromRoute] string organization, [FromRoute] string repository, [FromRoute] int issueId)
        {
            var token = GitHubTokens.GetToken(Configuration);
            if (token != null)
            {
                var githubClient = new GitHubClient(new ProductHeaderValue("trakhound-github-api"));

                var tokenAuth = new Credentials(token);
                githubClient.Credentials = tokenAuth;

                var issue = await githubClient.Issue.Get(organization, repository, issueId);
                if (issue != null)
                {
                    return Ok(issue);
                }
                else
                {
                    return NotFound($"No Issues found for '{organization}/{repository}'");
                }
            }
            else
            {
                return NotAuthorized("GitHubToken Configuration Parameter Not Found");
            }
        }

        [TrakHoundApiPublish("{organization}/{repository}")]
        public async Task<TrakHoundApiResponse> Publish(
            [FromRoute] string organization,
            [FromRoute] string repository,
            [FromQuery] string title,
            [FromBody(ContentType = "text/plain")] string body
            )
        {
            var token = GitHubTokens.GetToken(Configuration);
            if (token != null)
            {
                var githubClient = new GitHubClient(new ProductHeaderValue("trakhound-github-api"));

                var tokenAuth = new Credentials(token);
                githubClient.Credentials = tokenAuth;

                var issue = new NewIssue(title);
                issue.Body = body;

                var createdIssue = await githubClient.Issue.Create(organization, repository, issue);
                if (createdIssue != null)
                {
                    return Ok(createdIssue);
                }
                else
                {
                    return InternalError($"Error Creating Issue for '{organization}/{repository}'");
                }
            }
            else
            {
                return NotAuthorized("GitHubToken Configuration Parameter Not Found");
            }
        }
    }
}
