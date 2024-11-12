// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Packages;
using TrakHound.Volumes;

namespace TrakHound.Blazor.Routing
{
    public class Route
    {
        public string Id
        {
            get
            {
                if (Handler != null)
                {
                    return $"{AppId}:{Handler.FullName}".ToMD5Hash();
                }

                return null;
            }
        }

        public string AppId { get; set; }

        public string AppName { get; set; }

        public TrakHoundPackage Package { get; set; }

        public ITrakHoundClient Client { get; set; }

        public ITrakHoundVolume Volume { get; set; }

        public ITrakHoundLogger Logger { get; set; }

        public string BaseUrl { get; set; }

        public string BasePath { get; set; }

        public string BaseLocation { get; set; }

        public string Template { get; set; }

        public Type Handler { get; set; }


        public MatchResult Match(string url)
        {
            if (IsRouteMatch(url, Template)) return MatchResult.Match(this);

            return MatchResult.NoMatch();
        }

        private static bool IsRouteMatch(string url, string route)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(route))
            {
                var routeRegex = CreateRouteRegexPattern(route);

                var urlRegex = new Regex(routeRegex);
                if (urlRegex.IsMatch(url))
                {
                    return true;
                }
            }

            return false;
        }

        private static string CreateRouteRegexPattern(string route)
        {
            var pattern = route;

            if (!string.IsNullOrEmpty(route))
            {
                var regex = new Regex(@"(\{(.*?)\})");
                if (regex.IsMatch(route))
                {
                    var matches = regex.Matches(route);
                    if (matches != null)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Groups?.Count > 1)
                            {
                                var matchText = match.Groups[1].ToString();


                                // Check for Slug Match
                                var slugRegex = new Regex(@"(\{\*(.*?)(?:\:.+)?\})");
                                if (slugRegex.IsMatch(matchText))
                                {
                                    pattern = pattern.Replace(matchText, ".*");
                                }

                                // Check for Exact Match
                                var exactRegex = new Regex(@"(\{(.*?)(?:\:.+)?\})");
                                if (exactRegex.IsMatch(matchText))
                                {
                                    pattern = pattern.Replace(matchText, "[^\\/]*");
                                }
                            }
                        }
                    }
                }
            }

            return $"^{pattern}$";
        }
    }
}
