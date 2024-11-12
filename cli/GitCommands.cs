// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using LibGit2Sharp;

namespace TrakHound.CLI
{
    static class GitCommands
    {
        public static string GetCurrentBranch(string path)
        {
            try
            {
                var gitPath = Repository.Discover(path);
                if (gitPath != null)
                {
                    using (var repo = new Repository(gitPath))
                    {
                        return repo.Head.FriendlyName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Git Error : GetCurrentBranch() : {ex.Message}");
            }

            return null;
        }

        public static string GetLatestCommit(string path)
        {
            try
            {
                var gitPath = Repository.Discover(path);
                if (gitPath != null)
                {
                    using (var repo = new Repository(gitPath))
                    {
                        var commit = repo.Commits?.Take(1).FirstOrDefault();
                        if (commit != null)
                        {
                            return commit.Sha;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Git Error : GetLatestCommit() : {ex.Message}");
            }

            return null;
        }
    }
}
